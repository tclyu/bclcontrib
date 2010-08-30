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
using System.Globalization;
namespace System.Primitives.DataTypes
{
    /// <summary>
    /// RealDataType
    /// </summary>
	public class RealDataType : DataTypeBase
	{
		public class FormatAttrib
		{
			public Formats Formats;
			public string Format;
		}

		public enum Formats
		{
			Pattern,
		}

		public class ParseAttrib
		{
			public double? MaxValue { get; set; }
			public double? MinValue { get; set; }
			public int? Precision { get; set; }
			public int? Round { get; set; }
		}

		public RealDataType()
			: base(Prime.Type, Prime.FormFieldMeta, new DataTypeFormatter(), new DataTypeParser()) { }

		public class DataTypeFormatter : DataTypeFormatterBase<double, FormatAttrib, ParseAttrib>
		{
			public DataTypeFormatter()
				: base(Prime.Format, Prime.TryParse) { }
		}

		public class DataTypeParser : DataTypeParserBase<double, ParseAttrib>
		{
			public DataTypeParser()
				: base(Prime.TryParse, 0D, "0") { }
		}

		/// <summary>
		/// 
		/// </summary>
		public static class Prime
		{
			public static string Format(double value, FormatAttrib attrib)
			{
				if (attrib != null)
					switch (attrib.Formats)
					{
						case Formats.Pattern:
							return value.ToString(attrib.Format, CultureInfo.InvariantCulture);
						default:
							throw new InvalidOperationException();
					}
				return value.ToString("0.00", CultureInfo.InvariantCulture);
			}

			public static bool TryParse(string text, ParseAttrib attrib, out double value)
			{
				if (string.IsNullOrEmpty(text))
				{
					value = 0D;
					return false;
				}
				if (!double.TryParse(text, out value))
					return false;
				// check attrib
				if (attrib != null)
				{
					double? minValue = attrib.MinValue;
					if ((minValue != null) && (value < minValue))
						return false;
					double? maxValue = attrib.MaxValue;
					if ((maxValue != null) && (value > maxValue))
						return false;
					int? precision = attrib.Precision;
					if ((precision != null) && (value != Math.Round(value, (int)precision)))
						return false;
					int? round = attrib.Round;
					if (round != null)
						value = Math.Round(value, (int)round);
				}
				return true;
			}

			public static Type Type
			{
				get { return typeof(double); }
			}

			public static DataTypeFormFieldMeta FormFieldMeta
			{
				get
				{
					return new DataTypeFormFieldMeta()
					{
						GetBinderType = (int applicationType) => "Text",
						GetMaxLength = (int applicationType) => 15,
						GetSize = (int applicationType, int length) => "10",
					};
				}
			}
		}
	}
}