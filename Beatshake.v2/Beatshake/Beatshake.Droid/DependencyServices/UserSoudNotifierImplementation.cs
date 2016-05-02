using Android.App;
using Android.Media;
using Beatshake.DependencyServices;
using Java.Lang;
using Uri = Android.Net.Uri;

namespace Beatshake.Droid.DependencyServices
{
    public class UserSoudNotifierImplementation : IUserSoudNotifier
    {

        public void Notify()
        {
            try
            {
                Uri notification = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
                Ringtone r = RingtoneManager.GetRingtone(Application.Context, notification);
                r.Play();
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
        }
    }
}
