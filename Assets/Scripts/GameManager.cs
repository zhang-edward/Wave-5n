using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public Player player;
	public Map map;

	private List<Enemy> enemies = new List<Enemy>();
	public GameObject[] enemyPrefabs;

	void Update()
	{
		if (NumAliveEnemies() < 5)
		{
			int numEnemies = Random.Range (5, 25);
			for (int i = 0; i < numEnemies; i++)
				SpawnEnemy ();
		}
	}

	public void SpawnEnemy()
	{
		Vector3 randOpenCell = (Vector3)map.OpenCells [Random.Range (0, map.OpenCells.Count)];
		GameObject o = Instantiate (enemyPrefabs [Random.Range (0, enemyPrefabs.Length)]);
		o.transform.SetParent (transform);
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

	private int NumAliveEnemies()
	{
		int count = 0;
		foreach(Enemy e in enemies)
		{
			if (e.health > 0)
				count++;
		}
		return count;
	}
}
