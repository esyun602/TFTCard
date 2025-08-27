using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Image img;
    [SerializeField] private Image bgImg;
	
    public void Initialize(ICard card, IStat stat)
    {
        if (card is not SkillCardBase skillCard)
        {
            throw new ArgumentException();
        }
		
        nameText.text = card.Name;
        desc.text = card.Desc;
        cost.text = stat.GetValueByValueType(SkillValueType.Cost).ToString();
        if (card.CardStaticSpec.CardResource != null)
        {
            img.sprite = card.CardStaticSpec.CardResource;
        }
        
        if (skillCard is UnitSkillCard unitSkillCard && unitSkillCard.UnitSkillCardStat.Owner != null)
        {
            var spec = GameDataSystem.Instance.GetGameData<SynergyData>()
                .GetSynergySpec(unitSkillCard.UnitSkillCardStat.Owner.Stat.synergyList[0]);
            bgImg.color = spec.SymbolColor;
        }
    }
	
    //todo: callback or notice?
    private void Update()
    {
    }
}