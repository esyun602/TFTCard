using System;
using System.Collections.Generic;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
	private IStat stat;
	private ICard targetCard;
	private Dictionary<UnitValueType, TextMeshPro> valueMap;
	[SerializeField] private TextMeshPro atk;
	[SerializeField] private TextMeshPro hp;
	[SerializeField] private TextMeshPro nameText;
	[SerializeField] private TextMeshPro desc;
	[SerializeField] private MeshRenderer TextureRenderer;
	[SerializeField] private TextMeshPro shield;
	[SerializeField] private MeshRenderer backGround;
	
	public void Initialize(ICard card, IStat stat)
	{
		if (card is not UnitCard unitCard)
		{
			throw new ArgumentException();
		}

		valueMap = new()
		{
			[UnitValueType.Attack] = atk,
			[UnitValueType.Hp] = hp,
			[UnitValueType.Shield] = shield,
		};

		this.stat = stat;

		targetCard = card;
		nameText.text = card.Name;
		desc.text = card.Desc;
		if (card.CardStaticSpec.CardResource != null)
		{
			TextureRenderer.material.SetTexture("_BaseMap", card.CardStaticSpec.CardResource.texture);
		}
		
		//todo:fix
		if (unitCard.Stat.synergyList.Count > 0)
		{
			var synergySpec = GameDataSystem.Instance.GetGameData<SynergyData>()
				.GetSynergySpec(unitCard.Stat.synergyList[0]);
			backGround.material.SetColor("_BaseColor", synergySpec.SymbolColor);
		}
		
		
		atk.text = $"{stat.GetValueByValueType(UnitValueType.Attack)}";
		hp.text = $"{stat.GetValueByValueType(UnitValueType.Hp)}";
		
		//todo: important
		//todo: dispose
		NoticeSystem.Instance.Subscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
	}

	private void OnBattleValueChange(UnitBattleValueChangeNotice m)
	{
		if (m.Stat != stat) return;

		if (m.Type == UnitValueType.Shield)
		{
			OnBattleShieldChange(m);
		}
		else
		{
			JustChangeValue(m);
		}
	}

	private void JustChangeValue(UnitBattleValueChangeNotice m)
	{
		if (!valueMap.TryGetValue(m.Type, out var targetText)) return;
		
		DOTween.Kill(targetText);
		targetText.text = $"{stat.GetValueByValueType(m.Type)}";
		targetText.transform.localScale = Vector3.one * 2f;
		targetText.transform.DOScale(Vector3.one,  0.5f);

		desc.text = targetCard.Desc;
	}

	private void OnBattleShieldChange(UnitBattleValueChangeNotice m)
	{
		if (m.ChangedValue == 0)
		{
			shield.gameObject.SetActive(false);
		}
		else if (m.ChangedValue > 0 && !shield.gameObject.activeSelf)
		{
			shield.gameObject.SetActive(true);	
		}

		JustChangeValue(m);
	}

	//todo: callback or notice?
	private void Update()
	{
	}
}