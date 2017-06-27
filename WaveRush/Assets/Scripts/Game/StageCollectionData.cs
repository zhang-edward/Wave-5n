using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Stages/StageCollectionData", order = 1)]
public class StageCollectionData : ScriptableObject
{
	public string collectionName;
	public StageSeriesData[] series;

	void Awake()
	{
		for (int i = 0; i < series.Length; i ++)
		{
			series[i].index = i;
		}
	}
}
