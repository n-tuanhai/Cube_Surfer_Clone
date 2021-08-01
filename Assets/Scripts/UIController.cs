using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private RetryMenu retryMenu;
    [SerializeField] private WinMenu winMenu;
    [SerializeField] private BonusMenu bonusMenu;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private LevelSliderUI _levelSliderUI;
    [SerializeField] private Button _pressToPlayButton;
    [SerializeField] private ShopMenu _shopMenu;

    // Start is called before the first frame update
    public static UIController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
    }

    private void Start()
    {
        EventBroker.levelWin += ShowWinUIElements;
        EventBroker.levelLose += ShowLoseUIElements;
        _mainMenu.InitLevelIndicator();
    }

    private void OnDisable()
    {
        EventBroker.levelWin -= ShowWinUIElements;
        EventBroker.levelLose -= ShowLoseUIElements;
    }

    public void ShowMainMenu()
    {
        _mainMenu.gameObject.SetActive(true);
        _mainMenu.UpdateLevelIndicator();
        _levelSliderUI.gameObject.SetActive(false);
        _pressToPlayButton.gameObject.SetActive(true);
    }

    public void HideMainMenu()
    {
        _mainMenu.gameObject.SetActive(false);
        _pressToPlayButton.gameObject.SetActive(false);
    }

    public void ShowLevelSlider()
    {
        _levelSliderUI.gameObject.SetActive(true);
    }

    public void ShowBonusUI()
    {
        winMenu.gameObject.SetActive(true);
        winMenu.DisableAllChildrenUI();
        bonusMenu.gameObject.SetActive(true);
        bonusMenu.InitBalloonPanel();
    }

    public void ShowShopMenu()
    {
        _shopMenu.gameObject.SetActive(true);
        HideMainMenu();
    }

    public void HideAllLevelTransitionUI()
    {
        retryMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        bonusMenu.gameObject.SetActive(false);
    }

    private void ShowWinUIElements()
    {
        winMenu.levelGemCounter.ShowGemCollected();
        winMenu.levelGemCounter.gameObject.SetActive(true);
        _levelSliderUI.gameObject.SetActive(false);

        var lvlplayed = PlayerPrefs.GetInt(Settings.NUMBER_OF_LEVELS_PLAYED, 0);
        
        if (lvlplayed + 1 >= 5 && (lvlplayed + 1) % 5 == 0)
            winMenu.balloonButton.gameObject.SetActive(true);
        else winMenu.nextButton.gameObject.SetActive(true);
        winMenu.gameObject.SetActive(true);
    }

    private void ShowLoseUIElements()
    {
        retryMenu.gameObject.SetActive(true);
    }
}