using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Lang.Reflect;
 

namespace Calligraphy
{
    public static class ReflectionUtils
    {
        private static string TAG = typeof (ReflectionUtils).FullName;

        public static FieldInfo getFieldInfo(Type clazz, string FieldInfoName)
        {
            try
            {
                 var f = clazz.GetField(FieldInfoName, BindingFlags.NonPublic|BindingFlags.Public);
                
                return f;
            }
            catch (FieldAccessException ignored)
            {
            }
            return null;
        }

        public static object getValue(FieldInfo FieldInfo, object obj)
        {
            try
            {
                return FieldInfo.GetValue(obj);
            }
            catch (IllegalAccessException ignored)
            {
            }
            return null;
        }

        public static void setValue(FieldInfo FieldInfo, object obj, object value)
        {
            try
            {
                FieldInfo.SetValue(obj, value);
            }
            catch (IllegalAccessException ignored)
            {
            }
        }

        public static MethodInfo getMethod(Type clazz, string methodName)
        {
            var methods = clazz.GetMethods(BindingFlags.Public|BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
            {
                if (methodInfo.Name.Equals(methodName))
                {
                    return methodInfo;
                }
            }
            
            return null;
        }

        public static void invokeMethod(object obj, MethodInfo methodInfo, object[] args)
        {
            try
            {
                if (methodInfo == null) return;
                    
                methodInfo.Invoke(obj, args);
            }
            catch (IllegalAccessException  e)
            {
                Log.Debug(TAG, "Can't invoke method using reflection", e);

            }
            catch (InvocationTargetException e) {
                Log.Debug(TAG, "Can't invoke method using reflection", e);
            }
            }
        }
}