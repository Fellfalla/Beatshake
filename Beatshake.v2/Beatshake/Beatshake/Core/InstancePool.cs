using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The Type that gives instances</typeparam>
    public class InstancePool<T> where T:new()
    {
        private bool[] _isAlive;

        private T[] _pool;

        public InstancePool(int poolSize)
        {
            _pool = new T[poolSize];
            _isAlive = new bool[poolSize];

            for (int i = 0; i < _pool.Length; i++)
            {
                _isAlive[i] = false;
                _pool[i] = new T();
            }
        }

        public T GetInstance()
        {
            int _;
            return GetInstance(out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number">
        /// The position of the instance in the _pool. Use this for faster freeing.</param>
        /// <returns></returns>
        public T GetInstance(out int number)
        {
            for (int i = 0; i < _pool.Length; i++) // todo: make access faster (do not start at begin every time)
            {
                if (!_isAlive[i])
                {
                    _isAlive[i] = true;
                    number = i;
                    return _pool[i];
                }
            }

            // if the code runs to this point, all instances are in use -> Create more
            EnlargePool();
            return GetInstance(out number);
        }

        /// <summary>
        /// Doubles the size of the _pool
        /// </summary>
        private void EnlargePool()
        {
            int amountOfNewElements = _pool.Length;
            var newElements = new T[amountOfNewElements];
            var newBools = new bool[amountOfNewElements];
            for (int i = 0; i < amountOfNewElements; i++)
            {
                newElements[i] = new T();
                newBools[i] = false;
            }
            _pool = _pool.Concat(newElements).ToArray();
            _isAlive = _isAlive.Concat(newBools).ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>True if the instance could be freed (even if it was not blocked), false if the instance was not found</returns>
        public bool Unlock(T instance)
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                if (ReferenceEquals(_pool[i], instance))
                {
                    return Unlock(i);
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="instanceNumber"></param>
        /// <returns>True if the instance could be freed (even if it was not blocked), false if the instance was not found</returns>
        public bool Unlock(int instanceNumber)
        {
            if (instanceNumber < 0 || instanceNumber >= _isAlive.Length)
            {
                return false;
            }

            _isAlive[instanceNumber] = false;
            return true;

        }

    }
}
