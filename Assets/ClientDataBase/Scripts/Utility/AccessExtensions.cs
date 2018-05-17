using System;
using System.Reflection;

public static class AccessExtensions
{
    public static T GetFieldValue<T>(this object o, string name)
    {
        var value = o.GetFieldValue(name);
        if (value != null)
        {
            return (T)value;
        }

        return default(T);
    }

    public static object GetFieldValue(this object o, string name)
    {
        var field = o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            return field.GetValue(o);
        }

        return null;
    }

    public static void SetFieldValue(this object o, string name, object value)
    {
        var field = o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(o, value);
        }
    }

    public static T GetPropertyValue<T>(this object o, string name)
    {
        var value = o.GetPropertyValue(name);
        if (value != null)
        {
            return (T)value;
        }

        return default(T);
    }

    public static object GetPropertyValue(this object o, string name)
    {
        var property = o.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
        {
            return property.GetValue(o, null);
        }

        return null;
    }

    public static void SetPropertyValue(this object o, string name, object value)
    {
        var property = o.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
        {
            property.SetValue(o, value, null);
        }
    }
}