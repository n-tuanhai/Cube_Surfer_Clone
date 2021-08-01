using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LevelIndicatorUI _levelIndicatorUI;

    public void InitLevelIndicator()
    {
        _levelIndicatorUI.Init();
    }
    
    public void UpdateLevelIndicator()
    {
        var lvlIdx = PlayerPrefs.GetInt(Settings.LAST_LEVEL_PLAYED) + 1;
        var lvlIdx2 = PlayerPrefs.GetInt(Settings.LAST_BALLOON_LEVEL, -1) + 2;
        
        _levelIndicatorUI.UpdateIndicatorRange(lvlIdx2);
        _levelIndicatorUI.SetIndicatorLook(lvlIdx);
    }
}
