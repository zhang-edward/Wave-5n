using UnityEngine;
using System.Collections;

public abstract class EnemyStatus : MonoBehaviour
{
	public string statusName;
	protected Enemy enemy;
	public bool permanent;
	public float duration;
	protected float timer;

	public virtual void Init(Enemy enemy)
	{
		this.enemy = enemy;
		timer = duration;
		StartCoroutine ("Effect");
	}

	void Update()
	{
		timer -= Time.deltaTime;
	}

	protected abstract IEnumerator Effect ();
}

