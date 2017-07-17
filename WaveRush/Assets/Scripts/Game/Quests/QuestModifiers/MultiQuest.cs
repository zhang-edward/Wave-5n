using System.Collections.Generic;

namespace Quests
{
	/// <summary>
	/// All sub-quests must be completed for this quest to be completed
	/// </summary>
	[System.Serializable]
	public class MultiQuest : Quest
	{
		public List<Quest> quests { get; private set; }

		public MultiQuest(GameManager gm, params Quest[] questList) : base(gm)
		{
			quests = new List<Quest>();
			foreach (Quest quest in questList)
				quests.Add(quest);
		}

		protected override bool CheckCompleted()
		{
			foreach (Quest quest in quests)
			{
				if (!quest.completed)
					return false;
			}
			return true;
		}

		public override void UpdateCompletionState(Quest.QuestUpdateType updateType)
		{
			foreach (Quest quest in quests)
			{
				quest.UpdateCompletionState(updateType);
			}
			completed = CheckCompleted();
		}
	}
}
