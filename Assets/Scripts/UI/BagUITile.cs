using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class BagUITile : MonoBehaviour
{
	private static Dictionary<(int, int), BagUITile> tileInfos = new();
	
	public static BagUITile GetTargetTile(int row, int col)
	{
		return tileInfos.GetValueOrDefault((row, col));
	}
	
	private bool hasMouse;
	public bool IsOccupied { get; set; }
	[SerializeField] private int row;
	public int Row => row;
	[SerializeField] private int col;
	public int Col => col;

	private void OnEnable()
	{
		tileInfos.Add((row, col), this);
	}
	
	private void OnDisable()
	{
		tileInfos.Remove((row, col));
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}
	
	private void Update()
	{
		if (IsOccupied) return;
		
		if (Vector3.Distance(transform.position, Input.mousePosition) < 70f)
		{
			NoticeSystem.Instance.Publish(new BagUITileHoverNotice(this, HoverType.Enter));
			hasMouse = true;
		}
		else if(hasMouse)
		{
			NoticeSystem.Instance.Publish(new BagUITileHoverNotice(this, HoverType.Exit));
			hasMouse = false;
		}
	}
}