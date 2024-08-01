using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnitySpreadsheetSync.Scripts
{
    public class SpreadsheetSyncer : MonoBehaviour
    {
        [SerializeField] private string sheetId;
        [SerializeField] private bool autoStart;
        [SerializeField] private List<ScriptableObject> dataToUpdate;
        [SerializeField] private bool debug;

        private const string UrlTemplate = "https://docs.google.com/spreadsheets/d/e/{0}/pub?output=csv";
        private IDataDownloader _downloader;

        private void Start()
        {
            Init();
            if (autoStart)
            {
                Download();
            }
        }

        private void Init()
        {
            _downloader ??= new CsvDataDownloader(); //TODO rework to SO with diff impl
        }

        public string GetUrl() => string.Format(UrlTemplate, sheetId);

        [Button]
        public void Download()
        {
            Log("Downloading...");
            Init();
            StartCoroutine(_downloader.Download(GetUrl(),
                content =>
                {
                    Log("Success downloaded CSV data");
                    var page = CsvParser.ParseCsv(content);
                    Log("Parsed CSV data into page = " + page.Count);
                    FillContent(page);
                }, error => { Debug.LogError("Error downloading CSV: " + error); }));
        }

        private void FillContent(SpreadsheetPage page)
        {
            var flattenList = new List<ScriptableObject>();
            foreach (var item in dataToUpdate)
            {
                if (item is ScriptableObjectGroup group)
                {
                    flattenList.AddRange(group.List);
                }
                else
                {
                    flattenList.Add(item);
                }
            }

            Log($"Found {flattenList.Count} objects to update");
            foreach (var item in flattenList)
            {
                if (item is IDataBindable csvRemote)
                {
                    var line = page[csvRemote.id];
                    if (line != null)
                    {
                        csvRemote.Parse(line);
                    }
                }
            }
        }

        private void Log(string text)
        {
            if (debug)
            {
                Debug.Log("DataSync. " + text);
            }
        }
    }
}