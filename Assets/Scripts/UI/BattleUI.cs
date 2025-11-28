using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIGenState
{
	public BlockInputHandler InputHandler { get; set; }
	public Sprite bgSprite { get; set; }
}

public class BattleUI : UIInstance
{
	private BlockInputHandler inputHandler;
	[SerializeField] private GameObject tint;
	[SerializeField]
	private TextMeshProUGUI energy;

	private Dictionary<SynergyCategory, SynergyLabel> synergyLabelMap = new();
	[SerializeField]
	private SynergyLabel synergyLabelPrefab;

	[SerializeField] 
	private Transform synergyContentTransform;
	public override UIType UIType => UIType.SceneCameraUI;
	[SerializeField] private Image bg;

	private ArrowDrawer arrowDrawer;
	private TargetMarkerManager targetMarkerManager;

	private Vector2 currentSelectedCardStartPosition;

	private EnemyGauge enemyGauge;
	
	protected override void Init(object param)
	{
		NoticeSystem.Instance.Subscribe<EnergyChangeNotice>(OnEnergyChange);
		NoticeSystem.Instance.Subscribe<BagSynergyUpdateNotice>(OnSynergyUpdate);
		NoticeSystem.Instance.Subscribe<SkillHandCardHoverNotice>(OnHandCardHover);
		NoticeSystem.Instance.Subscribe<SkillHandCardRemoveHoverNotice>(OnHandCardRemove);
		NoticeSystem.Instance.Subscribe<EnemyIconHoverNotice>(OnEnemyCardHover);
		NoticeSystem.Instance.Subscribe<EnemyIconRemoveHoverNotice>(OnEnemyCardRemoveHover);
		NoticeSystem.Instance.Subscribe<TargetingCardAimedNotice>(OnAimed);
		NoticeSystem.Instance.Subscribe<TargetingCardAimRemovedNotice>(OnAimRemoved);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnHandCardStartUse);
		NoticeSystem.Instance.Subscribe<SkillHandCardTargetingUpdateNotice>(OnTargetingUpdate);
		
		NoticeSystem.Instance.Subscribe<BattleObjectDestroyedNotice>(OnBoDestroy);
		NoticeSystem.Instance.Subscribe<EnemyCardRegisteredNotice>(OnEnemyCardRegister);
		NoticeSystem.Instance.Subscribe<CurrentUsedCostChangeNotice>(OnCostChange);
		NoticeSystem.Instance.Subscribe<SkillCardActionTriggerNotice>(OnActionTrigger);
		
		inputHandler = ((BattleUIGenState)param).InputHandler;
		bg.sprite = ((BattleUIGenState)param).bgSprite;
		
		//todo: child 구현하면 수정
		arrowDrawer = Game.Instance.UIManager.GenerateUI<ArrowDrawer>();
		targetMarkerManager = Game.Instance.UIManager.GenerateUI<TargetMarkerManager>();
		enemyGauge = Game.Instance.UIManager.GenerateUI<EnemyGauge>();

