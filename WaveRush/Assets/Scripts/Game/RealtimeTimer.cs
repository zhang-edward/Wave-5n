using UnityEngine;
using System.Collections;

public class RealtimeTimer : MonoBehaviour
{
	private string id;
	public float time { get; private set; }

	public delegate void OnFinishedTimer();
	private OnFinishedTimer onFinishedTimer;

	public void Init(string id, float time, OnFinishedTimer onFinishedTimer)
	{
		this.id = id;
		this.time = time;
		this.onFinishedTimer = onFinishedTimer;
		StartCoroutine(UpdateTimer());
	}

	public void Reset(float timer, OnFinishedTimer onFinishedTimer)
	{
		StopAllCoroutines();
		this.time = timer;
		this.onFinishedTimer = onFinishedTimer;
		StartCoroutine(UpdateTimer());
	}

	void Update()
	{
		time -= Time.deltaTime;
	}

	private IEnumerator UpdateTimer()
	{
		while (time > 0)
			yield return null;
		if (onFinishedTimer != null)
			onFinishedTimer();
	}

	public void SubtractTime(float time)
	{
		this.time -= time;
	}
}
