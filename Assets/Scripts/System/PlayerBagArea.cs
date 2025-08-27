using UnityEngine;

public class PlayerBagArea
{
	private Transform leftTop;
	private Transform rightTop;
	private float verticalPlacing;

	public PlayerBagArea(Transform lt, Transform rt, float vPlacing)
	{
		leftTop = lt;
		rightTop = rt;
		verticalPlacing = vPlacing;

		InitializeCards();
	}

	private void InitializeCards()
	{
		var prefab = Resources.Load<BagUICard>("BagUICard");
		foreach (var card in Game.Instance.GetPlayer().CurrentPlayInfo.TotalDeckCards)
		{
			
			var cardSprite = card.CardStaticSpec.CardResource;
			
		}
	}
	
	
	
	
}