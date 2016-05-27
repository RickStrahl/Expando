using System.Collections;
using System.Collections.Generic;


namespace Westwind.Utilities.Dynamic
{
    public static class ExpandoExtensions
    {
        /// <summary>
        /// Converts an <see cref="IDictionary&lt;string, object&gt;"/> into an <see cref="Expando"/>
        /// </summary>
        /// <returns><see cref="Expando"/></returns>
        public static Expando ToIndexableExpando(this IDictionary<string, object> @this)
        {
            var expando = new Westwind.Utilities.Dynamic.Expando();

            foreach (var kvp in @this)
            {
                var kvpValue = kvp.Value as IDictionary<string, object>;
                if (kvpValue != null)
                {
                    var expandoVal = kvpValue.ToIndexableExpando();
                    expando[kvp.Key] = expandoVal;
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any string-object dictionaries
                    // along the way into expando objects
                    var objList = new List<object>();
                    foreach (var item in (ICollection) kvp.Value)
                    {
                        var itemVals = item as IDictionary<string, object>;
                        if (itemVals != null)
                        {
                            var expandoItem = itemVals.ToIndexableExpando();
                            objList.Add(expandoItem);
                        }
                        else
                        {
                            objList.Add(item);
                        }
                    }
                    expando[kvp.Key] = objList;
                }
                else
                {
                    expando[kvp.Key] = kvp.Value;
                }
            }
            return expando;
        }
    }
}