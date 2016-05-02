using System;
using Android.App;
using Android.Content;
using Beatshake.DependencyServices;

namespace Beatshake.Droid.DependencyServices
{
    class UserTextNotifierImplementation : IUserTextNotifier
    {
        public async void Notify(string message)
        {
            var service = (NotificationManager) Application.Context.GetSystemService(Context.NotificationService);
            service.Notify(0, new Notification(0, message));
        }

        public void Notify(Exception exception)
        {
            Notify(exception.ToString());
        }
    }
}
