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

	void Update()
	{
		
	}

	private IEnumerator UpdateCounters()
	{
		while (true)
		{
			// update moneyEarnedCounter
			if (wallet.moneyEarned > moneyEarnedCounter)
				moneyEarnedCounter++;
			else if (wallet.moneyEarned < moneyEarnedCounter)
				moneyEarnedCounter--;
			// update moneyCounter
			if (wallet.money > moneyCounter)
				moneyCounter++;
			else if (wallet.money < moneyCounter)
				moneyCounter--;

			if (wallet.moneyEarned > 0)
				text.text = "" + moneyCounter + "<color=#FFA702>+" + moneyEarnedCounter + "</color>";
			else
				text.text = "" + moneyCounter;

			yield return new WaitForSeconds (0.01f);
		}
	}
}

