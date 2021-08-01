using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class PlusOneEffect : MonoBehaviour
{
    private void Start()
    {
        var seq = DOTween.Sequence().SetId(this);
        seq.Append(transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero));
        seq.Append(transform.DOScale(Vector3.zero, 0.2f).From(Vector3.one));
        seq.AppendCallback(() =>
        {
            Destroy(gameObject);
        });
    }
}