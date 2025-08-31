using System;
using System.Collections.Generic;
using System.Linq;
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
	private Dictionary<KeywordInfo, KeywordIcon> iconDict;
	[SerializeField] private TextMeshPro atk;
	[SerializeField] private TextMeshPro hp;
	[SerializeField] private TextMeshPro nameText;
	[SerializeField] private TextMeshPro desc;
	[SerializeField] private MeshRenderer TextureRenderer;
	[SerializeField] private TextMeshPro shield;
	[SerializeField] private MeshRenderer backGround;
	[SerializeField] private Transform attackRelevantIconAnchor;
	[SerializeField] private Transform hpRelevantIconAnchor;
	[SerializeField] private float iconSpacing;
	
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
		iconDict = new();

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
		NoticeSystem.Instance.Subscribe<BuffAddNotice>(OnBuffAdd);
		NoticeSystem.Instance.Subscribe<BuffLevelChangeNotice>(OnBuffLevelChange);
		NoticeSystem.Instance.Subscribe<BuffRemoveNotice>(OnBuffRemove);
	}

	private void OnBuffRemove(BuffRemoveNotice m)
	{
		if (m.Target.UnitCardBattleStat != stat) return;
		
		var keyword = GameDataSystem.Instance.GetGameData<KeywordData>().GetKeyword(m.Buff.Keyword);
		iconDict[keyword].GetComponent<PooledUnityObject>().Dispose();
		iconDict.Remove(keyword);
		
		UpdateIcon();
	}

	private void OnBuffLevelChange(BuffLevelChangeNotice m)
	{
		if (m.Target.UnitCardBattleStat != stat) return;
		
		var keyword = GameDataSystem.Instance.GetGameData<KeywordData>().GetKeyword(m.Buff.Keyword);
		//todo: 일단은 버프에 keyword 하나라고 가정
		iconDict[keyword].SetValue(m.Buff.Level);
		
		UpdateIcon();
	}

	private void OnBuffAdd(BuffAddNotice m)
	{
		if (m.Target.UnitCardBattleStat != stat) return;

		var keyword = GameDataSystem.Instance.GetGameData<KeywordData>().GetKeyword(m.Buff.Keyword);
		var resourceName = keyword.IconResource;
		var importance = keyword.Importance;
		switch (keyword.IconCategory)
		{
			case IconCategory.HpRelevant:
				var icon = UnityObjectPool.GetOrCreatePool("Icon", resourceName).Instantiate(parent: hpRelevantIconAnchor).GetComponent<KeywordIcon>();
				iconDict[keyword] = icon;
				icon.SetValue(m.Buff.Level);
				icon.Importance = keyword.Importance;
				break;
			
			case IconCategory.AttackRelevant:
				icon = UnityObjectPool.GetOrCreatePool("Icon", resourceName).Instantiate(parent: attackRelevantIconAnchor).GetComponent<KeywordIcon>();
				iconDict[keyword] = icon;
				icon.SetValue(m.Buff.Level);
				icon.Importance = keyword.Importance;
				break;
		}
		
		UpdateIcon();
	}

	private void UpdateIcon()
	{
		var hpIdx = 0;
		var attackIdx = 0;
		foreach (var icon in iconDict.Values.OrderByDescending(x => x.Importance))
		{
			if (icon.transform.parent == hpRelevantIconAnchor)
			{
				icon.transform.localPosition = Vector3.up * (iconSpacing * hpIdx++);
			}
			else if (icon.transform.parent == attackRelevantIconAnchor)
			{
				icon.transform.localPosition = Vector3.up * (iconSpacing * attackIdx++);
			}
		}
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