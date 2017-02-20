using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WalletUI : MonoBehaviour
{
	public Text text;
	private Wallet wallet;
	private int moneyEarnedCounter;
	private int moneyCounter;

	void Start()
	{
		wallet = GameManager.instance.wallet;
		StartCoroutine ("UpdateCounters");
		moneyCounter = wallet.money;
	}

	private IEnumerator UpdateCounters()
	{
		while (true)
		{
			// update moneyEarnedCounter
			if (moneyEarnedCounter != wallet.moneyEarned)
			{
				if (Mathf.Abs(wallet.moneyEarned - moneyEarnedCounter) > 50)
					moneyEarnedCounter = (int)Mathf.Lerp (moneyEarnedCounter, wallet.moneyEarned, 0.1f);
				else
					moneyEarnedCounter += ((int)Mathf.Sign (wallet.moneyEarned - moneyEarnedCounter));
			}
			// update moneyCounter
			if (moneyCounter != wallet.money)
			{
				if (Mathf.Abs(wallet.money - moneyCounter) > 50)
					moneyCounter = (int)Mathf.Lerp (moneyCounter, wallet.money, 0.1f);
				else
					moneyCounter += ((int)Mathf.Sign (wallet.money - moneyCounter));
			}

			if (wallet.moneyEarned > 0)
				text.text = "" + moneyCounter + "<color=#FFA702>+" + moneyEarnedCounter + "</color>";
			else
				text.text = "" + moneyCounter;

			yield return new WaitForSeconds (0.01f);
		}
	}
}

