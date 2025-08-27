using System;
using System.Collections.Generic;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
	[SerializeField] private TextMeshProUGUI atk;
	[SerializeField] private TextMeshProUGUI hp;
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI desc;
	[SerializeField] private Image img;
	[SerializeField] private Image bgImg;
	
	public void Initialize(ICard card, IStat stat)
	{
		if (card is not UnitCard unitCard)
		{
			throw new ArgumentException();
		}
		
		nameText.text = card.Name;
		desc.text = card.Desc;
		if (card.CardStaticSpec.CardResource != null)
		{
			img.sprite = card.CardStaticSpec.CardResource;
		}
		
		//todo:fix
		var synergySpec = GameDataSystem.Instance.GetGameData<SynergyData>()
			.GetSynergySpec(unitCard.Stat.synergyList[0]);
		bgImg.color = synergySpec.SymbolColor;
		
		atk.text = $"{stat.GetValueByValueType(UnitValueType.Attack)}";
		hp.text = $"{stat.GetValueByValueType(UnitValueType.Hp)}";
	}
	
	//todo: callback or notice?
	private void Update()
	{
	}
}