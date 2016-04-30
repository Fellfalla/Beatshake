using System;

namespace Beatshake.DependencyServices
{
    public interface IUserTextNotifier
    {
        void Notify (string message);
        void Notify (Exception exception);
    }
}
