namespace Quests
{
	[System.Serializable]
	public class DefeatEnemyQuest : Quest
	{
		public DefeatEnemyQuest(GameManager gm, int numEnemies) : base(gm)
		{
			maxProgress = numEnemies;
			questUpdateType = QuestUpdateType.DefeatedEnemy;
		}

		protected override bool CheckCompleted()
		{
			progress++;
			if (progress >= maxProgress)
			{
				return true;
			}
			return false;
		}

		public override string QuestDescription() { return string.Format("Defeat {0} Enemies", maxProgress); }
	}
}
