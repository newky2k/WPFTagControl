using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WPFTagControl
{
    /// <summary>
    /// Extension for any object
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
        public static T GetMemberValue<T>(this object target, string memberName)
        {
            object objValue = null;


            if (!string.IsNullOrWhiteSpace(memberName))
            {
                var typ = target.GetType().GetTypeInfo();

                var aMem = typ.GetProperty(memberName);



                if (aMem != null)
                {
                    objValue = aMem.GetValue(target);
                }
            }

            if (objValue == null)
                objValue = target.ToString();

            return (T)Convert.ChangeType(objValue, typeof(T));

        }
    }
}
