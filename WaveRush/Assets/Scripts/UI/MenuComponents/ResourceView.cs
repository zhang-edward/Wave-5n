using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ResourceView : MonoBehaviour
{
	private SaveModifier save;
	public IncrementingText text;
	public enum ResourceType {
		Money,
		Souls
	}
	public ResourceType resourceType;

	void Start()
	{
		save = GameManager.instance.save;
		if (resourceType == ResourceType.Money)
			text.GetComponent<TMP_Text>().text = save.money.ToString();
		if (resourceType == ResourceType.Souls)
			text.GetComponent<TMP_Text>().text = save.souls.ToString();

	}

	void Update() {
		if (save == null)
			return;

		if (resourceType == ResourceType.Money)
			text.DisplayNumber(save.money);
		if (resourceType == ResourceType.Souls)
			text.DisplayNumber(save.souls);
	}
}
