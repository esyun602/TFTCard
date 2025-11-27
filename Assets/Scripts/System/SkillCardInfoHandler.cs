using System;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
	private ICard targetCard;
	private IStat stat;
	[SerializeField] private TextMeshPro nameText;
	[SerializeField] private TextMeshPro desc;
	[SerializeField] private TextMeshPro cost;
	[SerializeField] private GameObject costStop;
	[SerializeField] private GameObject costFF;
	[SerializeField] private GameObject costRW;
	[SerializeField] private MeshRenderer TextureRenderer;
	[SerializeField] private MeshRenderer BackGround;
	[SerializeField] private GameObject bgFx;
	private Func<bool> isFxOn;
	
	//todo : fix?
	public void Initialize(ICard card, IStat stat, Func<bool> isFxOn)
	{
		if (card is not SkillCardBase skillCard)
		{
			throw new ArgumentException();
		}

		this.isFxOn = isFxOn;
		this.stat = stat;

		targetCard = card;
		nameText.text = card.Name;
		//todo: desc 변화 반영되게
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
			TextureRenderer.material.SetTexture("_BaseMap", card.CardStaticSpec.CardResource.texture);
		}

		//todo: fix
		if (skillCard is UnitSkillCard unitSkillCard
		    && unitSkillCard.UnitSkillCardStat.Owner.Stat.synergyList.Count > 0
		    && BackGround != null)
		{
			var spec = GameDataSystem.Instance.GetGameData<SynergyData>()
				.GetSynergySpec(unitSkillCard.UnitSkillCardStat.Owner.Stat.synergyList[0]);
			BackGround.material.SetColor("_BaseColor", spec.SymbolColor);
		}
	}

	public void Dispose()
	{
		isFxOn = null;
	}

	//todo: callback or notice?
	private void Update()
	{
		//todo: test
		UpdateCost();
		desc.text = targetCard.Desc;
		bgFx.SetActive(isFxOn?.Invoke() ?? false);
	}

	private void UpdateCost()
	{
		if (stat is UnitSkillCardBattleStat us)
		{
			var costValue = stat.GetValueByValueType(SkillValueType.Cost);
			if (costValue >= 0)
			{
				costValue = us.GetCostValueWithModifier();
			}
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
		}
		else if (stat is TacticsCardBattleStat ts)
		{
			var costValue = ts.GetCostValueWithModifier();
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
		}
		
	}
}