using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class WaveCellInfo
{
	public int row;
	public int col;
	public CardSpec cardObject;

	public WaveCellInfo(int row, int col)
	{
		this.row = row;
		this.col = col;
		cardObject = null;
	}
}

[Serializable]
public class WaveGrid
{
	public int rows = 5;
	public int columns = 5;
    
	// 각 셀의 데이터 저장 (격자 전체 셀을 미리 생성하거나 사용자가 등록할 때마다 추가)
	public List<WaveCellInfo> cells = new List<WaveCellInfo>();
}