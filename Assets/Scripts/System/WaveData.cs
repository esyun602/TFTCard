using System;
using System.Collections.Generic;
using UnityEngine;

//어떤 카드가 어디, 어느 타이밍에 스폰되는가? <-얘도 턴 시스템에 넣는게 낫나?
//어디: 미리 위치를 지정해두고
//어느 타이밍:

[Serializable]
public class WaveData
{
	public List<GridSelector> WaveInfoList;
}