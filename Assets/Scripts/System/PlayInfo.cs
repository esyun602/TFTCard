using System.Collections.Generic;
using System.Linq;
using MessageSystem;
using UnityEngine;

public class DeployInfo
{
	public DeployInfo(int row, int col, UnitCard targetCard)
	{
		Row = row;
		Col = col;
		TargetCard = targetCard;
	}

	public int Row { get; set; }
	public int Col { get; set; }
	public UnitCard TargetCard { get; set; }
}

public class PlayInfo
{
	public List<UnitCard> BagUnitCardList { get; } = new();

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

	//todo: fix?
	public List<SkillCard> DeckCardList { get; } = new();
	public List<DeployInfo> FieldDeployLocationInfo { get; } = new();
	private Dictionary<SynergyCategory, int> synergyNumDict = new();
	public Dictionary<SynergyCategory, IGlobalSynergy> activatedByDeploySynergyDict { get; } = new();
	public MapInfo CurrentMapInfo { get; set; }

	public MapNodeInfo CurrentSelectedNode { get; private set; }

	//todo: to constant
	//todo: test
	public int MaxDeployCount { get; private set; } = 30;
	public int DrawCount { get; private set; } = 5;

	//todo: additional value
	public int MaxEnergy => Constant.DefaultEnergy;

	/// <summary>
	/// 최대 배치 가능 갯수에 맞게 normalize
	/// </summary>
	public void NormalizeFieldDeployLocationInfo()
	{
		if (FieldDeployLocationInfo.Count == MaxDeployCount)
		{
			return;
		}

		if (FieldDeployLocationInfo.Count < MaxDeployCount)
		{
			var toDeployCount = Mathf.Min(MaxDeployCount - FieldDeployLocationInfo.Count, BagUnitCardList.Count);

			for (var row = 2; row >= 0; row--)
			{
				for (var col = 3; col >= 0; col--)
				{
					if (toDeployCount == 0)
					{
						return;
					}

					if (!FieldDeployLocationInfo.Any(info => info.Row == row && info.Col == col))
					{
						toDeployCount--;
						var targetCard = BagUnitCardList[^1];
						DeployCard(row, col, targetCard);
					}
				}
			}
		}
		else
		{
			//상황 발생 가능??
			//처리 방식 어떻게 할 지 논의
		}
	}

	public void DeployCard(int row, int col, UnitCard targetCard)
	{
		var isInBag = BagUnitCardList.Remove(targetCard);
		if (isInBag)
		{
			var unitSkillCard = targetCard.UnitSkillCard;
			foreach (var synergy in targetCard.Stat.synergyList)
			{
				if (!synergyNumDict.TryAdd(synergy, 1))
				{
					synergyNumDict[synergy]++;
				}
			}

			DeckCardList.Add(unitSkillCard);
			RefreshSynergyList();
		}

		var info = FieldDeployLocationInfo.Find(info => info.TargetCard == targetCard);

		FieldDeployLocationInfo.Remove(info);

		FieldDeployLocationInfo.Add(new DeployInfo(row, col, targetCard));

		NormalizeLocationInfos();
	}

	public void UndeployCard(UnitCard targetCard)
	{
		FieldDeployLocationInfo.RemoveAll(info => info.TargetCard == targetCard);
		BagUnitCardList.Add(targetCard);
		var unitSkillCard = targetCard.UnitSkillCard;
		DeckCardList.Remove(unitSkillCard);

		foreach (var synergy in targetCard.Stat.synergyList)
		{
			synergyNumDict[synergy]--;
		}

		NormalizeLocationInfos();
		RefreshSynergyList();
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
			else
			{
				if (activatedByDeploySynergyDict.TryGetValue(kvp.Key, out var synergy))
				{
					synergy.Level = kvp.Value;
				}
				else
				{
					if (GameDataSystem.Instance.GetGameData<SynergyData>()
					    .GetSynergySpec(kvp.Key).TryGenerateGlobalSynergyInstance(out var newSynergy))
					{
						activatedByDeploySynergyDict[kvp.Key] = newSynergy;
						newSynergy.Initialize();
					}
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
		NoticeSystem.Instance.Subscribe<MapNodeSelectNotice>(OnMapNodeSelect);
		NoticeSystem.Instance.Subscribe<StageClearNotice>(OnStageClear);
	}

	private void OnMapNodeSelect(MapNodeSelectNotice m)
	{
		CurrentSelectedNode = m.TargetInfo;
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<MapNodeSelectNotice>(OnMapNodeSelect);
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