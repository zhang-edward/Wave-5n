namespace Quests
{
	[System.Serializable]
	public class PlayLevelHeroQuest : Quest
	{
		private int level;

		public PlayLevelHeroQuest(GameManager gm, int level) : base(gm)
		{
			this.level = level;
			maxProgress = 1;
			questUpdateType = QuestUpdateType.BeganStage;
		}

		protected override bool CheckCompleted()
		{
			foreach (Pawn p in gm.selectedPawns) {
				if (p.level == level)	{
					progress = 1;
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("[Quest] Play a Level {0} Hero", level);
		}
	}
}
