namespace Quests
{
	[System.Serializable]
	public class PlayHeroQuest : Quest
	{
		private HeroType type;

		public PlayHeroQuest(GameManager gm, HeroType type) : base(gm)
		{
			this.type = type;
			maxProgress = 1;
			questUpdateType = QuestUpdateType.BeganStage;
		}

		protected override bool CheckCompleted()
		{
			if (gm.selectedPawn.type == type)
			{
				progress = 1;
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("[Quest] Play as {0}", type.ToString());
		}
	}
}
