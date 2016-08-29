using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Beatshake.Core
{
    /// <summary>
    /// Source: http://stackoverflow.com/a/384097
    /// This stack Drops out the FI-Item if maxCapacity is reachend by a newly inserted element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropOutStack<T> : IEnumerable<T>, ICollection<T>, ICollection
    {
        private T[] _items;
        private int _top = 0;
        private int _count = 0;
        private readonly object _syncRoot = new object();

        public DropOutStack(int capacity)
        {
            _items = new T[capacity];
        }

        public T this[int i]
        {
            get
            {
                if (i < 0 || i > _items.Length - 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(i));
                }

                int accessedItem = LowerStackIndex(_top, _items.Length);

                while (i > 0)
                {
                    accessedItem = LowerStackIndex(accessedItem, _items.Length);
                    i--;
                }

                return _items[accessedItem];
            }
        }

        public void Push(T item)
        {
            _items[_top] = item;

            _top = RaiseStackIndex(_top, _items.Length);
        }

        public T Pop()
        {
            _top = LowerStackIndex(_top, _items.Length);
            return _items[_top];
        }

        /// <summary>
        /// Get topmost element without removing it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return _items[LowerStackIndex(_top, _items.Length)];
        }

        private static int RaiseStackIndex(int index, int max)
        {
            return (index + 1) % max;
        }

        private static int LowerStackIndex(int index, int max)
        {
            return (max + index - 1) % max;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DropOutStackEnumerator(_top, this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class DropOutStackEnumerator : IEnumerator<T>
        {
            private int _currentIndex;
            private readonly int _startIndex;
            private DropOutStack<T> _stack;

            public DropOutStackEnumerator(int startIndex, DropOutStack<T> stack)
            {
                _startIndex = startIndex;
                _currentIndex = startIndex;
                _stack = stack;
            }

            private int CurrentIndex
            {
                get { return _currentIndex; }
                set
                {
                    _currentIndex = value;
                    if (_stack._items.Length > value && value > -1)
                    {
                        Current = _stack._items[value];
                    }
                    else
                    {
                        Current = default(T);
                    }
                }
            }

            public bool MoveNext()
            {
                var newIndex = LowerStackIndex(CurrentIndex, _stack._items.Length);
                if (newIndex != _startIndex)
                {
                    CurrentIndex = newIndex;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                _currentIndex = _startIndex;
            }

            public T Current { get; set; }

            object IEnumerator.Current
            {
                get { return Current; }

            }

            public void Dispose()
            {
                
            }
        }

        public void Add(T item)
        {
            Push(item);
        }

        public void Clear()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i] = default(T);
            }
            _top = 0;
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.CopyTo((Array)array, arrayIndex);
        }

        /// <summary>
        /// Not Supported
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(Array array, int index)
        {

            var tempIndex = _top;
            for (int i = 0; i < index + this.Count; i++)
            {
                tempIndex = LowerStackIndex(tempIndex, _items.Length);
                array.SetValue(_items[tempIndex], i);
            }

        }

        public int Count
        {
            get { return _items.Length; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}