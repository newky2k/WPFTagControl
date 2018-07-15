using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WPFTagControl
{
    public static class ObjectExtensions
    {
        public static T GetMemberValue<T>(this object target, string memberName)
        {
            object objValue = null;


            if (string.IsNullOrWhiteSpace(memberName))
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
