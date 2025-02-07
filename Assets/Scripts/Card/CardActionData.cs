using UnityEngine;

public abstract class CardActionData : ScriptableObject
{
	public abstract IAction CreateCardAction();
}