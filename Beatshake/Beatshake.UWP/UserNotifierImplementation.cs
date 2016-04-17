using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Beatshake.Core;

namespace Beatshake.UWP
{
    class UserNotifierImplementation : IUserNotifier
    {
        public async void Notify(string message)
        {
            await
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.High,
                    async () =>
                    {
                        var dialog = new MessageDialog(message);
                        await dialog.ShowAsync();
                    });

        }

        public void Notify(Exception exception)
        {
            Notify(exception.ToString());
        }
    }
}