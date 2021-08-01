using System;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class LevelGemCounter : MonoBehaviour
{
    public TMP_Text multiplier;
    public TMP_Text gemCollected;
    public GameObject gemIndicator;

    private void OnEnable()
    {
        transform.DOLocalMove(new Vector3(0f, 210f, 0f), 2).From(new Vector3(0f, 724f, 0f))
            .SetEase(Ease.OutQuart).SetDelay(2);
        gemIndicator.SetActive(true);
        gemIndicator.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).SetDelay(4);
    }

    private void OnDisable()
    {
        transform.DORestart();
        gemIndicator.transform.DORestart();
        gemIndicator.SetActive(false);
    }

    public void ShowGemCollected()
    {
        multiplier.text = $"Great! \n x{GameManager.Instance.GemMultiplier}";
        gemCollected.text = $"{GameManager.Instance.GemcCollectedInLevel}";
    }
}