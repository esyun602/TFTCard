using System;
using System.Collections.Generic;
using System.Linq;
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
	public override UIType UIType => UIType.SceneCameraUI;

	private ArrowDrawer arrowDrawer;
	private TargetMarkerManager targetMarkerManager;

	private Vector2 currentSelectedCardStartPosition;
	
	protected override void Init(object param)
	{
		NoticeSystem.Instance.Subscribe<EnergyChangeNotice>(OnEnergyChange);
		NoticeSystem.Instance.Subscribe<SynergyInfoUpdateNotice>(OnSynergyUpdate);
		NoticeSystem.Instance.Subscribe<SkillHandCardHoverNotice>(OnHandCardHover);
		NoticeSystem.Instance.Subscribe<SkillHandCardRemoveHoverNotice>(OnHandCardRemove);
		NoticeSystem.Instance.Subscribe<TargetingCardAimedNotice>(OnAimed);
		NoticeSystem.Instance.Subscribe<TargetingCardAimRemovedNotice>(OnAimRemoved);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnHandCardStartUse);
		NoticeSystem.Instance.Subscribe<SkillHandCardTargetingUpdateNotice>(OnTargetingUpdate);
		
		inputHandler = ((BattleUIGenState)param).InputHandler;
		
		//todo: child 구현하면 수정
		arrowDrawer = Game.Instance.UIManager.GenerateUI<ArrowDrawer>();
		targetMarkerManager = Game.Instance.UIManager.GenerateUI<TargetMarkerManager>();
		
	}

	private void OnAimed(TargetingCardAimedNotice m)
	{
		targetMarkerManager.SetTargetMarkerTo(m.Card.TargetCard.Action.Targets, m.Card);
	}

	private void OnAimRemoved(TargetingCardAimRemovedNotice m)
	{
		targetMarkerManager.RemoveTargetMarker(m.Card);
	}

	private void OnHandCardHover(SkillHandCardHoverNotice m)
	{
		if (!m.SelectedCard.IsTargeting)
		{
			targetMarkerManager.SetTargetMarkerTo(m.SelectedCard.TargetCard.Action.Targets, m.SelectedCard);
		}
	}

	private void OnHandCardRemove(SkillHandCardRemoveHoverNotice m)
	{
		if (!m.SelectedCard.IsTargeting)
		{
			targetMarkerManager.RemoveTargetMarker(m.SelectedCard);
		}
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
		if (m.SelectedCard.IsTargeting)
		{
			currentSelectedCardStartPosition = Camera.main.WorldToScreenPoint(m.SelectedCard.transform.position);
			arrowDrawer.Activate(currentSelectedCardStartPosition, 10);
		}
		else
		{
			targetMarkerManager.SetTargetMarkerTo(m.SelectedCard.TargetCard.Action.Targets, m.SelectedCard);
		}
	}

	private void OnHandCardSelectCancel(SkillHandCardSelectCancelNotice m)
	{
		if (m.SelectedCard.IsTargeting)
		{
			arrowDrawer.Deactivate();
		}
		
		
		targetMarkerManager.RemoveTargetMarker(m.SelectedCard);
			
	}

	private void OnHandCardStartUse(SkillHandCardStartUseNotice m)
	{
		if (m.SelectedCard.IsTargeting)
		{
			arrowDrawer.Deactivate();
		}
		
		targetMarkerManager.RemoveTargetMarker(m.SelectedCard);
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
		if (inputHandler.IsBlocked(InputBlockFlag.TurnEnd))
			return;
		
		NoticeSystem.Instance.Publish(new TurnEndClickNotice());
	}

	public void OpenDrawPanel(bool isEnemy)
	{
		Game.Instance.UIManager.GenerateUI<BattleCardListPanel>(new BattleCardListPanelGenState()
		{
			cardInfoList = 
				isEnemy
				? Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.EnemyCardPool.GetShuffled()
				: Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Deck.GetShuffled()
		});
	}

	public void OpenDiscardPanel(bool isEnemy)
	{
		Game.Instance.UIManager.GenerateUI<BattleCardListPanel>(new BattleCardListPanelGenState()
		{
			cardInfoList = 
				isEnemy
					? Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.EnemyDropCardList.GetShuffled()
					: Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DropCardList.GetShuffled()
		});
	}

	public void OpenExhaustionCardPanel()
	{
		//todo: impl
	}

	private void Update()
	{
		//todo: remove test code
		energy.text = Game.Instance.GetGameMode<BattleStageGameMode>()?.WaveSystem?.LeftNextWaveTurn.ToString();
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<EnergyChangeNotice>(OnEnergyChange);
		NoticeSystem.Instance.Unsubscribe<SynergyInfoUpdateNotice>(OnSynergyUpdate);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardHoverNotice>(OnHandCardHover);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardRemoveHoverNotice>(OnHandCardRemove);
		NoticeSystem.Instance.Unsubscribe<TargetingCardAimedNotice>(OnAimed);
		NoticeSystem.Instance.Unsubscribe<TargetingCardAimRemovedNotice>(OnAimRemoved);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnHandCardStartUse);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardTargetingUpdateNotice>(OnTargetingUpdate);
	}
}