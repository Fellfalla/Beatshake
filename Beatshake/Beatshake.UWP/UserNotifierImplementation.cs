using System;
using Windows.UI.Popups;
using Beatshake.Core;

namespace Beatshake.UWP
{
    class UserNotifierImplementation : IUserNotifier
    {
        public async void Notify(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        public void Notify(Exception exception)
        {
            Notify(exception.ToString());
        }
    }
}