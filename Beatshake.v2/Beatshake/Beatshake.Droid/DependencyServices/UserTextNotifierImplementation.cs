using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Beatshake.DependencyServices;

namespace Beatshake.Droid.DependencyServices
{
    class UserTextNotifierImplementation : IUserTextNotifier
    {
        public async Task Notify(string message)
        {
            await Task.Factory.StartNew(() =>
            {
                var service = (NotificationManager) Application.Context.GetSystemService(Context.NotificationService);
                service.Notify(0, new Notification(0, message));
            });
        }

        public async Task Notify(Exception exception)
        {
            await Notify(exception.ToString());
        }
    }
}
