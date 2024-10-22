using DefaultNamespace;
using UnityEngine;
using UnitySpreadsheetSync.Scripts;

namespace UnitySpreadsheetSync.Examples
{
    [CreateAssetMenu(menuName = "Example/Example_2")]
    public class WaveData : ScriptableObject, IDataBindable
    {
        [field: SerializeField] public string id { get; set; }
        public string mob;
        public string type;
        public string time;
        public float qty;
        public float speed;
        public float frequencyQ;
        public float atkSpeed;
        public float hp;
        public float dmg;

        public void Parse(SpreadsheetLine spreadsheetData)
        { 
            DataBinder.Bind(this, spreadsheetData);
        }
    }
}