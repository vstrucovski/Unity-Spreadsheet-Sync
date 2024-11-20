using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnitySpreadsheetSync.Scripts.Data;
using UnitySpreadsheetSync.Scripts.Parser;

namespace UnitySpreadsheetSync.Scripts
{
    public class SpreadsheetSyncer : MonoBehaviour
    {
        [SerializeField] protected string sheetId;
        [SerializeField] protected string pageId;
        [SerializeField] protected bool autoStart;
        [SerializeField] private bool isSingleType;

        [SerializeField, HideIf("isSingleType")]
        private List<ScriptableObject> dataToUpdate;

        [SerializeField, ShowIf("isSingleType")]
        private ScriptableObject dataToUpdateGroup;

        [SerializeField] private bool debug;

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
                    Log("Parsed CSV data into page count = " + page.Count);
                    FillContent(page);
                }, error => { Debug.LogError("Error downloading CSV: " + error); }));
        }

        private void FillContent(SpreadsheetPage page)
        {
            if (isSingleType)
            {
                FillContentSingleType(page);
            }
            else
            {
                FillContentDifferentTypes(page);
            }
        }

        private void FillContentDifferentTypes(SpreadsheetPage page)
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

            foreach (var pageKey in page.Keys)
            {
                //TODO if can't
            }
        }

        private void FillContentSingleType(SpreadsheetPage page)
        {
            if (dataToUpdateGroup is IScriptableObjectTypedGroup typedGroup)
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

        protected void Log(string text)
        {
            if (debug)
            {
                Debug.Log("DataSync. " + text);
            }
        }
    }
}