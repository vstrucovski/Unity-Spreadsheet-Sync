using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace UnitySpreadsheetSync.Scripts
{
    public static class DataBinder
    {
        private const NumberStyles numberStyle = NumberStyles.Any;

        public static void Bind(object target, Dictionary<string, string> data)
        {
            var type = target.GetType();
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            foreach (var key in data.Keys)
            {
                var property = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    ParseProperty(target, data, key, property, cultureInfo);
                }
                else
                {
                    var field = type.GetField(key, BindingFlags.Public | BindingFlags.Instance);
                    if (field != null)
                    {
                        ParseField(target, data, key, field, cultureInfo);
                    }
                    else
                    {
                        Debug.LogError($"CSV binding cannot find field/property = {key} in target class = " + type);
                    }
                }
            }
#if UNITY_EDITOR
            if (target is ScriptableObject scriptableObject)
            {
                EditorUtility.SetDirty(scriptableObject);
                AssetDatabase.SaveAssets();
            }
#endif
        }

        private static void ParseField(object target, IReadOnlyDictionary<string, string> data, string key,
            FieldInfo field, CultureInfo cultureInfo)
        {
            var value = data[key];
            if (string.IsNullOrEmpty(value)) value = "0";

            if (field.FieldType == typeof(int))
            {
                if (int.TryParse(value, numberStyle, cultureInfo, out var parsed))
                {
                    field.SetValue(target, parsed);
                }
            }
            else if (field.FieldType == typeof(float))
            {
                if (float.TryParse(value, numberStyle, cultureInfo, out var parsed))
                {
                    field.SetValue(target, parsed);
                }
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(target, value);
            }
            else if (field.FieldType.IsEnum)
            {
                if (Enum.TryParse(field.FieldType, value, out var parsed))
                {
                    field.SetValue(target, parsed);
                }
            }
        }

        private static void ParseProperty(object target, IReadOnlyDictionary<string, string> data, string key,
            PropertyInfo property, CultureInfo cultureInfo)
        {
            var value = data[key];
            if (string.IsNullOrEmpty(value)) value = "0";

            if (property.PropertyType == typeof(int))
            {
                if (int.TryParse(value, numberStyle, cultureInfo, out var parsed))
                {
                    property.SetValue(target, parsed);
                }
            }
            else if (property.PropertyType == typeof(float))
            {
                if (float.TryParse(value, numberStyle, cultureInfo, out var parsed))
                {
                    property.SetValue(target, parsed);
                }
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(target, value);
            }
            else if (property.PropertyType.IsEnum)
            {
                if (Enum.TryParse(property.PropertyType, value, out var parsed))
                {
                    property.SetValue(target, parsed);
                }
            }
        }
    }
}