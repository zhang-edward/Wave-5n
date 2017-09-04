using UnityEngine;
using System.Collections;

public class RealtimeTimer : MonoBehaviour
{
	private string id;
	public float timer { get; private set; }

	public delegate void OnFinishedTimer();
	private OnFinishedTimer onFinishedTimer;

	public void Init(string id, float timer, OnFinishedTimer onFinishedTimer)
	{
		this.id = id;
		this.timer = timer;
		this.onFinishedTimer = onFinishedTimer;
		StartCoroutine(UpdateTimer());
	}

	public void Reset(float timer, OnFinishedTimer onFinishedTimer)
	{
		StopAllCoroutines();
		this.timer = timer;
		this.onFinishedTimer = onFinishedTimer;
		StartCoroutine(UpdateTimer());
	}

	void Update()
	{
		timer -= Time.deltaTime;
	}

	private IEnumerator UpdateTimer()
	{
		while (timer > 0)
			yield return null;
		if (onFinishedTimer != null)
			onFinishedTimer();
	}

	public void SubtractTime(float time)
	{
		timer -= time;
	}
}
