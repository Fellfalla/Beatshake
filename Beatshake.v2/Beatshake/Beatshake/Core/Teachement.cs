using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AForge.Neuro;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;
using MathNet.Numerics;
using Xamarin.Forms;


namespace Beatshake.Core
{
    public interface ITeachable
    {
        Task<ITeachable> TeachMovement(IMotionDataProvider motionDataProvider);
        //bool FitsDataSet(double tolerance, double[] data);
    }

    public class NeuralTeachement : ITeachable
    {
        private static readonly int numberOfDimensions = 3;

        public static readonly int NumberOfPhysicalDatasets = Enum.GetNames(typeof(MotionData)).Length - 2; // minus MotionData.All & MotionData.None
        public static readonly int HiddenLayers = 1;

        // + BeatshakeSettings cause this is the amount of Timestamps
        public static readonly int InputNeurons = NumberOfPhysicalDatasets * numberOfDimensions * BeatshakeSettings.SamplePoints + BeatshakeSettings.SamplePoints; 
        public static readonly int OutputNeurons = 1;
        public static readonly int HiddenNeurons = (InputNeurons + OutputNeurons)/2;

        public NeuralTeachement()
        {
            ConfigureBrain();
        }

        private ActivationNetwork _brain;

        private void ConfigureBrain()
        {
            var hidden = new int[HiddenLayers].Populate(HiddenNeurons);
            int[] layerSizes = hidden.Concat(new int[] {OutputNeurons}).ToArray();
             _brain = new ActivationNetwork(new ThresholdFunction(), InputNeurons, layerSizes);
        }

        public bool FitsDataSet(double tolerance, double[] data)
        {
            _brain.Compute(TransformFunctionsToNetworkInputs(data));
            return false;
        }

        public static double[] TransformFunctionsToNetworkInputs(params IEnumerable<double>[] data)
        {
            //double[] inputData;
            List<double> tempList = new List<double>();
            foreach (var list in data)
            {
                tempList.AddRange(list);
            }
            return tempList.ToArray();
        }


        public static double[] TransformToNetworkInputs(IMotionDataProvider motionDataProvider)
        {
            var data =  motionDataProvider.Jolt.XTrans              .Concat(
                        motionDataProvider.Jolt.YTrans              ).Concat(
                        motionDataProvider.Jolt.ZTrans              ).Concat(
                        motionDataProvider.Jolt.XRot                ).Concat(
                        motionDataProvider.Jolt.YRot                ).Concat(
                        motionDataProvider.Jolt.ZRot                ).Concat(
                        
                        motionDataProvider.AbsAcceleration.XTrans   ).Concat(
                        motionDataProvider.AbsAcceleration.YTrans   ).Concat(
                        motionDataProvider.AbsAcceleration.ZTrans   ).Concat(
                        motionDataProvider.AbsAcceleration.XRot     ).Concat(
                        motionDataProvider.AbsAcceleration.YRot     ).Concat(
                        motionDataProvider.AbsAcceleration.ZRot     ).Concat(
                        
                        motionDataProvider.RelAcceleration.XTrans   ).Concat(
                        motionDataProvider.RelAcceleration.YTrans   ).Concat(
                        motionDataProvider.RelAcceleration.ZTrans   ).Concat(
                        motionDataProvider.RelAcceleration.XRot     ).Concat(
                        motionDataProvider.RelAcceleration.YRot     ).Concat(
                        motionDataProvider.RelAcceleration.ZRot     ).Concat(
                        
                        motionDataProvider.Velocity.XTrans          ).Concat(
                        motionDataProvider.Velocity.YTrans          ).Concat(
                        motionDataProvider.Velocity.ZTrans          ).Concat(
                        motionDataProvider.Velocity.XRot            ).Concat(
                        motionDataProvider.Velocity.YRot            ).Concat(
                        motionDataProvider.Velocity.ZRot            ).Concat(
                        
                        motionDataProvider.Pose.XTrans              ).Concat(
                        motionDataProvider.Pose.YTrans              ).Concat(
                        motionDataProvider.Pose.ZTrans              ).Concat(
                        motionDataProvider.Pose.XRot                ).Concat(
                        motionDataProvider.Pose.YRot                ).Concat(
                        motionDataProvider.Pose.ZRot                ).Concat(
                        
                        motionDataProvider.Timestamps               );
                //motionDataProvider.RelAcceleration).Concat(
                //    motionDataProvider.AbsAcceleration).Concat(
                //        motionDataProvider.Velocity).Concat(
                //            motionDataProvider.Pose).Concat(motionDataProvider.Timestamps);
            return data.ToArray();
        }

        public static NeuralTeachement Create()
        {
            return new NeuralTeachement();
        }

