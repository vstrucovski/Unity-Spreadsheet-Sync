﻿using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
                    if (string.IsNullOrEmpty(value)) value = "0";

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
                        if (string.IsNullOrEmpty(value)) value = "0";

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
#if UNITY_EDITOR
            if (target is ScriptableObject)
            {
                EditorUtility.SetDirty((ScriptableObject) target);
                AssetDatabase.SaveAssets();
            }
#endif
        }

        private static float ParseFloat(string value)
        {
            float result = -1;
            if (!float.TryParse(value, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.GetCultureInfo("en-US"), out result))
            {
                Debug.Log("Can't pars float, wrong text");
            }

            return result;
        }
    }
}