using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinToggle : MonoBehaviour
{
    [SerializeField] public UnlockableSkin _skin;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private GameObject _skinPage;
    [SerializeField] private Image _locked;
    [SerializeField] private Image _unlocked;
    [SerializeField] private Image _selected;
    [SerializeField] private Sprite _unselected;
    [SerializeField] private Image _unlocking;

    public void InitItem(string currentCubeSkin, string currentCharSkin)
    {
        bool isOn = false;
        _toggle.group = _skinPage.GetComponent<ToggleGroup>();
        _unlocked.sprite = _skin.skinIcon;
        switch (_skin.skinType)
        {
            case SkinType.Character:
                isOn = currentCharSkin != null ? _skin.skinID.Equals(currentCharSkin) : _skin.isPreUnlocked;
                break;
            case SkinType.Cube:
                isOn = currentCubeSkin != null ? _skin.skinID.Equals(currentCubeSkin) : _skin.isPreUnlocked;
                break;
        }

        _toggle.isOn = isOn;
        
        // if (_skin.IsUnlocked)
        //     _toggle.isOn = _skin.skinID.Equals(_skin.skinType == SkinType.Character
        //         ? PlayerPrefs.GetString(Settings.CURRENT_CHARACTER_SKIN)
        //         : PlayerPrefs.GetString(Settings.CURRENT_CUBE_SKIN));
        UpdateItem();
        SetSprite(_skin.IsUnlocked);
    }

    public void OnClick()
    {
        if (_skin.IsUnlocked)
        {
            _skinPage.GetComponent<SkinPageUI>().UpdatePageModel(_skin);
            switch (_skin.skinType)
            {
                case SkinType.Character:
                    PlayerPrefs.SetString(Settings.CURRENT_CHARACTER_SKIN, _skin.skinID);
                    EventBroker.CallChangePlayerSkin(_skin);
                    break;
                case SkinType.Cube:
                    PlayerPrefs.SetString(Settings.CURRENT_CUBE_SKIN, _skin.skinID);
                    EventBroker.CallChangeCubeSkin(_skin);
                    break;
            }
        }
    }

    public void UpdateItem()
    {
        bool unlocked = _skin.IsUnlocked;
        _toggle.interactable = unlocked;
        SetSprite(_skin.IsUnlocked);
    }

    public void SetUnlocking(bool a)
    {
        _unlocking.gameObject.SetActive(a);
        // Sprite tmp = a ? _unlocking.sprite : _unselected; 
        // _toggle.image.sprite = tmp;
    }

    public void SetSprite(bool unlocked)
    {
        _locked.gameObject.SetActive(!unlocked);
        _unlocked.gameObject.SetActive(unlocked);
    }
}