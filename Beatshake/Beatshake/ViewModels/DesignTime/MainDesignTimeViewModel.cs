using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.ViewModels.DesignTime
{
    public class MainDesignTimeViewModel : MainViewModel
    {
        public new string Title
        {
            get { return "Beatshake"; }
        }
    }
}
