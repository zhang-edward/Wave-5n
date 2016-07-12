using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public Player player;
	public Map map;

	private List<Enemy> enemies;
	public GameObject[] enemyPrefabs;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SpawnEnemy ();
		}
	}

	public void SpawnEnemy()
	{
		Vector3 randOpenCell = (Vector3)map.OpenCells [Random.Range (0, map.OpenCells.Count)];
		GameObject o = Instantiate (enemyPrefabs [Random.Range (0, enemyPrefabs.Length)]);
		if (Random.value < 0.5f)
		{
			o.transform.position = new Vector3 (Random.Range (0, 10), Map.size + 2);
		}
		else
		{
			o.transform.position = new Vector3 (Random.Range (0, 10), -2);

		}

		Enemy e = o.GetComponentInChildren<Enemy> ();
		e.Init (randOpenCell);
		e.player = player.transform;
		enemies.Add (e);
	}
}
