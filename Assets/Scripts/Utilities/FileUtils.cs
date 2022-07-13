using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Utils
{
    public static class FileUtils
    {
        public static LevelInfo GetCurrentLevelInfo(int maxLevel) => Resources.Load<LevelInfo>("Levels/" + Mathf.Clamp(PlayerPrefs.GetInt("next_levelSc", 1), 1, maxLevel));

        public static LevelInfo GetLevelInfo(int levelNum) => Resources.Load<LevelInfo>("Levels/" + levelNum);
    }
}
