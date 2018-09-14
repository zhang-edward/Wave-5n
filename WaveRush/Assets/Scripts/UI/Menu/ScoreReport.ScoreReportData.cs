public partial class ScoreReport
{
	public struct ScoreReportData
	{
		public int money, moneyEarned;
		public int souls, soulsEarned;
		public int bonusMoney;

		public ScoreReportData(int money, int moneyEarned, int souls, int soulsEarned, int bonusMoney)
		{
			this.money = money;
			this.moneyEarned = moneyEarned;
			this.souls = souls;
			this.soulsEarned = soulsEarned;
			this.bonusMoney = bonusMoney;
		}
	}
}
