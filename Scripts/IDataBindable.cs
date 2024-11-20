using UnitySpreadsheetSync.Scripts.Parser;

namespace UnitySpreadsheetSync.Scripts
{
    public interface IDataBindable
    {
        public string id { get; set; }

        public void Parse(SpreadsheetLine spreadsheetData);
    }
}