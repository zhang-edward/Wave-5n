using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages overall Battle Scene state
/// </summary>
public class BattleSceneManager : MonoBehaviour
{
	public struct PawnMetaData
	{
		public Pawn startingState;
		public int id;
	}

	public static BattleSceneManager instance;
	private GameManager gm;

	public Map map;
	public EnemyManager enemyManager;
	public Player player;
	public GUIManager gui;
	[Header("UI")]
	public StageEndMenu stageEndMenu;
	public DialogueView dialogueView;
	public GameObject stageCompleteOptions;
	public ModalSelectionView stageCompleteModal;
	public Button exitButton;
	public Button continueButton;
	public GameObject pauseButton;
	[Header("Audio")]
	//public AudioClip stageCompleteSound;
	public AudioClip stageCompleteMusic;
	public AudioClip stageDefeatMusic;

	public int moneyEarned { get; private set; }            // money earned in this session
	public int soulsEarned { get; private set; }            // souls earned in this session
	public bool leaveOrContinueOptionOpen { get; private set; }

	//private int[] pawnIds;
	//private Pawn[] startingPawnStates;
	private PawnMetaData[] pawnMetaData;

	public delegate void BattleSceneEvent();
	public BattleSceneEvent OnStageCompleted;

	void Awake()
	{
		// Make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);

		// Do BattleSceneManager-specific initialization which has no external dependencies
		//acquiredPawns = new List<Pawn>();
		gm = GameManager.instance;
		enemyManager.OnStageCompleted += () => { StartCoroutine(StageCompleteRoutine()); };
		gui.OnStageCompletedTextDone += () => { UpdateData(true); };
		player.OnPlayerDied += () => { UpdateData(false); };
		//gm.OnSceneLoaded += Init;
	}

	void Start()
	{
		Init();
	}

	// Init main game environment
	private void Init()
	{
		StartCoroutine(InitRoutine());
	}

	private IEnumerator InitRoutine()
	{
		// Get data from GameManager
		Pawn[] pawns = gm.selectedPawns;
		// DEBUG
		if (gm.debugMode)
		{
			for (int i = 0; i < pawns.Length; i++)
			{
				pawns[i] = new Pawn(gm.selectedPawns[i].type, gm.selectedPawns[i].tier, gm.selectedPawns[i].level);
			}
		}
		// Get stage data
		StageData stage = gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex);

		pawnMetaData = new PawnMetaData[pawns.Length];
		for (int i = 0; i < pawns.Length; i++)
		{
			pawnMetaData[i].id = pawns[i].Id;
			pawnMetaData[i].startingState = new Pawn(pawns[i]);
		}

		// Initialize components
		map.chosenMap = stage.mapType;
		map.GenerateMap();
		player.SetParty(pawns);
		player.SetHero(0);
		gui.partyView.Init(pawns);

		// Do dialogue before starting the game
		if (stage.dialogueSets.Length > 0)
			dialogueView.Init(stage.dialogueSets);
		player.GetComponent<PlayerInput>().enabled = false;
		gui.enemyWaveText.gameObject.SetActive(false);
		while (dialogueView.dialoguePlaying)
			yield return null;

		SoundManager.instance.PlayMusicLoop(map.data.musicLoop, false, map.data.musicIntro);
		player.GetComponent<PlayerInput>().enabled = true;
		gui.enemyWaveText.gameObject.SetActive(true);
		enemyManager.Init(stage);
		gui.DisplayIntroMessage();

