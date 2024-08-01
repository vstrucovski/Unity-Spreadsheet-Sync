using System;
using System.Collections;
using UnityEngine.Networking;

namespace UnitySpreadsheetSync.Scripts
{
    public class CsvDataDownloader : IDataDownloader
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public IEnumerator Download(string url, Action<string> onSuccess, Action<string> onError)
        {
            var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError(request.error);
            }
            else
            {
                onSuccess(request.downloadHandler.text);
            }
        }
    }
}