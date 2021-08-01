using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXUI : MonoBehaviour
{
    public RectTransform rect;
    public Camera uiCamera;
    public Camera worldCamera;
    public GameObject plusOneEffect;
    public GameObject gemFlyEffect;
    public GameObject gemFlyEffectDestination;

    private void OnEnable()
    {
        EventBroker.collectCubeFX += SpawnCollectCubeFX;
        EventBroker.collectGemFX += SpawnCollectGemFX;
    }

    private void OnDisable()
    {
        EventBroker.collectCubeFX -= SpawnCollectCubeFX;
        EventBroker.collectGemFX -= SpawnCollectGemFX;
    }

    private void Update()
    {
        if (worldCamera != null) return;
        worldCamera = FindObjectOfType<CameraController>().GetComponent<Camera>();
    }

    private void SpawnCollectGemFX(Vector3 pos)
    {
        var sp = worldCamera.WorldToScreenPoint(pos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, sp, uiCamera,
            out var anchoredPos);

        var gemfx = Instantiate(gemFlyEffect, anchoredPos, Quaternion.identity);
        gemfx.transform.SetParent(transform, false);
        gemfx.GetComponent<GemFlyEffect>().InitTween(pos, gemFlyEffectDestination.transform.position);
    }

    private void SpawnCollectCubeFX(Vector3 pos)
    {
        var sp = worldCamera.WorldToScreenPoint(pos);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, sp, uiCamera,
            out var anchoredPos);
        
        var eff =Instantiate(plusOneEffect, anchoredPos, Quaternion.identity);
        eff.transform.SetParent(transform, false);
    }
}