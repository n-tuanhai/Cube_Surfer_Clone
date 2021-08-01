using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelIndicatorUI : MonoBehaviour
{
    private List<Image> imgList;
    public Sprite completed;
    public Sprite incompleted;
    public Sprite playing;

    public void Init()
    {
        imgList = GetComponentsInChildren<Image>().ToList();
        imgList.RemoveAt(0);
        imgList.RemoveAt(imgList.Count - 1);
    }

    public void UpdateIndicatorRange(int startingIndex)
    {
        for (int i = 0; i < imgList.Count; i++)
        {
            var text = imgList[i].gameObject.GetComponentInChildren<TMP_Text>();
            text.text = (startingIndex + i).ToString();
        }
    }

    public void SetIndicatorLook(int currentIndex)
    {
        foreach (var t in imgList)
        {
            Int32.TryParse(t.gameObject.GetComponentInChildren<TMP_Text>().text, out var idx);
            t.sprite = idx == currentIndex ? playing : idx > currentIndex ? incompleted : completed;
        }
    }
}