using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public class FunctionGroup
    {
        private readonly List<PolynomialFunction> _functions = new List<PolynomialFunction>(); 

        public void AddFunction(PolynomialFunction function)
        {
            _functions.Add(function);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <returns>true if the function is known and was removed, otherwise false</returns>
        public bool RemoveFunction(PolynomialFunction function)
        {
            var index = _functions.IndexOf(function);
            if (index == -1) 
            {
                // function is not contained
                return false;
            }
            else
            {
                _functions.RemoveAt(index);
                return true;
            }
        }

        public double GetPeak()
        {
            return 0;
        }

    }
}
