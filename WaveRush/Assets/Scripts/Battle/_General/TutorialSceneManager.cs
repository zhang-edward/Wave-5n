using UnityEngine;
using System.Collections;

public class TutorialSceneManager : MonoBehaviour
{
	public static TutorialSceneManager instance;
	private GameManager gm;

	public Map map;
	public EnemyManager enemyManager;
	public Player player;
	public GUIManager gui;
	public DialogueView dialogueView;
	public AbilityIconBar abilitiesBar;
	public GameObject trainingDummyPrefab;
	public GameObject attackingDummyPrefab;

	public MapType mapType;

	private KnightHero knight;
	private PlayerHero.InputAction storedOnSwipe, storedOnTap;

	private int knightRushCount;
	private int knightShieldCount;
	private int parryCount;
	private bool playerActivatedSpecial;

	void Awake()
	{
		// Make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);

		gm = GameManager.instance;
		gm.OnSceneLoaded += Init;
	}

	// Init main game environment
	private void Init()
	{
		StartCoroutine(InitRoutine());
	}

	private void Restart()
	{
		
	}

	private IEnumerator InitRoutine()
	{
		// Get data from GameManager
		Pawn pawn = gm.selectedPawn;

		// Initialize components
		map.chosenMap = mapType;
		map.GenerateMap();
		player.Init(pawn);
		knight = (KnightHero)player.hero;
		enemyManager.level = 1;

		gm.OnSceneLoaded -= Init;   // Remove the listener because it is only run once per scene

		DisableTap();
		DisableSpecialAbility();

		knight.OnKnightRush += IncrementRushCount;
		yield return new WaitUntil(() => knightRushCount >= 3);
		yield return new WaitForSeconds(0.5f);
		knightRushCount = 0;
		knight.OnKnightRush -= IncrementRushCount;

		EnableTap();

		knight.OnKnightShield += IncrementShieldCount;
		yield return new WaitUntil(() => knightShieldCount >= 3);
		yield return new WaitForSeconds(0.5f);
		knightShieldCount = 0;
		knight.OnKnightShield -= IncrementShieldCount;

		GameObject trainingDummy = enemyManager.SpawnEnemy(trainingDummyPrefab, map.CenterPosition);
		while (trainingDummy.gameObject.activeInHierarchy)
			yield return null;
		yield return new WaitForSeconds(0.5f);

		Enemy attackingDummy = enemyManager.SpawnEnemy(attackingDummyPrefab, map.CenterPosition).GetComponentInChildren<Enemy>();
		attackingDummy.invincible = true;
		knight.onParry += IncrementParryCount;
		yield return new WaitUntil(() => parryCount >= 3);
		yield return new WaitForSeconds(0.5f);
		attackingDummy.invincible = false;
		attackingDummy.GetComponentInChildren<Enemy>().Damage(999);
		parryCount = 0;
		knight.onParry -= IncrementParryCount;
	}

	private void DisableSwipe()
	{
		storedOnSwipe = player.hero.onSwipe;
		player.hero.onSwipe = null;
	}

	private void EnableSwipe()
	{
		player.hero.onSwipe = storedOnSwipe;
	}

	private void DisableTap()
	{
		storedOnTap = player.hero.onTap;
		player.hero.onTap = null;
	}

	private void EnableTap()
	{
		player.hero.onTap = storedOnTap;
	}

	private void DisableSpecialAbility()
	{
		abilitiesBar.specialAbilityIcon.gameObject.SetActive(false);
	}

	private void EnableSpecialAbility()
	{
		abilitiesBar.specialAbilityIcon.gameObject.SetActive(true);
	}

	private void IncrementRushCount()
	{
		knightRushCount++;
	}

	private void IncrementShieldCount()
	{
		knightShieldCount++;
	}

	private void IncrementParryCount()
	{
		parryCount++;
	}
}
