using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

	public static EventManager instance;

	public delegate void EnemyDamaged(float strength);
	public EnemyDamaged OnEnemyDamaged;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);

		DontDestroyOnLoad (gameObject);
	}
}
