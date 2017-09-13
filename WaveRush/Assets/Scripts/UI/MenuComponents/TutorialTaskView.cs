using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialTaskView : MonoBehaviour
{
	public Toggle completedToggle;
	public TMP_Text taskName;

	public void Init(string text, bool completed)
	{
		taskName.text = text;
		completedToggle.isOn = completed;
	}

	public void SetText(string text)
	{
		taskName.text = text;
	}

	public void SetCompleted(bool completed)
	{
		completedToggle.isOn = completed;
	}
}
