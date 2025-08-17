using DG.Tweening;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraftUIUnitCard : DraftUICard
{
    public override void OnInitialize(ICardSpec targetCard)
    {
        TargetCard = new UnitCard((UnitCardSpec)targetCard);
        SetInfo();
    }
    
    private void SetInfo()
    {
        infoHandler.Initialize(TargetCard, ((UnitCard)TargetCard).Stat);
    }
}