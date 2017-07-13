using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerView : MonoBehaviour
{
	public RealtimeTimer timer;
	public Text text;

	void Update()
	{
		text.text = ParseTimeInSeconds(timer.timer);
	}

	private string ParseTimeInSeconds(float time)
	{
		int hours = (int)(time / 3600f);
		int minutes = (int)((time % 3600) / 60f);
		int seconds = (int)((time % 60));
		string digitHours = "";
		string digitMinutes = "";
		string digitSeconds = "";
		if (hours < 10)
			digitHours = "0";
		if (minutes < 10)
			digitMinutes = "0";
		if (seconds < 10)
			digitSeconds = "0";
			
		return "" + digitHours + hours + ":" + digitMinutes + minutes + ":" + digitSeconds + seconds;
	}

	public bool TimerDone()
	{
		return timer.timer <= 0;
	}
}
