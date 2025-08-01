using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridSelector
{
#if UNITY_EDITOR
	public bool isEnemyView = false;
#endif
	public int rows = 1;
	public int columns = 5;
	public bool swapView;

	// 선택된 셀 좌표 (row, col)를 저장하는 리스트
	public List<int> triggerCellList = new();
	public List<int> attackCellList = new();
}