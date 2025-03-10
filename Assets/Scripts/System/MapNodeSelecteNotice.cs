using MessageSystem;

public class MapNodeSelectNotice : Notice
{
	public MapNodeSelectNotice(StageSpec targetSpec)
	{
		TargetSpec = targetSpec;
	}

	public StageSpec TargetSpec { get; private set; }
	
}