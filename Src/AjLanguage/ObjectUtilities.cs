namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    public class ObjectUtilities
    {
        public static void SetValue(object obj, string name, object value)
        {
            if (obj is DynamicObject)
            {
                ((DynamicObject)obj).SetValue(name, value);

                return;
            }

            Type type = obj.GetType();

            type.InvokeMember(name, System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, obj, new object[] { value });
        }

        public static object GetValue(object obj, string name)
        {
            if (obj is DynamicObject)
                return ((DynamicObject)obj).GetValue(name);

            Type type = obj.GetType();

            return type.InvokeMember(name, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance, null, obj, null);
        }

        public static object GetValue(object obj, string name, object[] parameters)
        {
            if (obj is DynamicObject)
            {
                if (parameters == null)
                    return ((DynamicObject)obj).GetValue(name);

                return ((DynamicObject)obj).Invoke(name, parameters);
            }

            Type type = obj.GetType();

            return type.InvokeMember(name, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance, null, obj, parameters);
        }

        public static bool IsNumber(object obj)
        {
            return obj is int ||
                obj is short ||
                obj is long ||
                obj is decimal ||
                obj is double ||
                obj is float ||
                obj is byte;
        }
    }
}
