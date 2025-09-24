using System.Collections.Generic;

public class FlowGenData : GameData
{
	private Dictionary<string, FlowGenSpec> flowGenDict;
	
	public override void Initialize()
	{
		flowGenDict = new();
		var stageParams = GameDataSystem.Instance.GameDataParams["FlowData"];
		foreach (var param in stageParams)
		{
			var spec = FlowGenSpec.Create(param);
			flowGenDict[spec.Name] = spec;
		}
	}

	public FlowGenSpec GetFlowSpec(string name)
	{
		return flowGenDict[name];
	}

	public override void Dispose()
	{
		
	}
}