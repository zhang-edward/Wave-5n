using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIcon : MonoBehaviour
{
	public Image image { get; private set; }
	public Slider slider;

	void Awake()
	{
		image = GetComponent<Image> ();
		slider.maxValue = 1;
		slider.minValue = 0;
	}

	public void SetCooldown(float percent)
	{
		if (percent < 0)
			return;
		slider.value = percent;
	}
}

