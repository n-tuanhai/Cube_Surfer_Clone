using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;


public class GemCounter : MonoBehaviour
{
    public TMP_Text gemText;
    private int _gemValue;
    private bool _didWin;

    private void Start()
    {
        EventBroker.updateGemCounterUI += UpdateGemText;
        EventBroker.levelWin += ShowTotalGem;
        EventBroker.levelLose += ShowTotalGem;
        _gemValue = GameManager.Instance.TotalGem;
        gemText.text = _gemValue.ToString();
    }

    private void OnDestroy()
    {
        EventBroker.updateGemCounterUI -= UpdateGemText;
        EventBroker.levelLose -= ShowTotalGem;
        EventBroker.levelWin -= ShowTotalGem;
    }

    private void UpdateGemText()
    {
        _gemValue = GameManager.Instance.TotalGem + GameManager.Instance.GemcCollectedInLevel;
        gemText.text = _gemValue.ToString();
    }

    private void ShowTotalGem()
    {
        _gemValue = GameManager.Instance.TotalGem;
        gemText.text = _gemValue.ToString();
    }
}