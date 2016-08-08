using Beatshake.ViewModels;
using Beatshake.Views;
using Xunit;

namespace Beatshake.Tests.Views
{
    public class MainMenuViewTest
    {
        [Fact]
        public void SmokeTest()
        {
            var _ = new MainMenuView(new MainMenuViewModel(null));
        }
    }
}