using System.Collections.Generic;

namespace Quests
{
	/// <summary>
	/// One sub-quest must be completed for this quest to be completed
	/// </summary>
	[System.Serializable]
	public class QuestModifierOr : MultiQuest
	{
		public QuestModifierOr(GameManager gm, params Quest[] questList) : base(gm, questList)
		{
		}

		protected override bool CheckCompleted()
		{
			foreach (Quest quest in quests)
			{
				if (quest.completed)
					return true;
			}
			return false;
		}
	}
}
