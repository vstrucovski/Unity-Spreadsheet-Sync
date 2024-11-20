using System.Collections.Generic;
using UnityEngine;

namespace UnitySpreadsheetSync.Scripts.Data
{
    public interface IScriptableObjectTypedGroup
    {
        List<ScriptableObject> List { get; }

        ScriptableObject CreateSubAsset();
    }
}