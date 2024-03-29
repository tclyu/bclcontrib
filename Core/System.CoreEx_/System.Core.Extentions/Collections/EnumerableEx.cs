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
namespace System.Collections
{
    public static class EnumerableEx
    {
        public static bool IsNullOrEmptyArray(Array values)
        {
            return ((values == null) || (values.Length == 0));
        }

        public static bool IsNullOrEmptyEnumerable(IEnumerable values)
        {
            return ((values == null) || (!values.GetEnumerator().MoveNext()));
        }

        public static bool IsNullOrEmpty<TSource>(ICollection<TSource> values)
        {
            return ((values == null) || (values.Count == 0));
        }

        public static bool IsNullOrEmpty<TSource>(IEnumerable<TSource> values)
        {
            if (values != null)
                using (var enumerator = values.GetEnumerator())
                    return !enumerator.MoveNext();
            return true;
        }

        public static ICollection<TSource> NullIf<TSource>(ICollection<TSource> values) { return NullIf<TSource>(values, IsNullOrEmpty<TSource>); }
        public static ICollection<TSource> NullIf<TSource>(ICollection<TSource> values, Predicate<ICollection<TSource>> condition)
        {
            return ((values != null) && (!condition(values)) ? values : null);
        }

        public static IEnumerable<TSource> NullIf<TSource>(IEnumerable<TSource> values) { return NullIf<TSource>(values, IsNullOrEmpty<TSource>); }
        public static IEnumerable<TSource> NullIf<TSource>(IEnumerable<TSource> values, Predicate<IEnumerable<TSource>> condition)
        {
            return ((values != null) && (!condition(values)) ? values : null);
        }
    }
}