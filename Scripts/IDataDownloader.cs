using System;
using System.Collections;

namespace UnitySpreadsheetSync.Scripts
{
    public interface IDataDownloader
    {
        IEnumerator Download(string url, Action<string> onSuccess, Action<string> onError);
    }
}