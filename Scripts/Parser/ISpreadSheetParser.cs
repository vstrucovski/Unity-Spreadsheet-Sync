namespace UnitySpreadsheetSync.Scripts.Parser
{
    public interface ISpreadSheetParser
    {
        void ParseAndFill(string csvContent);
    }
}