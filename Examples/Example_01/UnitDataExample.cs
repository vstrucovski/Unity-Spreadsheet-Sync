using UnityEngine;
using UnitySpreadsheetSync.Scripts;
using UnitySpreadsheetSync.Scripts.Parser;

namespace UnitySpreadsheetSync.Examples.Example_01
{
    [CreateAssetMenu(menuName = "CSVSync/Example/Unit Data (1)")]
    public class UnitDataExample : ScriptableObject, IDataBindable
    {
        [field: SerializeField] public string id { get; set; } //first column
        public int health;                                     //second column
        public float attack;                                   //third column

        public void Parse(SpreadsheetLine spreadsheetData)
        {
            DataBinder.Bind(this, spreadsheetData);
        }
    }
}