using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages counting down realtime timers (timers which run even when game is closed) while in-game.
/// </summary>
public class RealtimeTimerCounter : MonoBehaviour
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

	public void SetTimer(string key, float time, RealtimeTimer.OnFinishedTimer onFinishCallback = null)
	{
		//print("RealtimeTimerCounter: Setting timer with key:" + key + ", time" + time);
		if (timers.ContainsKey(key))
			timers[key].Reset(time, onFinishCallback);
		else
		{
			GameObject o = Instantiate(timerPrefab);
			o.transform.SetParent(this.transform);
			RealtimeTimer timer = o.GetComponent<RealtimeTimer>();
			timer.Init(key, time, onFinishCallback);
			timers.Add(key, timer);
		}
	}

	public RealtimeTimer GetTimer(string key)
	{
		if (timers.ContainsKey(key))
			return timers[key];
		return null;
	}
}
