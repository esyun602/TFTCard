using System.Collections.Generic;

public class ShopStageSpec : StageSpec
{
	public override IStage InstantiateStage()
	{
		return new ShopStage(this);
	}

	public override StageType StageType => StageType.EventStage;

	protected override void Initialize(Dictionary<string, object> param)
	{
	}
}