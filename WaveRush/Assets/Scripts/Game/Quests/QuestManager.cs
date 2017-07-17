using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Quests;

public class QuestManager : MonoBehaviour
{
	private GameManager gm;
	private BattleSceneManager battleSceneManager;
	private EnemyManager enemyManager;

	public const int NUM_QUESTS = 5;

	public List<Quest> quests;

	public void Init()
	{
		gm = GameManager.instance;
		gm.OnSceneLoaded += OnSceneLoaded;
		quests = new List<Quest>();
		QuestModifierJoint quest = new QuestModifierJoint(
			gm,
			new PlayHeroQuest(gm, HeroType.Knight),
			new CompleteStageQuest(gm, 0, 0)
		);
		quests.Add(quest);
	}

	/// <summary>
	/// Adds a quest to the list of quests.
	/// </summary>
	/// <returns><c>true</c>, if quest was added, <c>false</c> otherwise.</returns>
	public bool AddQuest(Quest quest)
	{
		if (quests.Count >= NUM_QUESTS)
			return false;

		quests.Add(quest);
		return true;
	}


	private void OnSceneLoaded()
	{
		if (SceneManager.GetActiveScene().name.Equals(GameManager.BattleSceneName))
		{
			InitBattleSceneListeners();
			UpdateOnBeganStage();
		}
	}

	private void InitBattleSceneListeners()
	{
		battleSceneManager = BattleSceneManager.instance;
		battleSceneManager.OnStageCompleted += UpdateCompletedStage;

		enemyManager = battleSceneManager.enemyManager;
		enemyManager.OnEnemyDefeated += UpdateDefeatedEnemy;
	}

	// ==========
	// Quest Listeners
	// ==========
	private void UpdateOnBeganStage()
	{
		UpdateQuest(Quest.QuestUpdateType.BeganStage);
	}
	
	private void UpdateCompletedStage()
	{
		UpdateQuest(Quest.QuestUpdateType.CompletedStage);
	}
	
	private void UpdateDefeatedEnemy()
	{
		UpdateQuest(Quest.QuestUpdateType.DefeatedEnemy);
	}

	private void UpdateQuest(Quest.QuestUpdateType updateType)
	{
		foreach (Quest quest in quests)
		{
			if (quest as MultiQuest != null)
			{
				MultiQuest multiQuest = (MultiQuest)quest;
				multiQuest.UpdateCompletionState(updateType);
			}
			else
			{
				quest.UpdateCompletionState(updateType);
			}
		}
	}
}
