using System;
using System.Threading.Tasks;

namespace Beatshake.DependencyServices
{
    public interface IUserTextNotifier
    {
        Task Notify (string message);
        Task Notify (Exception exception);

        Task<int> DecisionNotification(string message, params string[] buttons);
    }
}
