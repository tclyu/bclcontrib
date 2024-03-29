#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System.Collections.Generic;
using System.Collections;
namespace System.Linq
{
    public static partial class EnumerableExtensions
    {
        private class WhereSelectArrayIterator<TSource, TResult> : Iterator<TResult>
        {
            private int _index;
            private Func<TSource, bool> _predicate;
            private Func<TSource, TResult> _selector;
            private TSource[] _source;

            public WhereSelectArrayIterator(TSource[] source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
            {
                _source = source;
                _predicate = predicate;
                _selector = selector;
            }

            public override Iterator<TResult> Clone()
            {
                return new WhereSelectArrayIterator<TSource, TResult>(_source, _predicate, _selector);
            }

            public override bool MoveNext()
            {
                if (_state == 1)
                {
                    while (_index < _source.Length)
                    {
                        TSource arg = _source[_index];
                        _index++;
                        if ((_predicate == null) || _predicate(arg))
                        {
                            _current = _selector(arg);
                            return true;
                        }
                    }
                    Dispose();
                }
                return false;
            }

            public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
            {
                return new WhereSelectArrayIterator<TSource, TResult2>(_source, _predicate, CombineSelectors<TSource, TResult, TResult2>(_selector, selector));
            }

            public override IEnumerable<TResult> Where(Func<TResult, bool> predicate)
            {
                return (IEnumerable<TResult>)new WhereEnumerableIterator<TResult>(this, predicate);
            }
        }
    }
}
