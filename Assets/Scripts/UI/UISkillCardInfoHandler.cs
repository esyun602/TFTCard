using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private GameObject costStop;
    [SerializeField] private GameObject costFF;
    [SerializeField] private GameObject costRW;
    [SerializeField] private Image img;
    [SerializeField] private Image bgImg;
	
    public void Initialize(ICard card, IStat stat, Func<bool> isFxOn)
    {
        if (card is not SkillCardBase skillCard)
        {
            throw new ArgumentException();
        }
		
        nameText.text = card.Name;
        desc.text = card.Desc;
        var costValue = stat.GetValueByValueType(SkillValueType.Cost);
        cost.text = $"{Mathf.Abs(costValue)}";
        if (costValue > 0)
        {
            costFF.SetActive(false);
            costRW.SetActive(true);
            costStop.SetActive(false);
        }
        else if (costValue == 0)
        {
            costFF.SetActive(false);
            costRW.SetActive(false);
            costStop.SetActive(true);
			
        }
        else
        {
            costFF.SetActive(true);
            costRW.SetActive(false);
            costStop.SetActive(false);
			
        }
        
        if (card.CardStaticSpec.CardResource != null)
        {
            img.sprite = card.CardStaticSpec.CardResource;
        }
        
        //todo: fix
        if (skillCard is UnitSkillCard unitSkillCard 
            && unitSkillCard.UnitSkillCardStat.Owner != null
            && unitSkillCard.UnitSkillCardStat.Owner.Stat.synergyList.Count > 0)
        {
            var spec = GameDataSystem.Instance.GetGameData<SynergyData>()
                .GetSynergySpec(unitSkillCard.UnitSkillCardStat.Owner.Stat.synergyList[0]);
            bgImg.color = spec.SymbolColor;
        }
    }

    public void Dispose()
    {
    }

    //todo: callback or notice?
    private void Update()
    {
    }
}