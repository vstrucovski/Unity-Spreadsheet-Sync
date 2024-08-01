using System.IO;
using System.Linq;
using UnityEngine;

namespace UnitySpreadsheetSync.Scripts
{
    public static class CsvParser
    {
        public static SpreadsheetPage ParseCsv(string csvContent)
        {
            var reader = new StringReader(csvContent);
            var spreadsheet = new SpreadsheetPage();
            var isEmpty = reader.Peek() == -1;
            if (isEmpty) return spreadsheet;

            var keysTxt = reader.ReadLine();
            if (string.IsNullOrEmpty(keysTxt))
            {
                Debug.LogError("Missed csv keys");
                return null;
            }

            var keys = keysTxt.Split(',').ToList();

            while (reader.Peek() != -1)
            {
                var line = new SpreadsheetLine();
                var lineStr = reader.ReadLine();
                var fields = lineStr.Split(',').ToList();
                if (fields.Count > 0)
                {
                    for (var index = 0; index < fields.Count; index++)
                    {
                        var field = fields[index];
                        if (index == 0)
                        {
                            spreadsheet.Add(field, line);
                        }
                        else
                        {
                            line[keys[index]] = field;
                        }
                    }
                }
            }

            return spreadsheet;
        }
    }
}