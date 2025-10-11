using System;
using DG.Tweening;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyCardIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image usedGo;
    [SerializeField] private Image notUsedGo;

    private bool isHovered;

    public EnemySkillCardObject CardObject { get; set; }
    
    public void SetUse(bool used, bool immediate = false)
    {
        var time = immediate ? 0 : 0.25f;
        if (used)
        {
            usedGo.DOFade(1f, time);
        }
        else
        {
            usedGo.DOFade(0f, time);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemoveHover();
    }

    private void OnDisable()
    {
        if (isHovered)
        {
            RemoveHover();
        }
    }

    private void SetHover()
    {
        isHovered = true;
        SfxManager.Instance.Play2D("cardhover");
        CardObject.Activate();
        CardObject.transform.position =
            CardObject.Stat.Owner.Position.GetX0z(Constant.HighlightYPos) + Vector3.right * 3;
        var owner = (CardObject.Stat.Owner as UnitCardInField);
        if (owner != null) owner.IsHighlighted = true;

        NoticeSystem.Instance.Publish(new EnemyIconHoverNotice(CardObject));
    }

    private void RemoveHover()
    {
        isHovered = false;
        SfxManager.Instance.Play2D("cardhover");
        CardObject.Deactivate();
        CardObject.Stat.Owner.FrameTransform.localPosition = CardObject.Stat.Owner.FrameTransform.localPosition.GetX0z(0);
        var owner = (CardObject.Stat.Owner as UnitCardInField);
        if (owner != null) owner.IsHighlighted = false;
        
        NoticeSystem.Instance.Publish(new EnemyIconRemoveHoverNotice(CardObject));
    }
}