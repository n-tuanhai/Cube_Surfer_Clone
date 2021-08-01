using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class BalloonButton : MonoBehaviour
{
    private int _balloonValue;
    private BonusMenu bonusMenu;
    private Button button;
    public TMP_Text gemVal;
    public Image gemImg;

    public int BalloonValue
    {
        get => _balloonValue;
        set => _balloonValue = value;
    }

    [ContextMenu("test anim")]
    public void ClickAnim()
    {
        if (bonusMenu.NumberOfBalloonOpened == 3) return;
        bonusMenu.NumberOfBalloonOpened++;

        button.interactable = false;
        button.targetGraphic.transform.DOKill();
        button.targetGraphic.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.OutQuad);
        button.targetGraphic.transform.DOScale(1.0f, 0.2f).SetEase(Ease.OutQuad);

        button.targetGraphic.transform.DOShakePosition(1.6f, new Vector3(20, 0, 0), 50, 0f).SetEase(Ease.InCubic);
        button.targetGraphic.transform.DOScale(new Vector3(0.8f, 0.6f), 1.0f).SetEase(Ease.InCubic);
        button.targetGraphic.transform.DOScale(new Vector3(1.1f, 1.1f), 0.1f).SetEase(Ease.OutQuart).SetDelay(1.6f)
            .OnComplete(ShowGem);
    }

    public void ResetState()
    {
        bonusMenu = FindObjectOfType<BonusMenu>();
        button = gameObject.GetComponent<Button>();
        button.interactable = true;
        
        button.targetGraphic.gameObject.SetActive(true);
        button.targetGraphic.transform.rotation = quaternion.identity;
        button.targetGraphic.transform.localScale = Vector3.one;
        
        button.targetGraphic.transform.DOKill();
        button.targetGraphic.transform.DOLocalRotate(new Vector3(0, 0, -15), 1.0f)
            .SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
        button.targetGraphic.transform.DOScale(1.1f, 1.0f)
            .SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
        
        gemVal.gameObject.SetActive(false);
        gemImg.gameObject.SetActive(false);
    }

    private void ShowGem()
    {
        button.targetGraphic.gameObject.SetActive(false);
        gemVal.text = _balloonValue.ToString();
        gemVal.gameObject.SetActive(true);
        gemImg.gameObject.SetActive(true);

        EventBroker.CallGemCollected(_balloonValue);

        if (bonusMenu.NumberOfBalloonOpened == 3) bonusMenu.nextLevelButton.gameObject.SetActive(true);
    }
}