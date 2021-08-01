using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokiAdManager : MonoBehaviour
{
    public static PokiAdManager Instance;

    private void Awake()
    {
        Debug.Log("Poki Init");
        if (Instance == null)
        {
            Instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
        PokiUnitySDK.Instance.init();
    }

    public void GameplayStart()
    {
        PokiUnitySDK.Instance.gameplayStart();
        Debug.Log("Gameplay Start");
    }

    public void GameplayStop()
    {
        PokiUnitySDK.Instance.gameplayStop();
        Debug.Log("Gameplay Stop");
    }

    public void HappyTime()
    {
        PokiUnitySDK.Instance.happyTime(0.5f);
        Debug.Log("Happy Time");
    }

    public void CommercialBreak(Action onAdComplete = null)
    {
        if (onAdComplete == null) return;
#if UNITY_EDITOR
        Debug.Log("Commercial time");
        onAdComplete.Invoke();
#else
            if (!PokiUnitySDK.Instance.adsBlocked())
            {
                PokiUnitySDK.Instance.commercialBreakCallBack = onAdComplete.Invoke;
                PokiUnitySDK.Instance.commercialBreak();
            }
            else
            {
                onAdComplete.Invoke();
            }
#endif
    }
    
    public void RewardedBreak(Action<bool> onRewarded)
    {
        if (onRewarded == null) return;
#if UNITY_EDITOR
        Debug.Log("Reward Time");
        onRewarded.Invoke(true);
#else
            if (!PokiUnitySDK.Instance.adsBlocked())
            {
                PokiUnitySDK.Instance.rewardedBreakCallBack = onRewarded.Invoke;
                PokiUnitySDK.Instance.rewardedBreak();
            }
#endif
    }
}