using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RetryMenu : MonoBehaviour
{
    public GameObject gameOverText;
    public GameObject retryButton;
    public GameObject skipButton;
    private bool isRewarded;
    
    private void OnEnable()
    {
        isRewarded = true;
        GetComponent<Image>().DOFade(0.7f, 0.01f).From(0).SetDelay(2);
        gameOverText.transform.DOLocalMove(new Vector3(0f, 0f, 0f), 2).From(new Vector3(0f, 670f, 0f))
            .SetEase(Ease.OutBounce).SetDelay(2);
        retryButton.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).SetDelay(6);
        skipButton.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).SetDelay(4);
        EventBroker.finishedLoading += FinishedLoadingCallback;
    }

    private void OnDisable()
    {
        GetComponent<Image>().DORestart();
        gameOverText.transform.DORestart();
        retryButton.transform.DORestart();
        skipButton.transform.DORestart();
        EventBroker.finishedLoading -= FinishedLoadingCallback;
    }

    private void FinishedLoadingCallback()
    {
        GameManager.Instance.Init(!isRewarded);
        gameObject.SetActive(false);
    }

    public void OnRetryButtonClick()
    {
        var load = FindObjectOfType<LoadingScreen>();
        isRewarded = false;
        //load.LoadScene(SceneManager.GetActiveScene().name);
        PokiAdManager.Instance.CommercialBreak(() => load.LoadScene(SceneManager.GetActiveScene().name));
    }

    public void OnSkipButtonClick()
    {
        PokiAdManager.Instance.RewardedBreak(SetReward);
    }

    private void SetReward(bool isAdWatched)
    {
        isRewarded = isAdWatched;
        var load = FindObjectOfType<LoadingScreen>();
        load.LoadScene(SceneManager.GetActiveScene().name);
    }
}