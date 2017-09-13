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

	public void FlashHighlight()
	{
		StartCoroutine(FlashHighlightRoutine());
	}

	public void StopFlashHighlight()
	{
		highlight.SetActive(false);
		StopAllCoroutines();
	}

	private IEnumerator FlashHighlightRoutine()
	{
		for (;;)
		{
			highlight.SetActive(!highlight.activeInHierarchy);
			yield return new WaitForSeconds(0.3f);
		}
	}
}

