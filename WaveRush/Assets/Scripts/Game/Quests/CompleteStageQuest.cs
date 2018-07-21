namespace Quests
{
	[System.Serializable]
	public class CompleteStageQuest : Quest
	{
		private int series;
		private int stage;

		public CompleteStageQuest(GameManager gm, int series, int stage) : base(gm)
		{
			this.series = series;
			this.stage = stage;
			maxProgress = 1;
			questUpdateType = QuestUpdateType.CompletedStage;
		}

		protected override bool CheckCompleted()
		{
			if (gm.selectedSeriesIndex == series && gm.selectedStageIndex == stage)
			{
				progress = 1;
				return true;
			}
			return false;
		}

		public override string QuestDescription() { return string.Format("Complete Stage {0}-{1}", series + 1, stage + 1); }
	}
}