		//gm.OnSceneLoaded -= Init;   // Remove the listener because it is only run once per scene
	}

	private IEnumerator StageCompleteRoutine()
	{
		SoundManager.instance.FadeMusic(0f);
		Time.timeScale = 0.2f;
		yield return new WaitForSecondsRealtime(1.0f);
		Time.timeScale = 1f;
	}

	private void UpdateData(bool completedStage)
	{
		// Event Call
		if (completedStage)
		{
			if (OnStageCompleted != null)
				OnStageCompleted();
			if (IsPlayerOnLatestStage())
				gm.UnlockNextStage();
			// Music
			SoundManager.instance.PlayMusicLoop(stageCompleteMusic, true);
		}
		else
			SoundManager.instance.PlayMusicLoop(stageDefeatMusic, true);

		// Generate Score Report Data
		CollectStrayResources();
		ScoreReport.ScoreReportData scoreData = new ScoreReport.ScoreReportData(
			money: gm.save.money,
			moneyEarned: moneyEarned,
			souls: gm.save.souls,
			soulsEarned: soulsEarned,
			bonusMoney: GetBonusMoney()
		);
		gm.save.AddMoney(moneyEarned);
		gm.save.AddSouls(soulsEarned);

		// Generate Pawn EXP Data
		UpdatePawnExperience(completedStage);
		HeroExpMenu.HeroExpMenuData[] expData = new HeroExpMenu.HeroExpMenuData[pawnMetaData.Length];
		for (int i = 0; i < pawnMetaData.Length; i++)
		{
			expData[i] = new HeroExpMenu.HeroExpMenuData(
				startState: pawnMetaData[i].startingState,
				endState: gm.save.GetPawn(pawnMetaData[i].id)
			);
		}

		stageEndMenu.gameObject.SetActive(true);
		stageEndMenu.Init(scoreData, expData, gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex).stageName);
		gm.Save();
	}

	/// <summary>
	/// Updates the experience and level each pawn in the save file based on 
	/// whether the stage was completed or not.
	/// </summary>
	/// <param name="completedStage">Whether the stage was completed</param>
	private void UpdatePawnExperience(bool completedStage)
	{
		// Add exp
		if (completedStage)
		{
			int stagesCompleted = enemyManager.waveNumber / enemyManager.stageData.goalWave;
			int gainedExperience = Formulas.StageExperience(enemyManager.level, enemyManager.waveNumber, enemyManager.stageData.maxPartySize, pawnMetaData.Length);
			for (int i = 0; i < pawnMetaData.Length; i++)
			{
				gm.save.AddExperience(pawnMetaData[i].id, gainedExperience);
			}
		}
		// Subtract exp
		else
		{
			for (int i = 0; i < pawnMetaData.Length; i++)
			{
				Pawn pawn = gm.save.GetPawn(pawnMetaData[i].id);
				int lostExperience = (int)(Formulas.ExperienceFormula(pawn.level, (int)pawn.tier) * 0.2f);
				gm.save.LoseExperience(pawnMetaData[i].id, lostExperience);
			}
		}
	}

	/// <summary>
	/// Collects any active uncollected resource pickups and adds them 
	/// to their corresponding "earned" field 
	/// </summary>
	private void CollectStrayResources()
	{
		List<GameObject> moneyPickups = ObjectPooler.GetObjectPooler(Enemy.POOL_MONEY).GetAllActiveObjects();
		List<GameObject> soulPickups = ObjectPooler.GetObjectPooler(BossEnemy.POOL_SOULS).GetAllActiveObjects();
		int leftoverMoney = 0;
		int leftoverSouls = 0;
		foreach (GameObject o in moneyPickups)
		{
			leftoverMoney += o.GetComponent<MoneyPickup>().value;
		}
		foreach (GameObject o in soulPickups)
		{
			leftoverSouls++;
		}
		AddMoney(leftoverMoney);
		AddSouls(leftoverSouls);
	}

	/// <summary>
	/// Calculates the maximum luck, then calculates bonus money based on the 
	/// amount of money earned.
	/// </summary>
	/// <returns>Bonus money</returns>
	private int GetBonusMoney()
	{
		float maxLuck = 0;
		for (int i = 0; i < pawnMetaData.Length; i++)
		{
			float luck = pawnMetaData[i].startingState.GetStatsArray()[StatData.LUCK];
			if (luck > maxLuck)
				maxLuck = luck;
		}
		return (int)(moneyEarned * (maxLuck + 1f));
	}

	/// <summary>
	/// Checks if the player is currently playing the latest unlocked stage
	/// </summary>
	private bool IsPlayerOnLatestStage()
	{
		return (gm.selectedStageIndex == gm.save.LatestStageIndex &&
				gm.selectedSeriesIndex == gm.save.LatestSeriesIndex);
	}

	/// <summary>
	/// Adds an amount of money to the moneyEarned for this stage
	/// </summary>
	/// <param name="amt">Amount of money to add</param>
	public void AddMoney(int amt)
	{
		moneyEarned += amt;
		gui.UpdateMoney(moneyEarned);
	}

	/// <summary>
	/// Adds an amount of souls to the moneyEarned for this stage
	/// </summary>
	/// <param name="amt">Amount of souls to add</param>
	public void AddSouls(int amt)
	{
		soulsEarned += amt;
		gui.UpdateSouls(soulsEarned);
	}

	// DEBUG
	public void DebugCompleteStage()
	{
		StartCoroutine(StageCompleteRoutine());
		gui.OnStageCompletedText();
	}
}
