﻿using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Stages/StageSeriesData", order = 1)]
public class StageSeriesData : ScriptableObject
{
	public string seriesName;		// the name of this series of stages
	public Sprite icon;         	// icon to be displayed in stage selection
	public StageData[] stages;		// the actual stage data

	[HideInInspector] public int index;	// what order this series is in the collection
}
