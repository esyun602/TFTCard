using System;
using System.Collections.Generic;
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

	private Dictionary<SynergyCategory, SynergyLabel> synergyLabelMap = new();
	[SerializeField]
	private SynergyLabel synergyLabelPrefab;

	[SerializeField] 
	private Transform synergyContentTransform;
	public override UIType UIType => UIType.SceneUI;

	[SerializeField] private ArrowDrawer arrowDrawer;

	private Vector2 currentSelectedCardStartPosition;
	
	protected override void Init(object param)
	{
		NoticeSystem.Instance.Subscribe<EnergyChangeNotice>(OnEnergyChange);
		NoticeSystem.Instance.Subscribe<SynergyInfoUpdateNotice>(OnSynergyUpdate);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnHandCardStartUse);
		NoticeSystem.Instance.Subscribe<SkillHandCardTargetingUpdateNotice>(OnTargetingUpdate);
		
		inputHandler = ((BattleUIGenState)param).InputHandler;
	}

	private void OnTargetingUpdate(SkillHandCardTargetingUpdateNotice m)
	{
		//todo: 임시구현  수정
		var controlPos = new Vector2(currentSelectedCardStartPosition.x,
			Mathf.Lerp(currentSelectedCardStartPosition.y, m.Position.y, 0.9f));
		arrowDrawer.SetArrowTarget(controlPos, m.Position);
	}

	private void OnHandCardSelect(SkillHandCardSelectNotice m)
	{
		if (!m.SelectedCard.IsTargeting) return;
		currentSelectedCardStartPosition = Camera.main.WorldToScreenPoint(m.SelectedCard.transform.position);
		arrowDrawer.Activate(currentSelectedCardStartPosition, 10);
	}

	private void OnHandCardSelectCancel(SkillHandCardSelectCancelNotice m)
	{
		if (!m.SelectedCard.IsTargeting) return;
		arrowDrawer.Deactivate();
	}

	private void OnHandCardStartUse(SkillHandCardStartUseNotice m)
	{
		if (!m.SelectedCard.IsTargeting) return;
		arrowDrawer.Deactivate();
	}

	private void OnSynergyUpdate(SynergyInfoUpdateNotice m)
	{
		if (m.Count <= 0)
		{
			synergyLabelMap[m.TargetSynergyCategory] = null;

			return;
		}
		
		if (!synergyLabelMap.TryGetValue(m.TargetSynergyCategory, out var label))
		{
			label = Instantiate(synergyLabelPrefab, synergyContentTransform);
			label.Initialize(m.TargetSynergyCategory);
			synergyLabelMap[m.TargetSynergyCategory] = label;
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
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnHandCardStartUse);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardTargetingUpdateNotice>(OnTargetingUpdate);
	}
}