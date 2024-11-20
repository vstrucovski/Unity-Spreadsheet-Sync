using UnityEngine;
using UnitySpreadsheetSync.Scripts;
using UnitySpreadsheetSync.Scripts.Data;
using UnitySpreadsheetSync.Scripts.Parser;

namespace UnitySpreadsheetSync.Examples.Example_03
{
    [CreateAssetMenu(menuName = "Example/TestItem")]
    public class TestItem : BindableScriptableObject
    {
        public int value;

        public override void Parse(SpreadsheetLine spreadsheetData)
        {
            DataBinder.Bind(this, spreadsheetData);
        }
    }
}