		InitializeSynergyInfo(Game.Instance.GetPlayer().CurrentPlayInfo.SynergyNumDict);
	}

	private void OnEnemyCardHover(EnemyIconHoverNotice m)
	{
		targetMarkerManager.SetTargetMarkerTo(m.Target.TargetCard.Action.Targets, m.Target);
		tint.SetActive(true);
	}

	private void OnEnemyCardRemoveHover(EnemyIconRemoveHoverNotice m)
	{
		targetMarkerManager.RemoveTargetMarker(m.Target);
		tint.SetActive(false);
	}

	private void OnBoDestroy(BattleObjectDestroyedNotice m)
	{
		enemyGauge.SetCardDisable(m.Target);
	}

	private void OnActionTrigger(SkillCardActionTriggerNotice m)
	{
		enemyGauge.SetCardUse(m.TargetAction);
	}

	private void OnCostChange(CurrentUsedCostChangeNotice m)
	{
		enemyGauge.SetFill(m.CurrentUsedCost);
	}

	private void OnEnemyCardRegister(EnemyCardRegisteredNotice m)
	{
		enemyGauge.InitializeBar(m.TotalCost, m.CumCost, m.CardList);
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

	private void InitializeSynergyInfo(IEnumerable<KeyValuePair<SynergyCategory, int>> dict)
	{
		foreach (var kvp in synergyLabelMap)
		{
			Destroy(kvp.Value.gameObject);
		}
		synergyLabelMap.Clear();
		
		
		foreach (var kvp in dict)
		{
			var category = kvp.Key;
			var num = kvp.Value;
			
			synergyLabelMap[category] = Instantiate(synergyLabelPrefab, synergyContentTransform);
			synergyLabelMap[category].Initialize(category, num);
		}
	}
	
	private void OnSynergyUpdate(BagSynergyUpdateNotice m)
	{
		InitializeSynergyInfo(m.SynergyInfo);
	}

	private void OnEnergyChange(EnergyChangeNotice m)
	{
	}

	public void OnTurnEndClick()
	{
		if (inputHandler.IsBlocked(InputBlockFlag.TurnEnd))
			return;
		
		NoticeSystem.Instance.Publish(new TurnEndClickNotice());
	}

	public void OpenDrawPanel(bool isEnemy)
	{
		if (isEnemy)
		{
			Game.Instance.UIManager.GenerateUI<BattleCardListPanel>(new BattleCardListPanelGenState()
			{
				enemyCardInfoList = Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.EnemyCardPool.GetShuffled()
			});
			
		}
		else
		{
			Game.Instance.UIManager.GenerateUI<BattleCardListPanel>(new BattleCardListPanelGenState()
			{
				cardInfoList =  Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Deck.GetShuffled()
			});
			
		}
	}

	public void OpenDiscardPanel(bool isEnemy)
	{
		if (isEnemy)
		{
			Game.Instance.UIManager.GenerateUI<BattleCardListPanel>(new BattleCardListPanelGenState()
			{
				enemyCardInfoList = Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.EnemyDropCardList.GetShuffled()
			});
			
		}
		else
		{
			Game.Instance.UIManager.GenerateUI<BattleCardListPanel>(new BattleCardListPanelGenState()
			{
				cardInfoList =  Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DropCardList.GetShuffled()
			});
			
		}
	}

	public void OpenExhaustionCardPanel()
	{
		//todo: impl
	}

	private void Update()
	{
		//todo: remove test code
		if (Game.Instance.GetGameMode<BattleStageGameMode>()?.WaveSystem?.LeftNextWaveTurn <= 0)
		{
			energy.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			energy.transform.parent.gameObject.SetActive(true);
			if (energy.text != Game.Instance.GetGameMode<BattleStageGameMode>()?.WaveSystem?.LeftNextWaveTurn
				    .ToString())
			{
				energy.text = Game.Instance.GetGameMode<BattleStageGameMode>()?.WaveSystem?.LeftNextWaveTurn.ToString();
				energy.DOKill(true);
				energy.transform.localScale = Vector3.one * 2;
				energy.transform.DOScale(Vector3.one, 0.5f);
			}
		}
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<EnergyChangeNotice>(OnEnergyChange);
		NoticeSystem.Instance.Unsubscribe<BagSynergyUpdateNotice>(OnSynergyUpdate);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardHoverNotice>(OnHandCardHover);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardRemoveHoverNotice>(OnHandCardRemove);
		NoticeSystem.Instance.Unsubscribe<EnemyIconHoverNotice>(OnEnemyCardHover);
		NoticeSystem.Instance.Unsubscribe<EnemyIconRemoveHoverNotice>(OnEnemyCardRemoveHover);
		NoticeSystem.Instance.Unsubscribe<TargetingCardAimedNotice>(OnAimed);
		NoticeSystem.Instance.Unsubscribe<TargetingCardAimRemovedNotice>(OnAimRemoved);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectNotice>(OnHandCardSelect);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectCancelNotice>(OnHandCardSelectCancel);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnHandCardStartUse);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardTargetingUpdateNotice>(OnTargetingUpdate);
		
		NoticeSystem.Instance.Unsubscribe<BattleObjectDestroyedNotice>(OnBoDestroy);
		NoticeSystem.Instance.Unsubscribe<EnemyCardRegisteredNotice>(OnEnemyCardRegister);
		NoticeSystem.Instance.Unsubscribe<CurrentUsedCostChangeNotice>(OnCostChange);
		NoticeSystem.Instance.Unsubscribe<SkillCardActionTriggerNotice>(OnActionTrigger);
	}
}