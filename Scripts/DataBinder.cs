using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DefaultNamespace
{
    public class DataBinder
    {
        public static void Bind(object target, Dictionary<string, string> data)
        {
            var type = target.GetType();
            foreach (var key in data.Keys)
            {
                var property = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    var value = data[key];
                    if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(target, int.Parse(value));
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        property.SetValue(target, float.Parse(value));
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(target, value);
                    }
                }
                else
                {
                    var field = type.GetField(key, BindingFlags.Public | BindingFlags.Instance);
                    if (field != null)
                    {
                        var value = data[key];
                        if (field.FieldType == typeof(int))
                        {
                            field.SetValue(target, int.Parse(value));
                        }
                        else if (field.FieldType == typeof(float))
                        {
                            field.SetValue(target, float.Parse(value));
                        }
                        else if (field.FieldType == typeof(string))
                        {
                            field.SetValue(target, value);
                        }
                    }
                    else
                    {
                        Debug.LogError($"CSV binding cannot find field/property = {key} in target class = " + type);
                    }
                }
            }
        }
    }
}