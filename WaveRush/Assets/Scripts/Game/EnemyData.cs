using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Enemy/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
	public MapType mapType;
	public List<GameObject> enemyPrefabs1;  // set of enemies for wave 1-5
	public List<GameObject> enemyPrefabs2;  // set of enemies for waves 5+
	public List<GameObject> bossPrefabs;    // bosses
}
