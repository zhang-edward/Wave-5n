using System.Collections.Generic;

namespace Quests
{
	/// <summary>
	/// The sub-quest must be completed a number of times before 
	/// </summary>
	[System.Serializable]
	public class QuestModifierMultiple : Quest
	{
		private Quest subQuest;
		private int numTimes;

		public QuestModifierMultiple(GameManager gm, Quest subQuest, int numTimes) : base(gm)
		{
			this.subQuest = subQuest;
			this.numTimes = numTimes;
			questUpdateType = subQuest.questUpdateType;
			maxProgress = numTimes;
		}

		protected override bool CheckCompleted()
		{
			subQuest.UpdateCompletionState(subQuest.questUpdateType);
			if (subQuest.completed)
			{
				progress++;
				subQuest.SetCompleted(false);
			}
			if (progress >= maxProgress)
			{
				return true;
			}
			return false;
		}

		public override string QuestDescription() { return subQuest.QuestDescription() + numTimes + " times"; }
	}
}
