//using System.ComponentModel;
//using Beatshake.UWP;
//using OxyPlot.Windows;
//using OxyPlot.Xamarin.Forms;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.UWP;
//using PlotView = OxyPlot.Xamarin.Forms.PlotView;

//// Exports the renderer.
//[assembly: ExportRenderer(typeof(PlotView), typeof(PlotViewRenderer))]

//namespace Beatshake.UWP
//{
//    /// <summary>
//    /// Provides a custom <see cref="OxyPlot.Xamarin.Forms.PlotView" /> renderer for UWP projects with xamarin forms. 
//    /// </summary>
//    public class PlotViewRenderer : ViewRenderer<PlotView, OxyPlot.Windows.PlotView>
//    {
//        /// <summary>
//        /// Initializes static members of the <see cref="PlotViewRenderer"/> class.
//        /// </summary>
//        static PlotViewRenderer()
//        {
//            Init();
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="PlotViewRenderer"/> class.
//        /// </summary>
//        public PlotViewRenderer()
//        {
//            // Do not delete
//        }

//        /// <summary>
//        /// Initializes the renderer.
//        /// </summary>
//        /// <remarks>This method must be called before a <see cref="T:PlotView" /> is used.</remarks>
//        public static void Init()
//        {
//            OxyPlot.Xamarin.Forms.PlotView.IsRendererInitialized = true;
//        }

//        /// <summary>
//        /// Raises the element changed event.
//        /// </summary>
//        /// <param name="e">The event arguments.</param>
//        protected override void OnElementChanged(ElementChangedEventArgs<PlotView> e)
//        {
//            base.OnElementChanged(e);
//            if (e.OldElement != null || this.Element == null)
//            {
//                return;
//            }

//            var plotView = new OxyPlot.Windows.PlotView
//            {
//                Model = this.Element.Model,
//                Controller = this.Element.Controller,
//                Background = this.Element.BackgroundColor.ToOxyColor().ToBrush()
//            };

//            this.SetNativeControl(plotView);
//        }

//        /// <summary>
//        /// Raises the element property changed event.
//        /// </summary>
//        /// <param name="sender">The sender.</param>
//        /// <param name="e">The event arguments.</param>
//        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            base.OnElementPropertyChanged(sender, e);
//            if (this.Element == null || this.Control == null)
//            {
//                return;
//            }

//            if (e.PropertyName == OxyPlot.Xamarin.Forms.PlotView.ModelProperty.PropertyName)
//            {
//                this.Control.Model = this.Element.Model;
//            }

//            if (e.PropertyName == OxyPlot.Xamarin.Forms.PlotView.ControllerProperty.PropertyName)
//            {
//                this.Control.Controller = this.Element.Controller;
//            }

//            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
//            {
//                this.Control.Background = this.Element.BackgroundColor.ToOxyColor().ToBrush();
//            }
//        }
//    }
//}
