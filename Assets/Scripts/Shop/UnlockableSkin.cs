using System;
using InspectorGadgets;
using InspectorGadgets.Attributes;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockableSkin", menuName = "Game/Skin")]
public class UnlockableSkin : ScriptableObject
{
    public string skinID;
    public string skinName;

    [Space] public Material skinMat;
    public GameObject skinModel;
    public Gradient trailColor;

    [Space] public Sprite skinIcon;

    [Space] public SkinType skinType;
    public int skinUnlockCost;
    public bool isPreUnlocked;

    [field: NonSerialized] public event Action OnUnlockChanged;
    public static event Action OnAnyUnlockChanged;

    string _UnlockKey => "_SkinUnlock_" + skinID;

    public bool IsUnlocked
    {
        get => isPreUnlocked || PlayerPrefs.GetFloat(_UnlockKey) > 0;
        set
        {
            PlayerPrefs.SetFloat(_UnlockKey, 1);
            OnUnlockChanged?.Invoke();
            OnAnyUnlockChanged?.Invoke();
        }
    }

#if UNITY_EDITOR
    public void Reset()
    {
        skinID = GUID.Generate().ToString();
    }
#endif
}