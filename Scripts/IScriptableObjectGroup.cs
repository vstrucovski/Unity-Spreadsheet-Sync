using System.Collections.Generic;
using UnityEngine;

namespace UnitySpreadsheetSync.Scripts
{
    public interface IScriptableObjectGroup
    {
        List<ScriptableObject> List { get; }
    }
}