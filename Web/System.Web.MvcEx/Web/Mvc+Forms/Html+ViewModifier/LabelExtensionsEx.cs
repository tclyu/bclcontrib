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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace System.Web.Mvc.Html
{
    public static class LabelExtensionsEx
    {
        public static MvcHtmlString LabelEx(this HtmlHelper htmlHelper, string expression) { return LabelHelperEx(htmlHelper, ModelMetadata.FromStringExpression(expression, htmlHelper.ViewData), expression); }
        public static MvcHtmlString LabelForEx<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) { return LabelHelperEx(htmlHelper, ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData), ExpressionHelper.GetExpressionText(expression)); }
        public static MvcHtmlString LabelForModelEx(this HtmlHelper htmlHelper) { return LabelHelperEx(htmlHelper, htmlHelper.ViewData.ModelMetadata, string.Empty); }
        internal static MvcHtmlString LabelHelperEx(HtmlHelper htmlHelper, ModelMetadata metadata, string htmlFieldName)
        {
            string text = (metadata.DisplayName ?? (metadata.PropertyName ?? htmlFieldName.Split(new char[] { '.' }).Last<string>()));
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Empty;
            var templateInfo = htmlHelper.ViewContext.ViewData.TemplateInfo;
            var fullFieldName = templateInfo.GetFullHtmlFieldName(htmlFieldName);
            var labelTag = new TagBuilder("label");
            ModelState state;
            if ((!string.IsNullOrEmpty(fullFieldName)) && (htmlHelper.ViewData.ModelState.TryGetValue(fullFieldName, out state)) && (state.Errors.Count > 0))
                labelTag.AddCssClass(HtmlHelperExtensions.ValidationLabelCssClassName);
            labelTag.Attributes.Add("for", CreateSanitizedId(templateInfo.GetFullHtmlFieldId(htmlFieldName), HtmlHelper.IdAttributeDotReplacement));
            labelTag.SetInnerText(text);
            return labelTag.ToMvcHtmlString(TagRenderMode.Normal);
        }

        #region from tagbuilder
        private static class Html401IdUtil
        {
            private static bool IsAllowableSpecialCharacter(char c)
            {
                char ch = c;
                return !(((ch != '-') && (ch != ':')) && (ch != '_'));
            }

            private static bool IsDigit(char c)
            {
                return (('0' <= c) && (c <= '9'));
            }

            public static bool IsLetter(char c)
            {
                return ((('A' <= c) && (c <= 'Z')) || (('a' <= c) && (c <= 'z')));
            }

            public static bool IsValidIdCharacter(char c)
            {
                return (!IsLetter(c) && !IsDigit(c) ? IsAllowableSpecialCharacter(c) : true);
            }
        }

        internal static string CreateSanitizedId(string originalId, string dotReplacement)
        {
            if (string.IsNullOrEmpty(originalId))
                return null;
            char c = originalId[0];
            if (!Html401IdUtil.IsLetter(c))
                return null;
            var b = new StringBuilder(originalId.Length);
            b.Append(c);
            for (int i = 1; i < originalId.Length; i++)
            {
                char ch2 = originalId[i];
                if (Html401IdUtil.IsValidIdCharacter(ch2))
                    b.Append(ch2);
                else
                    b.Append(dotReplacement);
            }
            return b.ToString();
        }
        #endregion
    }
}
