using UnityEngine;
using UnitySpreadsheetSync.Scripts.Parser;

namespace UnitySpreadsheetSync.Scripts.Data
{
    public abstract class BindableScriptableObject : ScriptableObject, IDataBindable
    {
        [field: SerializeField] public string id { get; set; }

        public virtual void Parse(SpreadsheetLine spreadsheetData)
        {
            DataBinder.Bind(this, spreadsheetData);
        }
    }
}