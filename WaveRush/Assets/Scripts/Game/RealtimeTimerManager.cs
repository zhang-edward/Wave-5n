using UnityEngine;
using System.Collections.Generic;
using System;

public class RealtimeTimerManager : MonoBehaviour
{
	public static string LAST_CLOSED_KEY = "timeLastClosedApp";
	public Dictionary<string, RealtimeTimer> timers = new Dictionary<string, RealtimeTimer>();

	public GameObject timerPrefab;


	public void UpdateTimersSinceLastClosed()
	{
		DateTime lastOpenTime = DateTime.Parse(PlayerPrefs.GetString(LAST_CLOSED_KEY, DateTime.Now.ToString()));
		TimeSpan timeSpan = (DateTime.Now - lastOpenTime);

		foreach(KeyValuePair<string, RealtimeTimer> kvp in timers)
		{
			RealtimeTimer timer = kvp.Value;
			timer.SubtractTime((float)timeSpan.TotalSeconds);
		}
	}

	public void AddTimer(string key, float time)
	{
		GameObject o = Instantiate(timerPrefab);
		o.transform.SetParent(this.transform);
		RealtimeTimer timer = o.GetComponent<RealtimeTimer>();
		timer.Init(key, time);
		timers.Add(key, timer);
	}

	public RealtimeTimer GetTimer(string key)
	{
		return timers[key];
	}
}
