using System;
using System.Threading.Tasks;

namespace Beatshake.DependencyServices
{
    public interface IUserTextNotifier
    {
        Task Notify (string message);
        Task Notify (Exception exception);
    }
}
