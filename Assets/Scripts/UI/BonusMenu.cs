using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using PathCreation.Examples;
using UnityEngine;
using UnityEngine.UI;

public class BonusMenu : MonoBehaviour
{
    [SerializeField] private int[] _balloonValueList;
    
    public GameObject balloonPanel;
    public GameObject nextLevelButton;
    private List<BalloonButton> _balloonButtonList;
    private int _numberOfBalloonOpened;

    public int NumberOfBalloonOpened
    {
        get => _numberOfBalloonOpened;
        set => _numberOfBalloonOpened = value;
    }
    
    private void Shuffle(ref int[] list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Length;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void InitBalloonButtonList()
    {
        _balloonButtonList = balloonPanel.GetComponentsInChildren<BalloonButton>().ToList();
        _numberOfBalloonOpened = 0;
    }

    public void InitBalloonPanel()
    {
        InitBalloonButtonList();
        Shuffle(ref _balloonValueList);
        
        for (int i = 0; i < _balloonButtonList.Count; i++)
        {
            _balloonButtonList[i].ResetState();
            _balloonButtonList[i].BalloonValue = _balloonValueList[i];
        }
    }
}