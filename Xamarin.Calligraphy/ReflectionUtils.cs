using System;
using System.Linq;
using System.Reflection;
using Android.Util;
using Java.Lang;
using Java.Lang.Reflect;
 

namespace Calligraphy
{
    public static class ReflectionUtils
    {
        private static readonly string Tag = typeof (ReflectionUtils).FullName;

        public static FieldInfo GetFieldInfo(Type clazz, string fieldInfoName)
        {
            try
            {
                 var f = clazz.GetField(fieldInfoName, BindingFlags.NonPublic|BindingFlags.Public);
                
                return f;
            }
            catch (FieldAccessException ignored)
            {
                Log.Debug("ReflectionUtils - Ignoring field not found", ignored.Message);
            }
            return null;
        }

        public static object GetValue(FieldInfo fieldInfo, object obj)
        {
            try
            {
                return fieldInfo.GetValue(obj);
            }
            catch (IllegalAccessException ignored)
            {
                Log.Debug("ReflectionUtils - Ignoring field not found", ignored.Message);
            }
            return null;
        }

        public static void SetValue(FieldInfo fieldInfo, object obj, object value)
        {
            try
            {
                fieldInfo.SetValue(obj, value);
            }
            catch (IllegalAccessException ignored)
            {
                Log.Debug("ReflectionUtils - Ignoring field not found", ignored.Message);
            }
        }

        public static MethodInfo GetMethod(Type clazz, string methodName)
        {
            var methods = clazz.GetMethods(BindingFlags.Public|BindingFlags.NonPublic);
            return methods.FirstOrDefault(methodInfo => methodInfo.Name.Equals(methodName));
        }

        public static void InvokeMethod(object obj, MethodInfo methodInfo, object[] args)
        {
            try
            {
                if (methodInfo == null) return;
                    
                methodInfo.Invoke(obj, args);
            }
            catch (IllegalAccessException  e)
            {
                Log.Debug(Tag, "Can't invoke method using reflection", e);

            }
            catch (InvocationTargetException e) {
                Log.Debug(Tag, "Can't invoke method using reflection", e);
            }
            }
        }
}