using System.Collections.Generic;
using UnityEngine;

namespace UnitySpreadsheetSync.Scripts.Data
{
    public interface IScriptableObjectGroup
    {
        List<ScriptableObject> List { get; }
    }
}