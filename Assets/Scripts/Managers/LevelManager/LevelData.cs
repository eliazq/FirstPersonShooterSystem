using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsData", menuName = "Create LevelsData/NewLevelsData", order = 1)]
public class LevelData : ScriptableObject
{

    [Serializable]
    public struct LevelInfo
    {
        public LevelManager.Level level;
        public string sceneName;
    }

    public LevelInfo[] levels;
}
