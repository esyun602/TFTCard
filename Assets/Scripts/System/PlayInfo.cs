using System.Collections.Generic;
using System.Linq;
using MessageSystem;
using UnityEngine;

public class DeployInfo
{
	public DeployInfo(int row, int col, UnitCard targetCard, bool isFixed = false)
	{
		Row = row;
		Col = col;
		TargetCard = targetCard;
		IsFixed = isFixed;
	}

	public int Row { get; set; }
	public int Col { get; set; }
	public UnitCard TargetCard { get; set; }
	public bool IsFixed { get; set; }
}

public class PlayInfo
{
	private int gold;
	public int Gold
	{
		get => gold;
		private set
		{
			gold = value;
			NoticeSystem.Instance.Publish(new GoldUpdateNotice(gold));
		}
	}

	public void GainGold(int val)
	{
		if (val <= 0) return;
		Gold += val;
	}

	public bool TryUseGold(int val)
	{
		if (Gold > val)
		{
			Gold -= val;
			return true;
		}

		return false;
	}
	
	
	private List<UnitCard> bagUnitCardList = new();
	public IEnumerable<UnitCard> BagUnitCardList => bagUnitCardList;

	public IEnumerable<UnitCard> TotalUnitCards
	{
		get
		{
			foreach (var card in BagUnitCardList)
			{
				yield return card;
			}

			foreach (var info in FieldDeployLocationInfo)
			{
				yield return info.TargetCard;
			}
		}
	}
	
	public IEnumerable<SkillCardBase> TotalDeckCards
	{
		get
		{
			foreach (var card in TacticsCardList)
			{
				yield return card;
			}

			
			//todo: 가방 고칠 때 수정
			foreach (var card in UnitSkillCardList)
			{
				yield return card;
			}
		}
	}

	//todo: 가방 고칠 때 수정
	public int TotalDeckCardsCount => TacticsCardList.Count() + UnitSkillCardList.Count;

	private List<TacticsCard> tacticsCardList = new();
	//todo: fix?
	public IEnumerable<TacticsCard> TacticsCardList => tacticsCardList;

	//todo: 가방 고칠 때 수정
	public List<UnitSkillCard> UnitSkillCardList => FieldDeployLocationInfo.SelectMany(x => x.TargetCard.UnitSkillCard).ToList();
	public int CurrentUsingDeploySlot => FieldDeployLocationInfo.Count((x) => !x.IsFixed);
	public List<DeployInfo> FieldDeployLocationInfo { get; } = new();
	public List<DeployInfo> SpecialDeployLocationInfo { get; } = new();
	private Dictionary<SynergyCategory, int> synergyNumDict = new();
	private Dictionary<SynergyCategory, IGlobalSynergy> activatedByDeploySynergyDict { get; } = new();
	public FlowInfo CurrentFlowInfo { get; set; }

	public FlowNodeInfo CurrentSelectedNode { get; private set; }

	//todo: to constant
	//todo: test
	public int MaxDeployCount { get; private set; } = 30;
	public int DeckDrawCount { get; private set; } = 5;
	public int EnemyDrawCount { get; private set; } = 3;
	

	//todo: additional value
	public int MaxEnergy => Constant.DefaultMaxEnergy;
	public int MinEnergy => Constant.DefaultMinEnergy;
	public int EnergyPerTurn => Constant.DefaultEnergy;

	public T GetGlobalSynergy<T>(SynergyCategory synergyCategory) where T : IGlobalSynergy
	{
		if (activatedByDeploySynergyDict.TryGetValue(synergyCategory, out var value) && value is T ret)
		{
			return ret;
		}

		return default;
	}
	
	/// <summary>
	/// 최대 배치 가능 갯수에 맞게 normalize
	/// </summary>
	public void NormalizeFieldDeployInfo()
	{
		if (CurrentUsingDeploySlot == MaxDeployCount)
		{
			return;
		}

		if (CurrentUsingDeploySlot < MaxDeployCount)
		{
			var toDeployCount = Mathf.Min(MaxDeployCount - CurrentUsingDeploySlot, BagUnitCardList.Count());
			for (var i = 0; i < toDeployCount; i++)
			{
				var targetCard = BagUnitCardList.Last();
				DeployToSomewhere(targetCard);
			}

		}
		else
		{
			//상황 발생 가능??
			//처리 방식 어떻게 할 지 논의
		}
	}

	public void DeployToSomewhere(UnitCard targetCard, bool isFixed = false)
	{
		var (row, col) = GetSomewhereCoord();
		DeployCard(row, col, targetCard, isFixed);
	}

	private (int row, int col) GetSomewhereCoord()
	{
		for (var row = 2; row >= 0; row--)
		{
			for (var col = 3; col >= 0; col--)
			{
				if (!FieldDeployLocationInfo.Any(info => info.Row == row && info.Col == col))
				{
					return (row, col);
				}
			}
		}

		return (-1, -1);
	}

