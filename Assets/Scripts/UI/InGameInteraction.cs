
using System;
using System.Linq;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;

public class InGameInteraction : UIInstance
{
	[SerializeField] private TextMeshProUGUI goldText;
	[SerializeField] private GameObject arrow; 
	public override UIType UIType => UIType.DontDestroyUI;
	public static InGameInteraction Instance { get; private set; }
	protected override void Init(object param)
	{
		if (Instance != null)
		{
			DestroyImmediate(this);
			return;
		}

		Instance = this;

		goldText.text = GetGoldText(Game.Instance.GetPlayer().CurrentPlayInfo.Gold);
		NoticeSystem.Instance.Subscribe<GoldUpdateNotice>(OnGoldUpdate);
		NoticeSystem.Instance.Subscribe<UnitCardAddNotice>(OnUnitCardAdd);
		NoticeSystem.Instance.Subscribe<UnitCardRemoveNotice>(OnUnitCardRemove);

		if (Game.Instance.GetPlayer().CurrentPlayInfo.TotalUnitCards.Any())
		{
			ActivateArrow();
		}
		
		transform.parent.SetAsFirstSibling();
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<GoldUpdateNotice>(OnGoldUpdate);
		NoticeSystem.Instance.Unsubscribe<UnitCardAddNotice>(OnUnitCardAdd);
		NoticeSystem.Instance.Unsubscribe<UnitCardRemoveNotice>(OnUnitCardRemove);
	}

	private void OnUnitCardAdd(UnitCardAddNotice m)
	{
		ActivateArrow();
	}

	private void OnUnitCardRemove(UnitCardRemoveNotice m)
	{
		if (!Game.Instance.GetPlayer().CurrentPlayInfo.TotalUnitCards.Any())
		{
			DeactivateArrow();
		}
	}

	private void ActivateArrow()
	{
		arrow.SetActive(true);
		arrow.transform.DORewind();
		arrow.transform.DOMoveY(arrow.transform.position.y + 20f, 0.5f).SetLoops(-1, LoopType.Yoyo);
	}

	private void DeactivateArrow()
	{
		arrow.SetActive(false);
	}

	private void OnGoldUpdate(GoldUpdateNotice m)
	{
		goldText.text = GetGoldText(m.Value);
	}

	private string GetGoldText(int val)
	{
		return  $"{val}G";
	}


	public void OpenBagPanel()
	{
		DeactivateArrow();
		Game.Instance.UIManager.GenerateUI<PlayerBagPanel>();
	}

	public void OpenGiveUpPanel()
	{
		Game.Instance.UIManager.GenerateUI<GiveUpPanel>();
	}

	private void Update()
	{
		
	}
}