using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ModelUNRegister.Utilities
{
    public enum BootstrapRadioButtonStyle
    {
        BootstrapToogleButton,
        RadioButton
    }

    public static class EnumRadioButtonExtension
    {
        internal static object GetModelStateValue(this HtmlHelper helper, string key, Type destinationType)
        {
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString EnumRadioButtonFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, BootstrapRadioButtonStyle style = BootstrapRadioButtonStyle.RadioButton)
        {
            return EnumRadioButtonFor(htmlHelper, expression, htmlAttributes: null, style: style);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString EnumRadioButtonFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, object htmlAttributes, BootstrapRadioButtonStyle style = BootstrapRadioButtonStyle.RadioButton)
        {
            return EnumRadioButtonFor(htmlHelper, expression,
                htmlAttributesDictionary: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), style: style);
        }

        // Unable to constrain TEnum.  Cannot include IComparable, IConvertible, IFormattable because Nullable<T> does
        // not implement those interfaces (and Int32 does).  Enum alone is not compatible with expression restrictions
        // because that requires a cast from all enum types.  And the struct generic constraint disallows passing a
        // Nullable<T> expression.
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString EnumRadioButtonFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, IDictionary<string, object> htmlAttributesDictionary, BootstrapRadioButtonStyle style = BootstrapRadioButtonStyle.RadioButton)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (metadata == null)
            {
                throw new ArgumentException("expression", expression.ToString());
            }

            if (metadata.ModelType == null)
            {
                throw new ArgumentNullException("expression",
                    expression.ToString());
            }

            if (!EnumHelper.IsValidForEnumHelper(metadata.ModelType))
            {
                throw new ArgumentException("expression");
            }

            // Run through same processing as SelectInternal() to determine selected value and ensure it is included
            // in the select list.
            string expressionName = ExpressionHelper.GetExpressionText(expression);
            string expressionFullName =
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionName);
            Enum currentValue = null;
            if (!String.IsNullOrEmpty(expressionFullName))
            {
                currentValue = htmlHelper.GetModelStateValue(expressionFullName, metadata.ModelType) as Enum;
            }

            if (currentValue == null && !String.IsNullOrEmpty(expressionName))
            {
                // Ignore any select list (enumerable with this name) in the view data
                currentValue = htmlHelper.ViewData.Eval(expressionName) as Enum;
            }

            if (currentValue == null)
            {
                currentValue = metadata.Model as Enum;
            }

            IList<SelectListItem> selectList = EnumHelper.GetSelectList(metadata.ModelType, null);

            return EnumRadioButtonHelper(htmlHelper, metadata, expressionName, expression, selectList, htmlAttributesDictionary, style);
        }

        private static MvcHtmlString EnumRadioButtonHelper<TModel, TEnum>(HtmlHelper<TModel> htmlHelper, ModelMetadata metadata, string expression, Expression<Func<TModel, TEnum>> lambdaExpression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes, BootstrapRadioButtonStyle style)
        {
            return SelectInternal(htmlHelper, metadata, expression, lambdaExpression, selectList, htmlAttributes: htmlAttributes, style: style);
        }

        // Helper methods
        private static IEnumerable<SelectListItem> GetSelectListWithDefaultValue(IEnumerable<SelectListItem> selectList, object defaultValue, bool allowMultiple)
        {
            IEnumerable defaultValues;

            if (allowMultiple)
            {
                defaultValues = defaultValue as IEnumerable;
                if (defaultValues == null || defaultValues is string)
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                defaultValues = new[] { defaultValue };
            }

            IEnumerable<string> values = from object value in defaultValues
                                         select Convert.ToString(value, CultureInfo.CurrentCulture);

            // ToString() by default returns an enum value's name.  But selectList may use numeric values.
            IEnumerable<string> enumValues = from Enum value in defaultValues.OfType<Enum>()
                                             select value.ToString("d");
            values = values.Concat(enumValues);

            HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
            List<SelectListItem> newSelectList = new List<SelectListItem>();

            foreach (SelectListItem item in selectList)
            {
                item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                newSelectList.Add(item);
            }
            return newSelectList;
        }

        private static string BootstrapToggleButtonBuilder<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> lambdaExpression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            StringBuilder listItemBuilder = new StringBuilder();

            foreach (SelectListItem item in selectList)
            {
                // Empty is for null compatiblility
                if (item.Text != String.Empty && item.Value != String.Empty)
                {
                    TagBuilder builder = new TagBuilder("label");
                    builder.MergeAttributes(htmlAttributes);
                    builder.AddCssClass("btn");
                    if (item.Selected)
                    {
                        builder.AddCssClass("active");
                    }

                    builder.InnerHtml = htmlHelper.RadioButtonFor(lambdaExpression, item.Value).ToHtmlString()
                        + HttpUtility.HtmlEncode(item.Text);

                    listItemBuilder.AppendLine(builder.ToString(TagRenderMode.Normal));
                }
            }

            TagBuilder tagBuilder = new TagBuilder("div")
            {
                InnerHtml = listItemBuilder.ToString()
            };

            tagBuilder.MergeAttribute("class", "btn-group");
            tagBuilder.MergeAttribute("data-toggle", "buttons");

            return tagBuilder.ToString(TagRenderMode.Normal);
        }

        private static string RadioButtonBuilder<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> lambdaExpression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            StringBuilder listItemBuilder = new StringBuilder();

            foreach (SelectListItem item in selectList)
            {
                // Empty is for null compatiblility
                if (item.Text != String.Empty && item.Value != String.Empty)
                {
                    TagBuilder labelbuilder = new TagBuilder("label");

                    labelbuilder.InnerHtml = htmlHelper.RadioButtonFor(lambdaExpression, item.Value).ToHtmlString()
                                    + HttpUtility.HtmlEncode(item.Text);

                    TagBuilder divBuilder = new TagBuilder("div")
                    {
                        InnerHtml = labelbuilder.ToString()
                    };

                    divBuilder.AddCssClass("radio");
                    divBuilder.MergeAttributes(htmlAttributes);

                    listItemBuilder.AppendLine(divBuilder.ToString(TagRenderMode.Normal));
                }
            }
            return listItemBuilder.ToString();
        }

        private static MvcHtmlString SelectInternal<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, ModelMetadata metadata,
            string name, Expression<Func<TModel, TEnum>> lambdaExpression,
            IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes, BootstrapRadioButtonStyle style)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentNullException("name");
            }

            object defaultValue = htmlHelper.GetModelStateValue(fullName, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData - supplied value before using the parameter-supplied value.
            if (defaultValue == null && !String.IsNullOrEmpty(name))
            {
                if (metadata != null)
                {
                    defaultValue = metadata.Model;
                }
            }

            if (defaultValue != null)
            {
                selectList = GetSelectListWithDefaultValue(selectList, defaultValue, false);
            }

            string content;
            switch (style)
            {
                case BootstrapRadioButtonStyle.BootstrapToogleButton:
                    content = BootstrapToggleButtonBuilder(htmlHelper, lambdaExpression, selectList, htmlAttributes);
                    break;
                case BootstrapRadioButtonStyle.RadioButton:
                    content = RadioButtonBuilder(htmlHelper, lambdaExpression, selectList, htmlAttributes);
                    break;
                default:
                    content = "";
                    break;
            }



            return new MvcHtmlString(content);
        }
    }
}