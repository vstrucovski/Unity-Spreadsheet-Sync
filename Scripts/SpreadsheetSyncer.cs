using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnitySpreadsheetSync.Scripts
{
    public class SpreadsheetSyncer : MonoBehaviour
    {
        [SerializeField] private string sheetId;
        [SerializeField] private string pageId;
        [SerializeField] private bool autoStart;
        [SerializeField] private List<ScriptableObject> dataToUpdate;
        [SerializeField] private bool debug;

        // private const string UrlTemplate = "https://docs.google.com/spreadsheets/d/{0}/pub?output=csv";
        private const string UrlTemplate =
            "https://docs.google.com/spreadsheets/d/{0}/pub?gid={1}&single=true&output=csv";

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

        public string GetUrl() => string.Format(UrlTemplate, sheetId, pageId);

        [Button]
        public void Download()
        {
            Log("Downloading..." + GetUrl());
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

        public IEnumerator DownloadCoroutine()
        {
            Log("Downloading..." + GetUrl());
            Init();
            yield return StartCoroutine(_downloader.Download(GetUrl(),
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
                if (item is IScriptableObjectGroup group)
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
                    var key = csvRemote.id;
                    if (!page.ContainsKey(key))
                    {
                        Log("No remote data for key => " + key);
                        continue;
                    }

                    var line = page[key];
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