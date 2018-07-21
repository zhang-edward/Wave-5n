namespace Quests
{
	[System.Serializable]
	public abstract class Quest
	{
		public GameManager gm;
		public enum QuestUpdateType		// describes when to check for completion of the quest
		{
			BeganStage,
			CompletedStage,
			DefeatedEnemy,
			PawnsChanged,
		}
		public int maxProgress { get; protected set; }
		public int progress { get; protected set; }
		public bool completed { get; protected set; }
		public QuestUpdateType questUpdateType { get; protected set; }

		public Quest(GameManager gm)
		{
			this.gm = gm;
		}

		public virtual void UpdateCompletionState(QuestUpdateType updateType)
		{
			if (updateType == questUpdateType)
			{
				completed = CheckCompleted();
				UnityEngine.Debug.Log(string.Format("{0}, [CompleteState]: {1}", ToString(), completed));
			}
		}

		protected abstract bool CheckCompleted();
		public abstract string QuestDescription();
		public override string ToString() { return "[Quest] " + QuestDescription(); }

		public void SetCompleted(bool completed)
		{
			this.completed = completed;
		}

	}
}
