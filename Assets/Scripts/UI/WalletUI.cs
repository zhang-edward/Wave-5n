using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WalletUI : MonoBehaviour
{
	public Text text;
	private Wallet wallet;
	private int incrementer;

	void Start()
	{
		wallet = GameManager.instance.wallet;
	}

	void Update()
	{
		if (wallet.moneyEarned > 0)
		{
			if (wallet.moneyEarned > incrementer)
				incrementer++;
			text.text = "" + wallet.money + "<color=#FFA702>+" + incrementer + "</color>";
		}
		else
			text.text = "" + wallet.money;
	}
}

