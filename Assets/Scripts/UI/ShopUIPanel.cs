using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIPanelGenState
{
	public int rollCount { get; set; }
	public Action doneAction { get; set; }
}

public class ShopUIPanel : UIInstance
{
	public override UIType UIType => UIType.SceneUI;
	private int rollCount;
	private Action cancelAction;

	private List<ICardSpec> cardDataList;
	[SerializeField] private List<Image> cardImageList;
	[SerializeField] private TextMeshProUGUI rollCountUI;

	protected override void Init(object param)
	{
		rollCount = ((ShopUIPanelGenState)param).rollCount;
		cancelAction = ((ShopUIPanelGenState)param).doneAction;

		UpdateRollCountText();
		RenewCandidates();
	}

	private void RenewCandidates()
	{
		cardDataList = new();
		//todo: constant
		for (int i = 0; i < 5; i++)
		{
			//todo: fix
			cardDataList.Add(GameDataSystem.Instance.GetGameData<CardData>().GetRandomUnitCardSpec());
		}

		for (int i = 0; i < 5; i++)
		{
			cardImageList[i].gameObject.SetActive(true);
			cardImageList[i].sprite = cardDataList[i].CardResource;
		}
	}

	public void OnCloseClick()
	{
		cancelAction?.Invoke();
		Hide();
	}

	public void OnRollClick()
	{
		RenewCandidates();
		rollCount--;
		UpdateRollCountText();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateRollCountText()
	{
		rollCountUI.text = $"Roll\nCount\n{rollCount}";
	}

	public void OnCardClick(int idx)
	{
		//todo: 어케하지
		//Game.Instance.GetPlayer().CurrentPlayInfo.CardList.Add(new UnitCard(cardDataList[idx]));
		//cardImageList[idx].gameObject.SetActive(false);
	}
}