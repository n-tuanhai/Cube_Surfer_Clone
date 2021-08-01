using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GemCounterFX : MonoBehaviour
{
    private void OnEnable()
    {
        EventBroker.collectGemFX += DoGemCounterFX;
    }

    private void OnDisable()
    {
        EventBroker.collectGemFX -= DoGemCounterFX;
    }

    private void DoGemCounterFX(Vector3 a)
    {
        Vector3 to = new Vector3(1.2f, 1.2f, 1.2f);
        var seq = DOTween.Sequence().SetId(this);
        seq.Append(transform.DOScale(to, 0.1f).From(Vector3.one));
        seq.Append(transform.DOScale(Vector3.one, 0.1f).From(to));
        seq.OnComplete(() => seq.Kill());
    }
}
