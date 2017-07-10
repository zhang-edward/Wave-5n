using UnityEngine;
using System.Collections;

public class RealtimeTimer : MonoBehaviour
{
	private string id;
	public float timer { get; private set; }

	public void Init(string id, float timer)
	{
		this.id = id;
		this.timer = timer;
		StartCoroutine(UpdateTimer());
	}

	private IEnumerator UpdateTimer()
	{
		while (timer > 0)
		{
			timer -= Time.deltaTime;
			yield return null;
		}
		timer = 0;
	}

	public void SubtractTime(float time)
	{
		timer -= time;
	}
}
