using System.Collections.Generic;
using UnityEngine;

namespace UnitySpreadsheetSync.Scripts
{
    [CreateAssetMenu(menuName = "Tools/Data group")]
    public class ScriptableObjectGroup : ScriptableObject
    {
        [SerializeField] private List<ScriptableObject> _list;

        public List<ScriptableObject> List => _list;

        public ScriptableObject Get(int index)
        {
            return List[index];
        }
    }
}