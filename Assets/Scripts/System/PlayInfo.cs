using System.Collections.Generic;
using MessageSystem;

public class PlayInfo
{
	public List<Card> CardList { get; } = new();
	public MapInfo CurrentMapInfo { get; set; }
	public MapNodeInfo CurrentSelectedNode { get; private set; }

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