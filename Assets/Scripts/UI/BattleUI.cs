using System;
using MessageSystem;
using TMPro;
using UnityEngine;

public class BattleUIGenState
{
	public BlockInputHandler InputHandler { get; set; }
}

public class BattleUI : UIInstance
{
	private BlockInputHandler inputHandler;
	[SerializeField]
	private TextMeshProUGUI energy;
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		NoticeSystem.Instance.Subscribe<EnergyChangeNotice>(OnEnergyChange);
		inputHandler = ((BattleUIGenState)param).InputHandler;
	}

	private void OnEnergyChange(EnergyChangeNotice m)
	{
		energy.text = m.CurValue.ToString();
	}

	public void OnTurnEndClick()
	{
		if (inputHandler.IsBlocked(InputBlockFlag.Select))
			return;
		
		NoticeSystem.Instance.Publish(new TurnEndClickNotice());
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<EnergyChangeNotice>(OnEnergyChange);
	}
}