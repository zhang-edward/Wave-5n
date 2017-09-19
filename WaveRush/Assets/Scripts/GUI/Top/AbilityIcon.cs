using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIcon : MonoBehaviour
{
	public Image image { get; private set; }
	public Image mask;
	public GameObject highlight;

	void Awake()
	{
		image = GetComponent<Image> ();
	}

	public void SetCooldown(float percent)
	{
		if (percent < 0)
			return;
		mask.fillAmount = percent;
	}

	public void FlashHighlight(Color color, float interval = 0.3f)
	{
		StartCoroutine(FlashHighlightRoutine(color, interval));
	}

	public void FlashHighlight(Color color, float time, float interval)
	{
		StartCoroutine(FlashHighlightRoutine(color, interval));
		Invoke("StopFlashHighlight", time);
	}

	public void StopFlashHighlight()
	{
		highlight.SetActive(false);
		StopAllCoroutines();
	}

	private IEnumerator FlashHighlightRoutine(Color color, float interval)
	{
		highlight.GetComponent<Image>().color = color;
		for (;;)
		{
			highlight.SetActive(!highlight.activeInHierarchy);
			yield return new WaitForSeconds(interval);
		}
		highlight.SetActive(false);
	}
}

