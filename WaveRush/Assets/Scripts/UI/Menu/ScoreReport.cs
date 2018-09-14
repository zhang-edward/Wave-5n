using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class ScoreReport : MonoBehaviour
{
	public IncrementingText moneyText, moneyEarned, soulsText, soulsEarned;
	public ScoreReportData data;

	public void Init(ScoreReportData data)
	{
		this.data = data;
		moneyText.text.text = data.money.ToString();
		moneyEarned.text.text = " +" + data.moneyEarned.ToString();
		soulsText.text.text = data.souls.ToString();
		soulsEarned.text.text = " +" + data.soulsEarned.ToString();
	}

	public void ReportScore()
	{
		moneyText.DisplayNumber(data.money + data.moneyEarned);
		moneyEarned.DisplayNumber(0);
		soulsText.DisplayNumber(data.souls + data.soulsEarned);
		soulsEarned.DisplayNumber(0);
	}

	public void UpdateMoney(int money)
	{
		moneyText.DisplayNumber(money);
	}

	public bool DoneUpdating()
	{
		return moneyText.doneUpdating && moneyEarned.doneUpdating && soulsText.doneUpdating && soulsEarned.doneUpdating;
	}
}
