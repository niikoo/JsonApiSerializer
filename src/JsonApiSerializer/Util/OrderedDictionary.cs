using System.Linq;

namespace JsonApiSerializer.Util
{
    // The MIT License (MIT)

    // Copyright (c) 2013 Clinton Brennan, 2018 Nikolai Ommundsen <post@niikoo.net>

    // Permission is hereby granted, free of charge, to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:
     
    // The above copyright notice and this permission notice shall be included in
    // all copies or substantial portions of the Software.

    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.
    using System;
    using System.Collections.Generic;
    using System.Collections;

    namespace JsonApiConverter.Util
    {
        public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
        {
            private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _mDictionary;
            private readonly LinkedList<KeyValuePair<TKey, TValue>> _mLinkedList;

            private readonly ValueCollection _valueCollection;
            private readonly KeyCollection _keyCollection;

            #region Constructors
            public OrderedDictionary()
            {
                _mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
                _mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
                _valueCollection = new ValueCollection(this);
                _keyCollection = new KeyCollection(this);
            }

            public OrderedDictionary(int capacity)
            {
                _mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity);
                _mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
                _valueCollection = new ValueCollection(this);
                _keyCollection = new KeyCollection(this);
            }

            public OrderedDictionary(IEqualityComparer<TKey> comparer)
            {
                _mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(comparer);
                _mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
                _valueCollection = new ValueCollection(this);
                _keyCollection = new KeyCollection(this);
            }
            #endregion Constructors

            public void Add(TKey key, TValue value)
            {
                var lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
                _mDictionary.Add(key, lln);
                _mLinkedList.AddLast(lln);
            }

            #region IDictionary Generic
            public bool ContainsKey(TKey key) => _mDictionary.ContainsKey(key);

            public ICollection<TKey> Keys => _keyCollection;

            public bool Remove(TKey key)
            {
                var found = _mDictionary.TryGetValue(key, out var lln);
                if (!found) { return false; }
                _mDictionary.Remove(key);
                _mLinkedList.Remove(lln);
                return true;
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                var found = _mDictionary.TryGetValue(key, out var lln);
                if (!found)
                {
                    value = default(TValue);
                    return false;
                }
                value = lln.Value.Value;
                return true;
            }

            public ICollection<TValue> Values => _valueCollection;

            public TValue this[TKey key]
            {
                get => _mDictionary[key].Value.Value;
                set
                {
                    LinkedListNode<KeyValuePair<TKey, TValue>> lln;
                    if (_mDictionary.ContainsKey(key))
                    {
                        lln = _mDictionary[key];
                        lln.Value = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    else
                    {
                        lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
                        _mLinkedList.AddLast(lln);
                        _mDictionary.Add(key, lln);
                    }
                }
            }

            public void Clear()
            {
                _mDictionary.Clear();
                _mLinkedList.Clear();
            }

            public int Count => _mLinkedList.Count;

            public bool IsReadOnly => throw new NotImplementedException();

            public IEnumerable<KeyValuePair<TKey, TValue>> MutateFriendlyEnumerable()
            {
                var current = _mLinkedList.First;
                while(current != null)
                {
                    yield return current.Value;
                    current = current.Next;
                }
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => MutateFriendlyEnumerable().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#endregion IDictionary Generic

#region Explicit ICollection Generic
            void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            {
                var lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(item);
                _mDictionary.Add(item.Key, lln);
                _mLinkedList.AddLast(lln);
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => _mDictionary.ContainsKey(item.Key);

            void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _mLinkedList.CopyTo(array, arrayIndex);

            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

            bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            {
                return Remove(item.Key);
            }
#endregion Explicit ICollection Generic

            public sealed class KeyCollection : ICollection<TKey>
            {
                readonly OrderedDictionary<TKey, TValue> _parent;

                internal KeyCollection(OrderedDictionary<TKey, TValue> parent)
                {
                    _parent = parent;
                }

                public int Count => _parent.Count;

                public bool IsReadOnly => true;

                public void CopyTo(TKey[] array, int arrayIndex)
                {
                    _parent._mLinkedList.Select(x=>x.Key).ToList().CopyTo(array, arrayIndex);
                }

                public void Add(TKey item) => throw new NotImplementedException();

                public void Clear() => throw new NotImplementedException();

                public bool Contains(TKey item) => item != null && _parent.ContainsKey(item);

                public bool Remove(TKey item) => throw new NotImplementedException();

                IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TKey>)this).GetEnumerator();

                IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => _parent._mLinkedList.Select(x=>x.Key).GetEnumerator();
            }

            public sealed class ValueCollection : ICollection<TValue>
            {
                public readonly OrderedDictionary<TKey, TValue> Parent;

                internal ValueCollection(OrderedDictionary<TKey, TValue> parent) => Parent = parent;

                public int Count => Parent.Count;

                public bool IsReadOnly => true;

                public void CopyTo(TValue[] array, int arrayIndex) => Parent._mLinkedList.Select(x => x.Value).ToList().CopyTo(array, arrayIndex);

                public void Add(TValue item) => throw new NotImplementedException();

                public void Clear() => throw new NotImplementedException();

                public bool Contains(TValue item) => Parent._mLinkedList.Any(x => x.Equals(item));

                public bool Remove(TValue item) => throw new NotImplementedException();

                IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TValue>)this).GetEnumerator();

                IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => Parent._mLinkedList.Select(x => x.Value).GetEnumerator();
            }
        }
    }
}
