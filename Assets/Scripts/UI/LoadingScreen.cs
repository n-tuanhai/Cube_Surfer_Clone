using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    
    private AsyncOperation operation;
    private Canvas canvas;

    private void OnEnable()
    {
        EventBroker.hideLoadingScreen += HideLoadScreen;
    }

    private void OnDisable()
    {
        EventBroker.hideLoadingScreen -= HideLoadScreen;
    }

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>(true);
        DontDestroyOnLoad(gameObject);
    }
    
    public void LoadScene(string sceneName)
    {
        UpdateProgressUI(0);
        canvas.gameObject.SetActive(true);

        StartCoroutine(BeginLoad(sceneName));
    }

    private IEnumerator BeginLoad(string sceneName)
    {
        operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            UpdateProgressUI(operation.progress);
            yield return null;
        }
        
        EventBroker.CallFinishedLoading();
        UpdateProgressUI(operation.progress);
        operation = null;
        //canvas.gameObject.SetActive(false);
    }

    private void UpdateProgressUI(float progress)
    {
        slider.value = progress;
    }

    private void HideLoadScreen()
    {
        canvas.gameObject.SetActive(false);
    }
}
