using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    public Button balloonButton;
    public Button nextButton;
    public LevelGemCounter levelGemCounter;

    private void OnEnable()
    {
        GetComponent<Image>().DOFade(0.7f, 0.01f).From(0).SetDelay(2);
        if (nextButton.gameObject.activeSelf)
        {
            nextButton.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).SetDelay(6);
        }
        else if (balloonButton.gameObject.activeSelf)
        {
            balloonButton.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).SetDelay(6);
        }

        EventBroker.finishedLoading += FinishedLoadingCallback;
    }

    private void OnDisable()
    {
        GetComponent<Image>().DORestart();
        nextButton.transform.DORestart();
        balloonButton.transform.DORestart();
        EventBroker.finishedLoading -= FinishedLoadingCallback;
    }
    
    public void DisableAllChildrenUI()
    {
        levelGemCounter.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        balloonButton.gameObject.SetActive(false);
    }

    private void FinishedLoadingCallback()
    {
        GameManager.Instance.Init(false);
        DisableAllChildrenUI();
        gameObject.SetActive(false);
    }
    
    public void NextLevel()
    {
        var load = FindObjectOfType<LoadingScreen>();
        //load.LoadScene(SceneManager.GetActiveScene().name);
        PokiAdManager.Instance.CommercialBreak(() => load.LoadScene(SceneManager.GetActiveScene().name));
    }

    public void SendShowBalloonUI()
    {
        UIController.Instance.ShowBonusUI();
    }
}