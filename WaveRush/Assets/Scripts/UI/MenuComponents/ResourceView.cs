using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ResourceView : MonoBehaviour
{
	private Wallet wallet;
	public IncrementingText text;
	public enum ResourceType {
		Money,
		Souls
	}
	public ResourceType resourceType;

	void Start()
	{
		wallet = GameManager.instance.wallet;
		if (resourceType == ResourceType.Money)
			text.GetComponent<TMP_Text>().text = wallet.money.ToString();
		if (resourceType == ResourceType.Souls)
			text.GetComponent<TMP_Text>().text = wallet.souls.ToString();

	}

	void Update()
	{
		if (resourceType == ResourceType.Money)
			text.DisplayNumber(wallet.money);
		if (resourceType == ResourceType.Souls)
			text.DisplayNumber(wallet.souls);
	}
}
