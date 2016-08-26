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
    public class NeuralTeachement
    {
        private static readonly int numberOfDimensions = 3;

        public static readonly int NumberOfPhysicalDatasets = Enum.GetNames(typeof(MotionData)).Length - 2; // minus MotionData.All & MotionData.None
        public static readonly int HiddenLayers = 1;
        public static readonly int InputNeurons = NumberOfPhysicalDatasets * numberOfDimensions * BeatshakeSettings.SamplePoints; 
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

        /// <summary>
        /// <exception cref="InsufficientDataException">Thrown if a peak is to close to the start of the data</exception>
        /// </summary>
        /// <param name="timesteps"></param>
        /// <param name="throwOnThinData"></param>
        /// <param name="valueArrays"></param>
        /// <returns></returns>
        public static NeuralTeachement Create(double[] timesteps, int output, bool throwOnThinData = false,  params IEnumerable<double>[] valueArrays)
        {
            //trainingData.SetTrainData();

            var teachement = new NeuralTeachement();
            var learner = new AForge.Neuro.Learning.BackPropagationLearning(teachement._brain);
            learner.Run(TransformFunctionsToNetworkInputs(valueArrays),new double []{ output });

            //IList<double> xValues = valueArrays[0];
            //IList<double> yValues = valueArrays[1];
            //IList<double> zValues = valueArrays[2];
            //// Get point with highest absolute Acceleration
            //var endIndex = DataAnalyzer.GetPeak(xValues, yValues, zValues);
            //int startIndex = endIndex - BeatshakeSettings.SamplePoints;

            //if (endIndex == -1)
            //{
            //    throw new InvalidOperationException("No peak value could be detected.");
            //}
            //if (startIndex < 0)
            //{
            //    if (throwOnThinData)
            //    {
            //        throw new InsufficientDataException();
            //    }
            //    else
            //    {
            //        startIndex = 0;
            //    }
            //}
            return teachement;
        }

        public void Train(double[] timesteps, int output, params IEnumerable<double>[] valueArrays)
        {
            var learner = new AForge.Neuro.Learning.BackPropagationLearning(_brain);
            learner.Run(TransformFunctionsToNetworkInputs(valueArrays), new double[] { output });
        }


        public static async Task<NeuralTeachement> TeachMovement(IMotionDataProvider motionDataProvider)
        {
            const int finishButtonIndex = 0;
            const int continueButtonIndex = 1;
            var buttons = new string[2];
            buttons[finishButtonIndex] =  "Finish";
            buttons[continueButtonIndex] = "Teach";
            const string message = "Click \"Finish\" when you finished train the neural network, otherwise \"Teach\".";

            // record movement

            var xAccTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yAccTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zAccTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);

            var xAccRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yAccRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zAccRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);

            var xAccTransAbs    = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yAccTransAbs    = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zAccTransAbs    = new DropOutStack<double>(BeatshakeSettings.SamplePoints);

            var xAccRotAbs      = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yAccRotAbs      = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zAccRotAbs      = new DropOutStack<double>(BeatshakeSettings.SamplePoints);

            var xVelTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yVelTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zVelTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);

            var xVelRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yVelRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zVelRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);


            var xPosTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yPosTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zPosTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var xPosRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yPosRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zPosRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);

            var xJoltTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yJoltTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zJoltTrans       = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var xJoltRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var yJoltRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);
            var zJoltRot         = new DropOutStack<double>(BeatshakeSettings.SamplePoints);

            var measureData = new DropOutStack<double>[]
            {
                xAccTrans   ,
                yAccTrans   ,
                zAccTrans   ,
                xAccRot     ,
                yAccRot     ,
                zAccRot     ,
                xAccTransAbs,
                yAccTransAbs,
                zAccTransAbs,
                xAccRotAbs  ,
                yAccRotAbs  ,
                zAccRotAbs  ,
                xVelTrans   ,
                yVelTrans   ,
                zVelTrans   ,
                xVelRot     ,
                yVelRot     ,
                zVelRot     ,
                xPosTrans   ,
                yPosTrans   ,
                zPosTrans   ,
                xPosRot     ,
                yPosRot     ,
                zPosRot     ,
                xJoltTrans  ,
                yJoltTrans  ,
                zJoltTrans  ,
                xJoltRot    ,
                yJoltRot    ,
                zJoltRot    ,
            };

            var timestamps = new DropOutStack<double>(BeatshakeSettings.SamplePoints);


            List<double> timesteps = new List<double>();


            var trainingTask = Task<NeuralTeachement>.Factory.StartNew(() =>
            {
                // init training task
                NeuralTeachement teachement = null;
                IUserTextNotifier notifier = Xamarin.Forms.DependencyService.Get<IUserTextNotifier>();


                RerunTeachement:
                var userConfirmation = notifier.DecisionNotification(message, buttons);
                while (!userConfirmation.IsCompleted)
                {
                    timesteps.Add(motionDataProvider.RelAcceleration.Timestamp);

                    // Set Accelerations
                    xAccTrans       .Push(motionDataProvider.RelAcceleration.Trans[0]);
                    yAccTrans       .Push(motionDataProvider.RelAcceleration.Trans[1]);
                    zAccTrans       .Push(motionDataProvider.RelAcceleration.Trans[2]);

                    xAccRot         .Push(motionDataProvider.RelAcceleration.Rot[0]);
                    yAccRot         .Push(motionDataProvider.RelAcceleration.Rot[1]);
                    zAccRot         .Push(motionDataProvider.RelAcceleration.Rot[2]);

                    xAccTransAbs    .Push(motionDataProvider.AbsAcceleration.Trans[0]);
                    yAccTransAbs    .Push(motionDataProvider.AbsAcceleration.Trans[1]);
                    zAccTransAbs    .Push(motionDataProvider.AbsAcceleration.Trans[2]);

                    xAccRotAbs      .Push(motionDataProvider.AbsAcceleration.Rot[0]);
                    yAccRotAbs      .Push(motionDataProvider.AbsAcceleration.Rot[1]);
                    zAccRotAbs      .Push(motionDataProvider.AbsAcceleration.Rot[2]);

                    // Set Velocities
                    xVelTrans       .Push(motionDataProvider.Velocity.Trans[0]);
                    yVelTrans       .Push(motionDataProvider.Velocity.Trans[1]);
                    zVelTrans       .Push(motionDataProvider.Velocity.Trans[2]);
                    xVelRot         .Push(motionDataProvider.Velocity.Rot[0]);
                    yVelRot         .Push(motionDataProvider.Velocity.Rot[1]);
                    zVelRot         .Push(motionDataProvider.Velocity.Rot[2]);


                    // Set Positions
                    xPosTrans       .Push(motionDataProvider.Pose.Trans[0]);
                    yPosTrans       .Push(motionDataProvider.Pose.Trans[1]);
                    zPosTrans       .Push(motionDataProvider.Pose.Trans[2]);
                    xPosRot         .Push(motionDataProvider.Pose.Rot[0]);
                    yPosRot         .Push(motionDataProvider.Pose.Rot[1]);
                    zPosRot         .Push(motionDataProvider.Pose.Rot[2]);

                    // Set Jolt
                    xJoltTrans       .Push(motionDataProvider.Jolt.Trans[0]);
                    yJoltTrans       .Push(motionDataProvider.Jolt.Trans[1]);
                    zJoltTrans       .Push(motionDataProvider.Jolt.Trans[2]);
                    xJoltRot         .Push(motionDataProvider.Jolt.Rot[0]);
                    yJoltRot         .Push(motionDataProvider.Jolt.Rot[1]);
                    zJoltRot         .Push(motionDataProvider.Jolt.Rot[2]);

                    timestamps.Push(motionDataProvider.RelAcceleration.Timestamp);

                    var task = Task.Delay((int)motionDataProvider.RefreshRate);

                    // Teach
                    if (teachement == null)
                    {
                        teachement = NeuralTeachement.Create(timestamps.ToArray(), 0, true, measureData);
                    }
                    else
                    {
                        teachement.Train(timestamps.ToArray(), 0, measureData);
                    }

                    task.Wait();
                }

                // Teach
                if (teachement == null)
                {
                    teachement = NeuralTeachement.Create(timestamps.ToArray(), 1, true, measureData);
                }
                else
                {
                    teachement.Train(timestamps.ToArray(), 1, measureData);
                }

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

    public class Teachement : FunctionGroup
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
        public static Teachement Create(double[] timesteps, bool throwOnThinData = false, params IList<double>[] valueArrays)
        {
            var teachement = new Teachement();

            IList<double> xValues = valueArrays[0];
            IList<double> yValues = valueArrays[1];
            IList<double> zValues = valueArrays[2];
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
                xValues.SubList(startIndex, endIndex)));
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                yValues.SubList(startIndex, endIndex)));
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                zValues.SubList(startIndex, endIndex)));


            return teachement;
        }


        public static async Task<Teachement> TeachMovement(IMotionDataProvider motionDataProvider)
        {

            // record movement
            Teachement teachement = null;

            List<double> xAcc = new List<double>();
            List<double> yAcc = new List<double>();
            List<double> zAcc = new List<double>();

            List<double> xPos = new List<double>();
            List<double> yPos = new List<double>();
            List<double> zPos = new List<double>();

            List<double> timesteps = new List<double>();
            var userConfirmation = Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");

            var measureTask = Task.Factory.StartNew(() =>
            {
                while (!userConfirmation.IsCompleted)
                {
                    timesteps.Add(motionDataProvider.RelAcceleration.Timestamp);

                    // Set Accelerations
                    xAcc.Add(motionDataProvider.RelAcceleration.Trans[0]);
                    yAcc.Add(motionDataProvider.RelAcceleration.Trans[1]);
                    zAcc.Add(motionDataProvider.RelAcceleration.Trans[2]);

                    // Set Positions
                    xPos.Add(motionDataProvider.Pose.Trans[0]);
                    yPos.Add(motionDataProvider.Pose.Trans[1]);
                    zPos.Add(motionDataProvider.Pose.Trans[2]);

                    var task = Task.Delay((int)motionDataProvider.RefreshRate);
                    task.Wait();
                }
            });

            //userConfirmation.Wait(30000);
            await userConfirmation.ConfigureAwait(true);
            try
            {
                //Task.WaitAll(userConfirmation, measureTask);

                await measureTask;
            }
            catch (TaskCanceledException)
            {
            }

            try
            {
                var normalizedTimestamps = Utility.NormalizeTimeStamps(timesteps);
                teachement = Teachement.Create(normalizedTimestamps.ToArray(), false, xAcc, yAcc, zAcc);

                int peakPos = DataAnalyzer.GetPeak(xAcc, yAcc, zAcc);

                teachement._xPosition = xPos[peakPos];
                teachement._yPosition = yPos[peakPos];
                teachement._zPosition = zPos[peakPos];
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
            return motionData.Pose.Trans[0].IsAlmostEqual(_xPosition, tolerance) &&
                   motionData.Pose.Trans[1].IsAlmostEqual(_yPosition, tolerance) &&
                   motionData.Pose.Trans[2].IsAlmostEqual(_zPosition, tolerance);
        }
    }
}
