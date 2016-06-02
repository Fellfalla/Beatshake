using Beatshake.DependencyServices;

namespace Beatshake.ViewModels
{
    public interface IMotionDataProcessor
    {
        IMotionDataProvider MotionDataProvider { get; set; }

        void ProcessMotionData(IMotionDataProvider sender);
    }
}