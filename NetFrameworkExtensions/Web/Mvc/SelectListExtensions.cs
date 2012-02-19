using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NetFrameworkExtensions.Web.Mvc
{
    /// <summary>
    /// Methods to turn any Enumerable lists into Mvc SelectList objects
    /// </summary>
    public static partial class SelectListExtensions
    {
        public static SelectList ToSelectList<T, TKey, TValue>(this IEnumerable<T> values, 
                                Func<T, TKey> keySelector, Func<T, TValue> valueSelector, TKey selectedValue = default(TKey))
        {
            SelectList result = null;

            if (selectedValue == null)
            {
                result = new SelectList(values.ToDictionary(keySelector, valueSelector), "Key", "Value");
            }
            else
            {
                result = new SelectList(values.ToDictionary(keySelector, valueSelector), "Key", "Value", selectedValue);
            }

            return result;
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> values, T selectedValue = default(T))
        {
            return new SelectList(values.ToDictionary(key => key, value => value), "Key", "Value", selectedValue);
        }

        public static SelectList ToSelectList<TKey, TValue>(this IDictionary<TKey, TValue> values, System.Nullable<TKey> selectedValue = null) where TKey : struct
        {
            SelectList result = null;

            if (selectedValue == null)
            {
                result = new SelectList(values, "Key", "Value");
            }
            else
            {
                result = new SelectList(values, "Key", "Value", selectedValue);
            }

            return result;
        }

        public static SelectList ToSelectList<TKey, TValue>(this IDictionary<TKey, TValue> values, TKey selectedValue = default(TKey))
        {
            SelectList result = null;

            if (selectedValue == null)
            {
                result = new SelectList(values, "Key", "Value");
            }
            else
            {
                result = new SelectList(values, "Key", "Value", selectedValue);
            }

            return result;
        }

        public static SelectList Insert(this SelectList values, int index, string key, string value, bool selected = false)
        {
            List<SelectListItem> result = new List<SelectListItem>(values);

            result.Insert(index, new SelectListItem()
            {
                Value = key,
                Text = value,
                Selected = selected
            });

            return new SelectList(result, "Value", "Text", (selected ? key : values.SelectedValue));
        }

        public static SelectList Add(this SelectList values, string key, string value, bool selected = false)
        {
            List<SelectListItem> result = new List<SelectListItem>(values);

            result.Add(new SelectListItem()
            {
                Value = key,
                Text = value,
                Selected = selected
            });

            return new SelectList(result, "Value", "Text", (selected ? key : values.SelectedValue));
        }
    }
}