	public void DeployCard(int row, int col, UnitCard targetCard, bool isFixed = false)
	{
		var info = FieldDeployLocationInfo.Find(info => info.TargetCard == targetCard);
		var isInField = FieldDeployLocationInfo.Remove(info);
		
		RemoveCard(targetCard);
		if (FieldDeployLocationInfo.Any(x => x.Row == row && x.Col == col))
		{
			(row, col) = GetSomewhereCoord();
			FieldDeployLocationInfo.Add(new DeployInfo(row, col, targetCard, isFixed));
		}
		else
		{
			FieldDeployLocationInfo.Add(new DeployInfo(row, col, targetCard, isFixed));
		}

		NormalizeLocationInfos();
		
		if (!isInField)
		{
			//var unitSkillCard = targetCard.UnitSkillCard;
			foreach (var synergy in targetCard.Stat.synergyList)
			{
				if (!synergyNumDict.TryAdd(synergy, 1))
				{
					synergyNumDict[synergy]++;
				}
				
				
				if (activatedByDeploySynergyDict.TryGetValue(synergy, out var synergyInstance))
				{
					synergyInstance.AddMember(targetCard);
				}
				else
				{
					if (GameDataSystem.Instance.GetGameData<SynergyData>()
					    .GetSynergySpec(synergy).TryGenerateGlobalSynergyInstance(out var newSynergy))
					{
						activatedByDeploySynergyDict[synergy] = newSynergy;
						newSynergy.Initialize();
						newSynergy.AddMember(targetCard);
					}
				}
			}

			//UnitSkillCardList.Add(unitSkillCard);
			RefreshSynergyList();
		}

	}

	public void UndeployCard(UnitCard targetCard, bool returnToBag = true)
	{
		FieldDeployLocationInfo.RemoveAll(info => info.TargetCard == targetCard);
		if (returnToBag)
		{
			AddCard(targetCard);
		}
		NormalizeLocationInfos();
		
		//var unitSkillCard = targetCard.UnitSkillCard;
		//UnitSkillCardList.Remove(unitSkillCard);

		foreach (var synergy in targetCard.Stat.synergyList)
		{
			synergyNumDict[synergy]--;
			
			if (activatedByDeploySynergyDict.TryGetValue(synergy, out var synergyInstance))
			{
				synergyInstance.RemoveMember(targetCard);
			}
		}

		RefreshSynergyList();
	}

	public void AddCard(ICard card)
	{
		if (card is UnitCard unitCard)
		{
			bagUnitCardList.Add(unitCard);
			bagUnitCardList.Sort((x,y) => x.UnitCardStaticSpec.Id.CompareTo(y.UnitCardStaticSpec.Id));
		}
		else if (card is TacticsCard tacticsCard)
		{
			tacticsCardList.Add(tacticsCard);
			tacticsCardList.Sort((x,y) => x.SkillCardStaticSpec.Id.CompareTo(y.SkillCardStaticSpec.Id));
		}
	}

	public bool RemoveCard(ICard card)
	{
		var removed = false;
		if (card is UnitCard unitCard)
		{
			removed = bagUnitCardList.Remove(unitCard);
			bagUnitCardList.Sort((x,y) => x.UnitCardStaticSpec.Id.CompareTo(y.UnitCardStaticSpec.Id));
		}
		else if (card is TacticsCard tacticsCard)
		{
			removed = tacticsCardList.Remove(tacticsCard);
			tacticsCardList.Sort((x,y) => x.SkillCardStaticSpec.Id.CompareTo(y.SkillCardStaticSpec.Id));
		}

		return removed;
	}

	private void RefreshSynergyList()
	{
		var kvps = synergyNumDict.ToList();
		foreach (var kvp in kvps)
		{
			if (kvp.Value == 0)
			{
				synergyNumDict.Remove(kvp.Key);
				if (activatedByDeploySynergyDict.TryGetValue(kvp.Key, out var synergy))
				{
					synergy.Dispose();
					activatedByDeploySynergyDict.Remove(kvp.Key);
				}
			}
		}
	}

	private void NormalizeLocationInfos()
	{
		for (int row = 0; row < 3; row++)
		{
			if (!FieldDeployLocationInfo.Any(info => info.Row == row && info.Col == 3))
			{
				for (int col = 2; col >= 0; col--)
				{
					var info = FieldDeployLocationInfo.Find(info => info.Row == row && info.Col == col);

					if (info != null)
					{
						info.Col = 3;
						break;
					}
				}
			}
		}
	}


	public void Initialize()
	{
		NoticeSystem.Instance.Subscribe<FlowNodeSelectNotice>(OnMapNodeSelect);
		NoticeSystem.Instance.Subscribe<StageClearNotice>(OnStageClear);
	}

	private void OnMapNodeSelect(FlowNodeSelectNotice m)
	{
		CurrentSelectedNode = m.TargetInfo;
		foreach (var node in CurrentFlowInfo.GetCousins(CurrentSelectedNode))
		{
			node.CloseNode();
		}
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<FlowNodeSelectNotice>(OnMapNodeSelect);
		NoticeSystem.Instance.Unsubscribe<StageClearNotice>(OnStageClear);
	}

	private void OnStageClear(StageClearNotice m)
	{
		CurrentSelectedNode.ClearNode();
		foreach (var child in CurrentSelectedNode.Children)
		{
			child.OpenNode();
		}
	}
}