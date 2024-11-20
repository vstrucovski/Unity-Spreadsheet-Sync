using System.Linq;
using UnityEngine;
using UnitySpreadsheetSync.Scripts.Data;

namespace UnitySpreadsheetSync.Scripts.Parser
{
    public class SpreadSheetPageParser : MonoBehaviour, ISpreadSheetParser
    {
        [SerializeField] private ScriptableObject dataToUpdate;
        [SerializeField] private bool debug;

        public void ParseAndFill(string csvContent)
        {
            Log("Parsing CSV content...");
            var page = CsvParser.ParseCsv(csvContent);
            Log("Parsed CSV data into page count = " + page.Count);
            FillContent(page);
        }

        private void FillContent(SpreadsheetPage page)
        {
            if (dataToUpdate is IScriptableObjectTypedGroup typedGroup)
            {
                var typedList = typedGroup.List.Cast<IDataBindable>();
                var existingAssets = typedList.ToDictionary(item => item.id);
                foreach (var pageKey in page.Keys)
                {
                    var line = page[pageKey];
                    if (existingAssets.TryGetValue(pageKey, out var existingAsset))
                    {
                        Log($"Updating asset with key: {pageKey}");
                        existingAsset.Parse(line);
                    }
                    else
                    {
                        Log($"Missing asset for key: {pageKey}, creating new one.");
                        var newItem = typedGroup.CreateSubAsset();
                        if (newItem is IDataBindable bindableData)
                        {
                            bindableData.id = pageKey;
                            bindableData.Parse(line);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Wrong SO type");
            }
        }

        private void Log(string text)
        {
            if (debug)
            {
                Debug.Log("GoogleSheetParser: " + text);
            }
        }
    }
}