        /// <summary>
        /// <exception cref="InsufficientDataException">Thrown if a peak is to close to the start of the data</exception>
        /// </summary>
        /// <param name="outputs"></param>
        /// <param name="throwOnThinData"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public static NeuralTeachement Create(double[] inputs, double[] outputs, bool throwOnThinData = false)
        {
            var teachement = new NeuralTeachement();
            teachement.Train(inputs, outputs);
            return teachement;
        }

        async Task<ITeachable> ITeachable.TeachMovement(IMotionDataProvider motionDataProvider)
        {
            return await TeachMovement(motionDataProvider);
        }

        public void Train(double[] inputs, double[] outputs, bool throwOnThinData = false)
        {
            var learner = new AForge.Neuro.Learning.BackPropagationLearning(_brain);
            learner.Run(inputs, outputs);
        }


        public static async Task<NeuralTeachement> TeachMovement(IMotionDataProvider motionDataProvider)
        {
            const int finishButtonIndex = 0;
            const int continueButtonIndex = 1;
            var buttons = new string[2];
            buttons[finishButtonIndex] =  "Finish";
            buttons[continueButtonIndex] = "Teach";
            const string message = "Click \"Finish\" when you finished train the neural network, otherwise \"Teach\".";

            var trainingTask = Task<NeuralTeachement>.Factory.StartNew(() =>
            {
                // init training task
                NeuralTeachement teachement = NeuralTeachement.Create();
                IUserTextNotifier notifier = Xamarin.Forms.DependencyService.Get<IUserTextNotifier>();


                RerunTeachement:
                var userConfirmation = notifier.DecisionNotification(message, buttons);
                while (!userConfirmation.IsCompleted)
                {

                    var task = Task.Delay((int)motionDataProvider.RefreshRate);

                    // Teach
                    teachement.Train(NeuralTeachement.TransformToNetworkInputs(motionDataProvider),  new double[] {0}); // 0 cause nothing is triggered by user

                    task.Wait();
                }

                teachement.Train(NeuralTeachement.TransformToNetworkInputs(motionDataProvider), new double[] { 1 });

                userConfirmation.Wait();
                if (userConfirmation.Result == continueButtonIndex )
                {
                    goto RerunTeachement;
                }

                return teachement;
            });

            //userConfirmation.Wait(30000);
            //await userConfirmation.ConfigureAwait(true);
            NeuralTeachement finishedTeachement = null;
            try
            {
                //Task.WaitAll(userConfirmation, measureTask);

                finishedTeachement = await trainingTask;
            }
            catch (TaskCanceledException)
            {
            }
            catch (InsufficientDataException) // thrown if the peak is to near at beginning data
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Teachement failed. Please try again. (longer)");
            }
            catch (InvalidOperationException)
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");
            }

