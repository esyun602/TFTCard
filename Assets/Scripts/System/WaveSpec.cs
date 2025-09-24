using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveCellInfo
{
	public int Row { get; private set; }
	public int Col { get; private set; }
	public string UnitCardName { get; private set; }

	public WaveCellInfo(int row, int col, string unitCardName)
	{
		this.Row = row;
		this.Col = col;
		UnitCardName = unitCardName;
	}
}

public class WaveSpec
{
	/*public int rows = 3;
	public int columns = 4;*/
	public int PrepareTurn { get; private set; }
	public string Name { get; private set; }
	
	public static WaveSpec Create(Dictionary<string, object> waveParam)
	{
		var spec = new WaveSpec();
		spec.Name = waveParam.GetString(nameof(Name));
		spec.cells = new();
		var cellInfoList = waveParam.GetObjectArray(nameof(CellList));
		foreach (var cellInfo in cellInfoList)
		{
			var instance = new WaveCellInfo(cellInfo.GetInt("Row"), cellInfo.GetInt("Col"), cellInfo.GetString("UnitCardName"));
			spec.cells.Add(instance);
		}

		spec.PrepareTurn = waveParam.GetInt(nameof(PrepareTurn));

		return spec;
	}

	public static WaveSpec CreateForCarryOver()
	{
		var spec = new WaveSpec();
		spec.Name = "CarryOver";
		spec.cells = new();
		//todo: fix
		spec.PrepareTurn = 3;

		return spec;
	}

	private WaveSpec()
	{
		
	}
	
	public List<WaveCellInfo> CellList
	{
		get 
		{
			cells.Sort((x, y) => x.Col == y.Col ? x.Row.CompareTo(y.Row) : x.Col.CompareTo(y.Col));
			return cells;
		}
	}

	private List<WaveCellInfo> cells;
}