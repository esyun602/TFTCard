using MessageSystem;

public class MapNodeSelectNotice : Notice
{
	public MapNodeSelectNotice(MapNodeInfo targetInfo)
	{
		TargetInfo = targetInfo;
	}

	public MapNodeInfo TargetInfo { get; private set; }
	
}