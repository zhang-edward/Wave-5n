using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages counting down realtime timers (timers which run even when game is closed) while in-game.
/// </summary>
public class RealtimeTimerCounter : MonoBehaviour {

	public const string KEY_PREFIX = "RTTimer_";
	/// <summary>
	/// The list of every realtime timer that operates in the game
	/// 
	/// </summary>
	public static readonly Dictionary<string, int> KEY_LIST = new Dictionary<string, int>
	{
		{ PawnShopMenu.TIMER_KEY , 7200 }		// 4 hours = 14400
	};
	public static string LAST_CLOSED_KEY = "TimeLastClosed";
	public static RealtimeTimerCounter instance;

	public GameManager gm;
	public GameObject timerPrefab;

	public Dictionary<string, RealtimeTimer> timers = new Dictionary<string, RealtimeTimer>();

	void Awake() {
		// Singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);
		// Initialize each key in the key list
		foreach (KeyValuePair<string, int> kvp in KEY_LIST) {
			string key = kvp.Key;
			if (PlayerPrefs.HasKey(KEY_PREFIX + key))
				SetTimer(key, Mathf.Min(PlayerPrefs.GetFloat(KEY_PREFIX + key), kvp.Value));
			else
				SetTimer(key, kvp.Value);
		}
		gm.OnAppLoaded += UpdateTimersSinceLastClosed;
		gm.OnAppClosed += SaveTimers;
	}

	void OnDisable() {
		Debug.LogWarning("RealtimeTimerCounter being disabled");
	}

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

	public void SaveTimers() {
		foreach (KeyValuePair<string, int> kvp in KEY_LIST) {
			string key = kvp.Key;
			PlayerPrefs.SetFloat(KEY_PREFIX + key, GetTime(key));
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


	public RealtimeTimer GetTimer(string key) {
		if (timers.ContainsKey(key))
			return timers[key];
		Debug.Log("Couldn't find a timer called " + key);
		return null;
	}

	public float GetTime(string k) {
		return GetTimer(k).time;
	}

	public void ResetTimer(string key) {
		if (timers.ContainsKey(key)) {
			timers[key].Reset(KEY_LIST[key], null);
		}
		else
			Debug.Log("Couldn't find a timer called " + key);
	}

	public void ClearTimers() {
		foreach (KeyValuePair<string, int> kvp in KEY_LIST) {
			string key = kvp.Key + KEY_PREFIX;
			PlayerPrefs.DeleteKey(key);
		}
	}
}
