using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SkinPageUI : MonoBehaviour
{
    private List<SkinToggle> _skinToggles;
    [SerializeField] private GameObject pageModel;
    [SerializeField] private Button unlockButton;
    [SerializeField] private int pageCost;
    [SerializeField] private Image blocker;
    [SerializeField] private TMP_Text cost;

    public void InitPage()
    {
        var currentCharSkin = PlayerPrefs.GetString(Settings.CURRENT_CHARACTER_SKIN);
        var currentCubeSkin = PlayerPrefs.GetString(Settings.CURRENT_CUBE_SKIN); 
        _skinToggles = GetComponentsInChildren<SkinToggle>().ToList();
        cost.text = pageCost.ToString();
        foreach (var toggle in _skinToggles)
        {
            toggle.InitItem(currentCubeSkin, currentCharSkin);
        }
    }

    public void UpdatePageModel(UnlockableSkin skin)
    {
        switch (skin.skinType)
        {
            case SkinType.Character:
                    var skinnedMeshRenderer = pageModel.GetComponentInChildren<SkinnedMeshRenderer>();
                    var newMeshRenderer = skin.skinModel.GetComponentInChildren<SkinnedMeshRenderer>();
                    // update mesh
                    skinnedMeshRenderer.sharedMesh = newMeshRenderer.sharedMesh;
                    Transform[] children = transform.GetComponentsInChildren<Transform> (true);
                    // // sort bones.
                    // Transform[] bones = new Transform[newMeshRenderer.bones.Length];
                    // for (int boneOrder = 0; boneOrder < newMeshRenderer.bones.Length; boneOrder++) {
                    //     bones [boneOrder] = Array.Find<Transform> (children, c => c.name.Equals(newMeshRenderer.bones [boneOrder].name));
                    // }
                    // skinnedMeshRenderer.bones = bones;
                break;
            case SkinType.Cube:
                pageModel.GetComponent<MeshRenderer>().material = skin.skinMat;
                break;
        }
    }

    private void Update()
    {
        unlockButton.interactable = UnlockButtonCheck();
    }

    public void UnlockButtonOnClick()
    {
        EventBroker.CallGemReducedFromPurchase(pageCost);
        var locked = _skinToggles.Where(i => !i._skin.IsUnlocked).ToList();

        float interval = 0.35f;
        int maxTimes = 15;
        int lastIndex = -1;
        SkinToggle currRndToggle = null;
        
        blocker.gameObject.SetActive(true);
        if (locked.Count == 1)
        {
            currRndToggle = locked[0];
            UnlockItem(currRndToggle);
            return;
        }
        
        var seq = DOTween.Sequence().SetId(this);
        seq.Append(DOTween.To(f =>
        {
            int newIndex = Mathf.FloorToInt(f);
            if (newIndex != lastIndex)
            {
                lastIndex = newIndex;
                if (currRndToggle == null) currRndToggle = locked[Random.Range(0, locked.Count)];
                else
                {
                    var tmp = locked.Where(i => !i._skin.skinID.Equals(currRndToggle._skin.skinID)).ToList();
                    currRndToggle = tmp.ElementAt(Random.Range(0, tmp.Count));
                }

                foreach (var i in _skinToggles)
                {
                    i.SetUnlocking(i._skin.skinID.Equals(currRndToggle._skin.skinID));
                }
            }
        }, 0, maxTimes, maxTimes * interval).SetEase(Ease.OutQuad));
        seq.AppendCallback(() =>
        {
            UnlockItem(currRndToggle);
        });
    }

    public void UnlockItem(SkinToggle toggle)
    {
        blocker.gameObject.SetActive(false);
        toggle._skin.IsUnlocked = true;
        toggle.SetUnlocking(false);
        toggle.UpdateItem();
    }

    public bool CheckUnlockedEverything()
    {
        var locked = _skinToggles.Where(i => i._skin.IsUnlocked).ToList();
        return locked.Count == 0;
    }

    public bool UnlockButtonCheck()
    {
        return PlayerPrefs.GetInt(Settings.GEM) >= pageCost && !CheckUnlockedEverything();
    }
}