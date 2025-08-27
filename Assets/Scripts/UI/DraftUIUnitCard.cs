using DG.Tweening;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraftUIUnitCard : DraftUICard
{
    public override ICard TargetCard => unitCard;
    private UnitCard unitCard;
    
    public override void OnInitialize(ICardSpec targetCard)
    {
        unitCard = new UnitCard((UnitCardSpec)targetCard);
        SetInfo();
    }
    
    private void SetInfo()
    {
        infoHandler.Initialize(TargetCard, unitCard.Stat);
    }
}