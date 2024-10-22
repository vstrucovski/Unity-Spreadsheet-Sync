using System;
using NaughtyAttributes;
using UnityEngine;
using UnitySpreadsheetSync.Scripts;

namespace UnitySpreadsheetSync.Examples
{
    [CreateAssetMenu(menuName = "Data/TestingAllTypes")]
    public class AllTypesSO : ScriptableObject, IDataBindable
    {
        [field: SerializeField] public string id { get; set; }
        public int type_int;
        public float type_float;
        public CustomEnum type_enum;
        public int type_example_time;
        public string type_str;

        public void Parse(SpreadsheetLine spreadsheetData)
        {
            DataBinder.Bind(this, spreadsheetData);
            var timeStr = spreadsheetData["type_example_time"];
            if (timeStr.Contains(":"))
            {
                type_example_time = ParseTimeToSeconds(timeStr);
            }
        }

        private static int ParseTimeToSeconds(string time)
        {
            string[] timeParts = time.Split(':');
            int minutes = int.Parse(timeParts[0]);
            int seconds = int.Parse(timeParts[1]);

            return minutes * 60 + seconds;
        }

        [Button]
        public void Reset()
        {
            type_int = -1;
            type_float = -1;
            type_enum = 0;
            type_example_time = -1;
            type_str = "-1";
        }
    }
}