using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowModel : MonoBehaviour
{
    [SerializeField] private ScrollSnapRect _scrollSnapRect;
    [SerializeField] private GameObject _characterModel;
    [SerializeField] private GameObject _cubeModel;

    private void Start()
    {
        _cubeModel.SetActive(false);
        _characterModel.SetActive(true);
    }

    private void Update()
    {
        if (_scrollSnapRect.CurrentPage == 0)
        {
            _cubeModel.SetActive(false);
            _characterModel.SetActive(true);    
        }
        else
        {
            _cubeModel.SetActive(true);
            _characterModel.SetActive(false);
        }
    }
}
