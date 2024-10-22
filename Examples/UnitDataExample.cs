using UnityEngine;
using UnitySpreadsheetSync.Scripts;

namespace UnitySpreadsheetSync.Examples
{
    [CreateAssetMenu(menuName = "Example/UnitData")]
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