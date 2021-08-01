using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GemFlyEffect : MonoBehaviour
{
    public void InitTween(Vector3 from, Vector3 to)
    {
        var seq = DOTween.Sequence().SetId(this);
        seq.Append(transform.DOMove(to, 0.5f).From(from)
            .SetEase(Ease.InOutQuad));

        seq.AppendCallback(() => { Destroy(gameObject); });
    }
}