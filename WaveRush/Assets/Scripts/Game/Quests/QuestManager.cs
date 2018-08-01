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

	public List<Quest> publicQuests;		// These are visible in the game and give the player money rewards
	public Quest[] heroUnlockQuests;	// These are invisible and unlock certain hero types upon completion

	void Start() {
		gm = GameManager.instance;
		gm.OnSceneLoaded += OnSceneLoaded;
		publicQuests = new List<Quest>();
		heroUnlockQuests = new Quest[System.Enum.GetValues(typeof(HeroType)).Length * 3];
		
		QuestModifierJoint quest = new QuestModifierJoint(
			gm,
			new PlayHeroQuest(gm, HeroType.Knight),
			new CompleteStageQuest(gm, 0, 0)
		);
		publicQuests.Add(quest);
		GetHeroUnlockQuests();
	}

	private void GetHeroUnlockQuests() {
		for (int i = 0; i < heroUnlockQuests.Length; i ++) {
			// If this hero was already unlocked, don't bother adding the unlock quest
			if (gm.save.UnlockedHeroes[i])
				continue;
			int type = i / 3;
			int tier = i % 3;
			heroUnlockQuests[i] = DataManager.GetPlayerHero((HeroType)type).GetComponent<PlayerHero>().GetUnlockQuest(((HeroTier)tier));
		}
	}

	/// <summary>
	/// Adds a quest to the list of quests.
	/// </summary>
	/// <returns><c>true</c>, if quest was added, <c>false</c> otherwise.</returns>
	public bool AddQuest(Quest quest)
	{
		if (publicQuests.Count >= NUM_QUESTS)
			return false;

		publicQuests.Add(quest);
		return true;
	}


	private void OnSceneLoaded()
	{
		if (SceneManager.GetActiveScene().name.Equals(GameManager.SCENE_BATTLE))
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
		// Public quests
		foreach (Quest quest in publicQuests) {
			if (quest as MultiQuest != null) {
				MultiQuest multiQuest = (MultiQuest)quest;
				multiQuest.UpdateCompletionState(updateType);
			}
			else {
				quest.UpdateCompletionState(updateType);
			}
		}

		// Unlock hero quests
		for (int i = 0; i < heroUnlockQuests.Length; i ++) {
			Quest quest = heroUnlockQuests[i];
			// Quest == null means this hero was already unlocked
			if (quest == null)
				continue;
			// Check unlock status
			if (quest as MultiQuest != null) {
				MultiQuest multiQuest = (MultiQuest)quest;
				multiQuest.UpdateCompletionState(updateType);
			}
			else {
				quest.UpdateCompletionState(updateType);
				if (quest.completed) {
					gm.save.UnlockHero(i);
					gm.heroJustUnlocked[i] = true;
					print(string.Format("Unlocked hero {0}, tier {1}.", (HeroType)(i / 3), (HeroTier)(i % 3)));
				}
			}
		}

	}
}
