using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridSelector
{
	public int rows = 5;
	public int columns = 5;
	public bool isAbsolute;
    
	// 선택된 셀 좌표 (row, col)를 저장하는 리스트
	public List<GridCell> selectedCells = new List<GridCell>();
}

[Serializable]
public struct GridCell
{
	public int row;
	public int col;

	public GridCell(int row, int col)
	{
		this.row = row;
		this.col = col;
	}
}