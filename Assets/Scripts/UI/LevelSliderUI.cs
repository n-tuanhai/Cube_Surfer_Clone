using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using PathCreation.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class LevelSliderUI : MonoBehaviour
{
    public GameObject currentLevelCounter;
    public Slider slider;
    private PathCreator path;
    private PathFollower pathFollower;
    public float maxDistance;

    private void OnEnable()
    {
        path = FindObjectOfType<PathCreator>();
        pathFollower = FindObjectOfType<PathFollower>();
        UpdateLevelCounter();
        GetLevelLength();
    }

    private void Update()
    {
        if (pathFollower.DistanceTravelled <= maxDistance)
        {
            slider.value =  Mathf.Clamp(pathFollower.DistanceTravelled / maxDistance, 0, 1);
        }
    }

    private void GetLevelLength()
    {
        maxDistance = path.path.length - 98f;
    }

    private void UpdateLevelCounter()
    {
        currentLevelCounter.GetComponentInChildren<TMP_Text>().text = (PlayerPrefs.GetInt(Settings.LAST_LEVEL_PLAYED) + 1).ToString();
    }
}
