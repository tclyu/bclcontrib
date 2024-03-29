﻿namespace System.Collections.Frugal
{
    /// <summary>
    /// FrugalObjectList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FrugalObjectList<T>
    {
        internal FrugalListBase<T> _listStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrugalObjectList&lt;T&gt;"/> class.
        /// </summary>
        public FrugalObjectList()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FrugalObjectList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public FrugalObjectList(int size)
        {
            Capacity = size;
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int Add(T value)
        {
            if (_listStore == null)
            {
                _listStore = new SingleItemList<T>();
            }
            FrugalListStoreState state = _listStore.Add(value);
            if (state != FrugalListStoreState.Success)
            {
                if (FrugalListStoreState.ThreeItemList != state)
                {
                    if (FrugalListStoreState.SixItemList != state)
                    {
                        if (FrugalListStoreState.Array != state)
                        {
                            throw new InvalidOperationException(TR.Get("FrugalList_CannotPromoteBeyondArray"));
                        }
                        ArrayItemList<T> list3 = new ArrayItemList<T>(_listStore.Count + 1);
                        list3.Promote(_listStore);
                        _listStore = list3;
                        list3.Add(value);
                        _listStore = list3;
                    }
                    else
                    {
                        SixItemList<T> list2 = new SixItemList<T>();
                        list2.Promote(_listStore);
                        _listStore = list2;
                        list2.Add(value);
                        _listStore = list2;
                    }
                }
                else
                {
                    ThreeItemList<T> list = new ThreeItemList<T>();
                    list.Promote(_listStore);
                    list.Add(value);
                    _listStore = list;
                }
            }
            return (_listStore.Count - 1);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            if (_listStore != null)
            {
                _listStore.Clear();
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public FrugalObjectList<T> Clone()
        {
            FrugalObjectList<T> list = new FrugalObjectList<T>();
            if (_listStore != null)
            {
                list._listStore = (FrugalListBase<T>)_listStore.Clone();
            }
            return list;
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            return (((_listStore != null) && (_listStore.Count > 0)) && (_listStore.Contains(value) == true));
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(T[] array, int index)
        {
            if ((_listStore != null) && (_listStore.Count > 0))
            {
                _listStore.CopyTo(array, index);
            }
        }

        /// <summary>
        /// Ensures the index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void EnsureIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            int num = (index + 1) - Count;
            if (num > 0)
            {
                Capacity = index + 1;
                T local = default(T);
                for (int listIndex = 0; listIndex < num; listIndex++)
                {
                    _listStore.Add(local);
                }
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(T value)
        {
            if ((_listStore != null) && (_listStore.Count > 0))
            {
                return _listStore.IndexOf(value);
            }
            return -1;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, T value)
        {
            if ((index != 0) && (((_listStore == null) || (index > _listStore.Count)) || (index < 0)))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            int num = 1;
            if ((_listStore != null) && (_listStore.Count == _listStore.Capacity))
            {
                num = Capacity + 1;
            }
            Capacity = num;
            _listStore.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool Remove(T value)
        {
            return (((_listStore != null) && (_listStore.Count > 0)) && (_listStore.Remove(value) == true));
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            if (((_listStore == null) || (index >= _listStore.Count)) || (index < 0))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            _listStore.RemoveAt(index);
        }

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            if ((_listStore != null) && (_listStore.Count > 0))
            {
                return _listStore.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int Capacity
        {
            get
            {
                if (_listStore != null)
                {
                    return _listStore.Capacity;
                }
                return 0;
            }
            set
            {
                int capacity = 0;
                if (_listStore != null)
                {
                    capacity = _listStore.Capacity;
                }
                if (capacity < value)
                {
                    FrugalListBase<T> base2;
                    if (value == 1)
                    {
                        base2 = new SingleItemList<T>();
                    }
                    else if (value <= 3)
                    {
                        base2 = new ThreeItemList<T>();
                    }
                    else if (value <= 6)
                    {
                        base2 = new SixItemList<T>();
                    }
                    else
                    {
                        base2 = new ArrayItemList<T>(value);
                    }
                    if (_listStore != null)
                    {
                        base2.Promote(_listStore);
                    }
                    _listStore = base2;
                }
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                if (_listStore != null)
                {
                    return _listStore.Count;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get
            {
                if (((_listStore == null) || (index >= _listStore.Count)) || (index < 0))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _listStore.EntryAt(index);
            }
            set
            {
                if (((_listStore == null) || (index >= _listStore.Count)) || (index < 0))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                _listStore.SetAt(index, value);
            }
        }
    }
}