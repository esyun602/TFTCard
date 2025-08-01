using UnityEngine;

public abstract class UnitCardActionData : ScriptableObject
{
	public abstract UnitCardActionBase CreateCardAction();
}