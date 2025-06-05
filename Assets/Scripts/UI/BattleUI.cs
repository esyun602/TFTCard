using System;
using System.Collections.Generic;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleUIGenState
{
	public BlockInputHandler InputHandler { get; set; }
}

public class BattleUI : UIInstance
{
	private BlockInputHandler inputHandler;
	[SerializeField]
	private TextMeshProUGUI energy;

	private Dictionary<Synergy, SynergyLabel> synergyLabelMap = new();
	[SerializeField]
	private SynergyLabel synergyLabelPrefab;

	[SerializeField] 
	private Transform synergyContentTransform;

	[SerializeField] 
	private GameObject turnStartNoticePanel;
	
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		NoticeSystem.Instance.Subscribe<EnergyChangeNotice>(OnEnergyChange);
		NoticeSystem.Instance.Subscribe<SynergyInfoUpdateNotice>(OnSynergyUpdate);
		NoticeSystem.Instance.Subscribe<PlayerTurnStartNotice>(OnPlayerTurnSTart);
		inputHandler = ((BattleUIGenState)param).InputHandler;
	}

	private void OnPlayerTurnSTart(PlayerTurnStartNotice m)
	{
		turnStartNoticePanel.gameObject.SetActive(true);
	}

	private void OnSynergyUpdate(SynergyInfoUpdateNotice m)
	{
		if (m.Count <= 0)
		{
			synergyLabelMap[m.TargetSynergy] = null;

			return;
		}
		
		if (!synergyLabelMap.TryGetValue(m.TargetSynergy, out var label))
		{
			label = Instantiate(synergyLabelPrefab, synergyContentTransform);
			label.Initialize(m.TargetSynergy);
			synergyLabelMap[m.TargetSynergy] = label;
		}
		
		label.SynergyCount = m.Count;
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

	private void Update()
	{
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<EnergyChangeNotice>(OnEnergyChange);
		NoticeSystem.Instance.Unsubscribe<SynergyInfoUpdateNotice>(OnSynergyUpdate);
	}
}