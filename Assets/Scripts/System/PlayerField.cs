
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

public class PlayerField
{
	private List<UnitCardInField> cardObjectInFields;
	private InputBlockFlag blockInput;
	private Transform fieldParent;

	public void Initialize()
	{
		cardObjectInFields = new();
		fieldParent = new GameObject("PlayerField").transform;
		fieldParent.SetParent(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject.transform);
	}
		
	public void UpdateBlockFlags(InputBlockFlag flag)
	{
		blockInput = flag;
		
		foreach (var card in cardObjectInFields)
		{
			card.UpdateBlockInput(blockInput);
		}
	}

	public void AddToField(UnitCardInField target)
	{
		cardObjectInFields.Add(target);
		target.transform.SetParent(fieldParent);
		target.UpdateBlockInput(blockInput);
	}
	
	public void RemoveFromField(UnitCardInField target)
	{
		cardObjectInFields.Remove(target);
	}

	public void Dispose()
	{
		foreach (var cardObject in cardObjectInFields)
		{
			cardObject.Dispose();
		}
	}
	
}