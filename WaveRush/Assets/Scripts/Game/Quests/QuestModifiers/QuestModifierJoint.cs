using System.Collections.Generic;

namespace Quests
{
	/// <summary>
	/// All sub-quests must be completed for this quest to be completed
	/// </summary>
	[System.Serializable]
	public class QuestModifierJoint : MultiQuest
	{
		public QuestModifierJoint(GameManager gm, params Quest[] questList) : base(gm, questList)
		{
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
	}
}
