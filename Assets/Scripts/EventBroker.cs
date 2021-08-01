using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Vector3 = UnityEngine.Vector3;

public class EventBroker
{
    public static event Action levelWin;
    public static event Action<UnlockableSkin> changePlayerSkin;
    public static event Action<UnlockableSkin> changeCubeSkin;
    public static event Action upgradeCubeTower;
    public static event Action levelLose;
    public static event Action<int> numberOfCubeChanged;
    public static event Action updateGemCounterUI;
    public static event Action<int> gemCollectedFromBonus;
    public static event Action<int> gemReducedFromPurchase;
    public static event Action<Vector3> collectCubeFX;
    public static event Action<Vector3> collectGemFX;
    public static event Action finishedLoading;
    public static event Action hideLoadingScreen;

    public static void CallFinishedLoading()
    {
        if (finishedLoading != null)
            finishedLoading();
    }
    
    public static void CallHideLoadingScreen() 
    {
        if (hideLoadingScreen != null)
            hideLoadingScreen();
    }
    
    public static void CallCollectGemFX(Vector3 pos)
    {
        if (collectGemFX != null)
            collectGemFX(pos);
    }
    public static void CallCollectCubeFX(Vector3 pos)
    {
        if (collectCubeFX != null)
            collectCubeFX(pos);
    }
    public static void CallGemReducedFromPurchase(int amount)
    {
        if (gemReducedFromPurchase != null)
            gemReducedFromPurchase(amount);
    }

    public static void CallNumberOfCubeChanged(int amount)
    {
        if (numberOfCubeChanged != null)
            numberOfCubeChanged(amount);
    } 
        
    public static void CallUpgradeCubeTower()
    {
        if (upgradeCubeTower != null)
            upgradeCubeTower();
    }
    
    public static void CallChangePlayerSkin(UnlockableSkin charSkin)
    {
        if (changePlayerSkin != null)
            changePlayerSkin(charSkin);
    }

    public static void CallChangeCubeSkin(UnlockableSkin cubeSkin)
    {
        if (changeCubeSkin != null)
            changeCubeSkin(cubeSkin);
    }

    public static void CallUpdateGemUI()
    {
        if (updateGemCounterUI != null)
            updateGemCounterUI();
    }

    public static void CallLevelWin()
    {
        if (levelWin != null)
            levelWin();
    }

    public static void CallLevelLose()
    {
        if (levelLose != null)
            levelLose();
    }

    public static void CallGemCollected(int gem)
    {
        if (gemCollectedFromBonus != null)
            gemCollectedFromBonus(gem);
    }
}