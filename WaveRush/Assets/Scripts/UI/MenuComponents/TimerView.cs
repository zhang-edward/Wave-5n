using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerView : MonoBehaviour
{
	public enum ViewType
	{
		Digital_HMS,
		Words_DHMS
	}

	public RealtimeTimer timer { get; set; }
	public TMPro.TMP_Text text;

	void Update()
	{
		text.text = Compose(timer.time);
	}

	private string Compose(float time)
	{
		int hours = (int)(time / 3600f);
		int minutes = (int)((time % 3600) / 60f);
		int seconds = (int)((time % 60));
		string digitHours = "";
		string digitMinutes = "";
		string digitSeconds = "";
		if (time <= 0)
		{
			hours = 0;
			minutes = 0;
			seconds = 0;
		}
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
		return timer.time <= 0;
	}
}
