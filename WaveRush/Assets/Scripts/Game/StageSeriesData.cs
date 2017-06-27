﻿﻿﻿using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Stages/StageSeriesData", order = 1)]
public class StageSeriesData : ScriptableObject
{
	public string seriesName;		// the name of this seriesression of stages
	public int index;				// what order this stage is in the collection
	public Sprite icon;         	// icon to be displayed in stage selection
	public StageData[] stages;		// the actual stage data
}
