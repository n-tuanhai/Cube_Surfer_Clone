using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCubeButton : MonoBehaviour
{
    private Button _button;
    private int _currentUpgradeLevel;
    private int _currentGem;
    public TMP_Text costText;
    public TMP_Text lvlText;
    [SerializeField] private int upgradeCost = 1000;
    
    private void Start()
    {
        _button = GetComponent<Button>();
        _currentGem = PlayerPrefs.GetInt(Settings.GEM, 0);
        _currentUpgradeLevel = PlayerPrefs.GetInt(Settings.STARTING_CUBE_NUMBER, 1);
        _button.interactable = ClickableCheck();
    }

    private void Update()
    {
        costText.text = _currentUpgradeLevel == 4 ? "MAX" : upgradeCost.ToString();
        lvlText.text = $"LV {_currentUpgradeLevel.ToString()}";
        _button.interactable = ClickableCheck();
    }

    public void OnUpgradeClick()
    {
        EventBroker.CallUpgradeCubeTower();    
        EventBroker.CallGemReducedFromPurchase(upgradeCost);
        PlayerPrefs.SetInt(Settings.STARTING_CUBE_NUMBER, _currentUpgradeLevel++);
    }

    private bool ClickableCheck()
    {
        _currentGem = PlayerPrefs.GetInt(Settings.GEM, 0);
        return _currentUpgradeLevel < 4 && _currentGem >= upgradeCost;
    }
}