            if (finishedTeachement == null)
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Teachement failed. Please try again. (longer)");
            }

            //try
            //{
            //    var normalizedTimestamps = Utility.NormalizeTimeStamps(timesteps);
            //    teachement = NeuralTeachement.Create(normalizedTimestamps.ToArray(), false, xAcc.ToList(), yAcc.ToList(), zAcc.ToList()); // todo: improve performance by avoiding conversion to list


            //}
            //catch (InsufficientDataException) // thrown if the peak is to near at beginning data
            //{
            //    await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Teachement failed. Please try again. (longer)");
            //}
            //catch (InvalidOperationException)
            //{
            //    await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");
            //}


            return finishedTeachement;
        }

    }

    public class Teachement : FunctionGroup, ITeachable
    {
        public PolynomialFunction XCurve
        {
            get { return Functions[0]; }
            set { Functions[0] = value; }
        }
        public PolynomialFunction YCurve
        {
            get { return Functions[1]; }
            set { Functions[1] = value; }
        }
        public PolynomialFunction ZCurve
        {
            get { return Functions[2]; }
            set { Functions[2] = value; }
        }

        private double _xPosition;

        private double _yPosition;

        private double _zPosition;

        /// <summary>
        /// <exception cref="InsufficientDataException">Thrown if a peak is to close to the start of the data</exception>
        /// </summary>
        /// <param name="timesteps"></param>
        /// <param name="throwOnThinData"></param>
        /// <param name="valueArrays"></param>
        /// <returns></returns>
        public static Teachement Create(double[] timesteps, bool throwOnThinData = false, params IEnumerable<double>[] valueArrays)
        {
            var teachement = new Teachement();

            IEnumerable<double> xValues = valueArrays[0];
            IEnumerable<double> yValues = valueArrays[1];
            IEnumerable<double> zValues = valueArrays[2];
            // Get point with highest absolute Acceleration
            var endIndex = DataAnalyzer.GetPeak(xValues , yValues, zValues);
            int startIndex = endIndex - BeatshakeSettings.SamplePoints;

            if (endIndex == -1)
            {
                throw new InvalidOperationException("No peak value could be detected.");
            }
            if (startIndex < 0)
            {
                if (throwOnThinData)
                {
                    throw new InsufficientDataException();
                }
                else
                {
                    startIndex = 0;
                }
            }
            teachement.Functions.Clear();
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                xValues.SubEnumerable(startIndex, endIndex)));
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                yValues.SubEnumerable(startIndex, endIndex)));
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                zValues.SubEnumerable(startIndex, endIndex)));


            return teachement;
        }

        async Task<ITeachable> ITeachable.TeachMovement(IMotionDataProvider motionDataProvider)
        {
            return await Teachement.TeachMovement(motionDataProvider);
        }

        public static async Task<Teachement> TeachMovement(IMotionDataProvider motionDataProvider)
        {

            // record movement
            Teachement teachement = null;

            //List<double> xAcc = new List<double>();
            //List<double> yAcc = new List<double>();
            //List<double> zAcc = new List<double>();

            //List<double> xPos = new List<double>();
            //List<double> yPos = new List<double>();
            //List<double> zPos = new List<double>();

            //List<double> timesteps = new List<double>();
            var userConfirmation = Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");

            //var measureTask = Task.Factory.StartNew(() =>
            //{
            //    while (!userConfirmation.IsCompleted)
            //    {
            //        timesteps.Add(motionDataProvider.RelAcceleration.Timestamp);

            //        // Set Accelerations
            //        xAcc.Add(motionDataProvider.RelAcceleration.Trans[0]);
            //        yAcc.Add(motionDataProvider.RelAcceleration.Trans[1]);
            //        zAcc.Add(motionDataProvider.RelAcceleration.Trans[2]);

            //        // Set Positions
            //        xPos.Add(motionDataProvider.Pose.Trans[0]);
            //        yPos.Add(motionDataProvider.Pose.Trans[1]);
            //        zPos.Add(motionDataProvider.Pose.Trans[2]);

            //        var task = Task.Delay((int)motionDataProvider.RefreshRate);
            //        task.Wait();
            //    }
            //});

            //userConfirmation.Wait(30000);
            await userConfirmation.ConfigureAwait(true);
            //try
            //{
            //    //Task.WaitAll(userConfirmation, measureTask);

            //    await measureTask;
            //}
            //catch (TaskCanceledException)
            //{
            //}

            try
            {
                var normalizedTimestamps = Utility.NormalizeTimeStamps(motionDataProvider.Timestamps);
                teachement = Teachement.Create(normalizedTimestamps.ToArray(), false, motionDataProvider.RelAcceleration.XTrans, 
                    motionDataProvider.RelAcceleration.YTrans, 
                    motionDataProvider.RelAcceleration.ZTrans);

                int peakPos = DataAnalyzer.GetPeak(motionDataProvider.RelAcceleration.XTrans,
                    motionDataProvider.RelAcceleration.YTrans,
                    motionDataProvider.RelAcceleration.ZTrans);

                teachement._xPosition = motionDataProvider.Pose.XTrans[peakPos];
                teachement._yPosition = motionDataProvider.Pose.YTrans[peakPos];
                teachement._zPosition = motionDataProvider.Pose.ZTrans[peakPos];
            }
            catch (InsufficientDataException) // thrown if the peak is to near at beginning data
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Teachement failed. Please try again. (longer)");
            }
            catch (InvalidOperationException)
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");
            }


            return teachement;
        }

        ///// <summary>
        ///// <seealso cref="Create(double[],double[],double[],double[])"/>
        ///// </summary>
        ///// <param name="timesteps"></param>
        ///// <param name="xValues"></param>
        ///// <param name="yValues"></param>
        ///// <param name="zValues"></param>
        ///// <returns></returns>
        //public static Teachement Create(IEnumerable<double> timesteps, bool throwOnThinData = false, params IEnumerable<double>[] valueArrays)
        //{
        //        return Create(timesteps.ToArray(), throwOnThinData, valueArrays.Select(doubles => doubles.ToArray()).ToArray());
        //}


        public bool FitsDataSet(double tolerance, double start, double end, ComparisonStrategy strategy, FunctionGroup functions)
        {
            switch (strategy)
            {
                case ComparisonStrategy.Absolute: throw new NotImplementedException();
                case ComparisonStrategy.CoefficientNormalized: throw new NotImplementedException();
                case ComparisonStrategy.Position:

                case ComparisonStrategy.PeakNormalized:
                    functions.PeakNormalizeDownTo(this);
                    var difference = GetDifferenceIntegral(end, start, functions);
                    if (DataAnalyzer.AreFunctionsAlmostEqual(difference, tolerance, end - start))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
            return false;
        }

        public bool FitsPositionData(double tolerance, IMotionDataProvider motionData)
        {
            return motionData.Pose.XTrans.Peek().IsAlmostEqual(_xPosition, tolerance) &&
                   motionData.Pose.YTrans.Peek().IsAlmostEqual(_yPosition, tolerance) &&
                   motionData.Pose.ZTrans.Peek().IsAlmostEqual(_zPosition, tolerance);
        }
    }
}
