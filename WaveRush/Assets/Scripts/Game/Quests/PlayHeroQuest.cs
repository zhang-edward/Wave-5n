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
			foreach (Pawn p in gm.selectedPawns) {
				if (p.type == type) {
					progress = 1;
					return true;
				}	
			}
			return false;
		}

		public override string QuestDescription() { return string.Format("Play as {0}", type.ToString()); }
	}
}
