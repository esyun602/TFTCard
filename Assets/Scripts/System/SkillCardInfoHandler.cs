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
	
	public void Initialize(ICard card, IStat stat)
	{
		if (card is not SkillCard)
		{
			throw new ArgumentException();
		}

		this.stat = stat;

		targetCard = card;
		nameText.text = card.Name;
		//todo: desc 변화 반영되게
		desc.text = card.Desc;
		cost.text = $"{stat.GetValueByValueType(BattleValueType.Cost)}";
		if (card.CardStaticSpec.CardResource != null)
		{
			TextureRenderer.material.SetTexture("_BaseMap", card.CardStaticSpec.CardResource.texture);
		}

	}

	//todo: callback or notice?
	private void Update()
	{
		//todo: test
		desc.text = targetCard.Desc;
	}
}