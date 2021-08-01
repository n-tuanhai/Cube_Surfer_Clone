using System.Collections;
using System.Collections.Generic;
using PathCreation.Examples;
using UnityEngine;
using UnityEngine.UI;

public class PressToPlayButton : MonoBehaviour
{
    public void OnClickToPlay()
    {
        PokiAdManager.Instance.GameplayStart();
        FindObjectOfType<PathFollower>().Pressed = true;
        UIController.Instance.HideMainMenu();
        UIController.Instance.ShowLevelSlider();   
    }
}