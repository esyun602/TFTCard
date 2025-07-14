using System.Collections.Generic;
using MessageSystem;

public struct DeployInfo
{
	public DeployInfo(int row, int col, UnitCard targetCard)
	{
		Row = row;
		Col = col;
		TargetCard = targetCard;
	}

	public int Row { get; }
	public int Col { get; }
	public UnitCard TargetCard { get; }
}

public class PlayInfo
{
	public List<UnitCard> BagUnitCardList { get; } = new();
	//todo: fix?
	public List<SkillCard> DeckCardList { get; } = new();
	public List<DeployInfo> FieldDeployLocationInfo { get; } = new();
	public MapInfo CurrentMapInfo { get; set; }
	public MapNodeInfo CurrentSelectedNode { get; private set; }
	//todo: to constant
	public int MaxFieldUnitCard { get; private set; } = 3;
	public int DrawCount { get; private set; } = 5;

	//todo: additional value
	public int MaxEnergy => Constant.DefaultEnergy;
	
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