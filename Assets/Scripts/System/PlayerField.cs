
using System.Collections.Generic;
using MessageSystem;

public class PlayerField
{
	private List<BattleCardObjectInField> cardObjectInFields;
	private InputBlockFlag blockInput;

	public void Initialize()
	{
		cardObjectInFields = new();
	}
		
	public void UpdateBlockFlags(InputBlockFlag flag)
	{
		blockInput = flag;
		
		foreach (var card in cardObjectInFields)
		{
			card.UpdateBlockInput(blockInput);
		}
	}

	public void AddToField(BattleCardObjectInField target)
	{
		cardObjectInFields.Add(target);
		target.UpdateBlockInput(blockInput);
	}
	
	public void RemoveFromField(BattleCardObjectInField target)
	{
		cardObjectInFields.Remove(target);
	}

}