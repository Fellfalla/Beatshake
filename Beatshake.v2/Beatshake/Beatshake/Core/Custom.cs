using System;

namespace Beatshake.Core
{
    public static class Custom
    {
        public delegate void TypedEventHandler<TSender, TArgs>(TSender sender, TArgs args) where TArgs : EventArgs;

        public delegate void TypedEventHandler<TSender>(TSender sender);
    }
}
