using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    public SkinPageUI charPage;
    public SkinPageUI cubePage;

    private void OnEnable()
    {
        charPage.InitPage();
        cubePage.InitPage();
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
        UIController.Instance.ShowMainMenu();
    }
}