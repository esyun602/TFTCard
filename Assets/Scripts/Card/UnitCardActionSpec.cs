using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCardActionSpec
{
	public abstract UnitCardActionBase CreateCardAction();
	public abstract void Initialize(Dictionary<string, object> param);
}