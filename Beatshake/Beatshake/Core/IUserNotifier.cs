using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public interface IUserNotifier
    {
        void Notify (string message);
        void Notify (Exception exception);
    }
}
