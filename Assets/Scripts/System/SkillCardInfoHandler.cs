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
	[SerializeField] private MeshRenderer TextureRenderer;
	[SerializeField] private MeshRenderer BackGround;
	
	public void Initialize(ICard card, IStat stat)
	{
		if (card is not SkillCardBase skillCard)
		{
			throw new ArgumentException();
		}

		this.stat = stat;

		targetCard = card;
		nameText.text = card.Name;
		//todo: desc 변화 반영되게
		desc.text = card.Desc;
		cost.text = $"{stat.GetValueByValueType(SkillValueType.Cost)}";
		if (card.CardStaticSpec.CardResource != null)
		{
			TextureRenderer.material.SetTexture("_BaseMap", card.CardStaticSpec.CardResource.texture);
		}

		//todo: fix
		if (skillCard is UnitSkillCard unitSkillCard && unitSkillCard.UnitSkillCardStat.Owner.Stat.synergyList.Count > 0)
		{
			var spec = GameDataSystem.Instance.GetGameData<SynergyData>()
				.GetSynergySpec(unitSkillCard.UnitSkillCardStat.Owner.Stat.synergyList[0]);
			BackGround.material.SetColor("_BaseColor", spec.SymbolColor);
		}
	}

	//todo: callback or notice?
	private void Update()
	{
		//todo: test
		desc.text = targetCard.Desc;
	}
}