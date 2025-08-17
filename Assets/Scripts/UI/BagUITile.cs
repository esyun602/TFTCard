using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class BagUITile : MonoBehaviour
{
	private bool hasMouse;

	private bool isOccupied;
	public bool IsOccupied
	{
		get=>isOccupied;
		set
		{
			if (value && hasMouse)
			{
				NoticeSystem.Instance.Publish(new BagUITileHoverNotice(this, HoverType.Exit));
				hasMouse = false;
			}

			isOccupied = value;
		}
	}

	[SerializeField] private int row;
	public int Row => row;
	[SerializeField] private int col;
	public int Col => col;

	public Vector3 GetPosition()
	{
		return transform.position;
	}
	
	private void Update()
	{
		if (IsOccupied) return;
		
		if (!hasMouse && Vector3.Distance(transform.position, Input.mousePosition) < 70f)
		{
			NoticeSystem.Instance.Publish(new BagUITileHoverNotice(this, HoverType.Enter));
			hasMouse = true;
		}
		else if(hasMouse && Vector3.Distance(transform.position, Input.mousePosition) >= 70f)
		{
			NoticeSystem.Instance.Publish(new BagUITileHoverNotice(this, HoverType.Exit));
			hasMouse = false;
		}
	}
}