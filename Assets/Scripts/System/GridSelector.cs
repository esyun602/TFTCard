using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridCellData
{
	public int row;
	public int col;
	// 연결된 오브젝트 (예: GameObject, ScriptableObject 등)
	public UnityEngine.Object linkedObject;

	public GridCellData(int row, int col)
	{
		this.row = row;
		this.col = col;
		linkedObject = null;
	}
}

[Serializable]
public class GridSelector
{
	public int rows = 5;
	public int columns = 5;
    
	// 각 셀의 데이터 저장 (격자 전체 셀을 미리 생성하거나 사용자가 등록할 때마다 추가)
	public List<GridCellData> cells = new List<GridCellData>();
}