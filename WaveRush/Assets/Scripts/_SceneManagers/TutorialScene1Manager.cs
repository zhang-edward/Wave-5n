using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialScene1Manager : MonoBehaviour
{
	private const float TASK_DELAY_INTERVAL = 1.0f;

	public static TutorialScene1Manager instance;
	private GameManager gm;
	[Header("Game")]
	public Map map;
	public EnemyManager enemyManager;
	public Player player;
	public SimpleAnimationPlayer knightCharacter;
	[Header("UI")]
	public GUIManager gui;
	public DialogueView dialogueView;
	public AbilityIconBar abilitiesBar;
	public TutorialTaskView tutorialTaskView;
	//public Animator controlPointer;
	public TMPro.TMP_Text ouchText;
	private int ouchTextIndex;
	[Header("Tip UI")]
	public ReverseMaskHighlight highlighter;
	public ScrollingText tipText;
	public GameObject tipButton;
	public RectTransform pauseButton;		// Highlighted in tip
	public RectTransform pauseMenuPawnIcon; // Highlighted in tip
	public GameObject pausePawnInfoPanel;	// Used in tip
	[Header("Data")]
	public DialogueSet[] dialogueSteps;
	public GameObject trainingDummyPrefab;
	public GameObject attackingDummyPrefab;
	public AudioClip taskCompleteSound;

	public MapType mapType;

	private KnightHero knight;
	private PlayerHero.DirectionalInputAction storedOnSwipe, storedOnTap;
	private List<Enemy> dummies = new List<Enemy>();
	private int knightRushCount;
	private int knightShieldCount;
	private int parryCount;

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
		StartCoroutine(TutorialScene());
	}

	private void Restart()
	{
		
	}

	private IEnumerator TutorialScene()
	{
		// Get data from GameManager
		Pawn pawn = new Pawn(HeroType.Knight, HeroTier.tier1);
		GameManager.instance.selectedPawns = new Pawn[1];
		GameManager.instance.selectedPawns[0] = pawn;
		SoundManager sound = SoundManager.instance;
		CameraControl cam = CameraControl.instance;

		// Initialize components
		map.chosenMap = mapType;
		map.GenerateMap();
		player.SetParty(pawn);
		player.SetHero(0);
		knight = (KnightHero)player.hero;
		enemyManager.level = 1;

		SoundManager.instance.PlayMusicLoop(map.data.musicLoop, false, map.data.musicIntro);

		gm.OnSceneLoaded -= Init;   // Remove the listener because it is only run once per scene


		/** Step -2: Swipe */
		DisableTap();
		DisableSpecialAbility();
		tutorialTaskView.Init("Swipe to use your Rush Ability", false);

		knight.OnKnightRush += IncrementRushCount;
		while (knightRushCount < 1)
			yield return null;
		tutorialTaskView.SetCompleted(true);
		sound.PlayUISound(taskCompleteSound);
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);
		knightRushCount = 0;

		/** Step -1: Learn swipe controls */
		tutorialTaskView.SetCompleted(false);
		// Spawn training dummies
		for (int i = 0; i <= 1; i ++) {
			for (int j = 0; j <= 1; j ++) {
				dummies.Add(enemyManager.SpawnEnemy(trainingDummyPrefab, map.CenterPosition + new Vector3(i * 1.5f, j * 1.5f)).GetComponentInChildren<Enemy>());
			}
		}
		while (knightRushCount < 3)
		{
			tutorialTaskView.SetText(string.Format("Use your Rush Ability 3 times ({0}/3)", knightRushCount));
			yield return null;
		}
		tutorialTaskView.Init("Use your Rush Ability 3 times (3/3)", true);
		sound.PlayUISound(taskCompleteSound);
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);
		knightRushCount = 0;
		abilitiesBar.abilityIcons[0].StopFlashHighlight();
		knight.OnKnightRush -= IncrementRushCount;

		/** Step 0: Learn tap controls */
		tutorialTaskView.SetCompleted(false);
		PlayKnightCharDialogue(0);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.ResetFocus();

		tutorialTaskView.Init("Use your Shield Ability (0/2)", false);
		//controlPointer.gameObject.SetActive(true);
		//controlPointer.CrossFade("Tap", 0f);
		//abilitiesBar.abilityIcons[1].FlashHighlight(Color.white);
		EnableTap();

		knight.OnKnightShield += IncrementShieldCount;

		/** Tip about cooldown timers */
		while (knightShieldCount < 2) {
			tutorialTaskView.SetText(string.Format("Use your Shield Ability ({0}/2)", knightShieldCount));
			yield return null;
		}
		tutorialTaskView.Init("Use your Shield Ability (2/2)", true);
		yield return new WaitForSecondsRealtime(1.0f);
		highlighter.Highlight(abilitiesBar.abilityIcons[1].image.rectTransform.position,
							  abilitiesBar.abilityIcons[1].image.rectTransform.sizeDelta);
		tipText.UpdateText("Keep an eye on your cooldown timers. You can't use an ability while it is cooling down!");
		while (highlighter.gameObject.activeInHierarchy)
			yield return null;
		abilitiesBar.abilityIcons[1].StopFlashHighlight();
		sound.PlayUISound(taskCompleteSound);
		foreach (Enemy dummy in dummies) {
			dummy.Damage(999, null);
		}
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);
		//controlPointer.gameObject.SetActive(false);
		knightShieldCount = 0;
		knight.OnKnightShield -= IncrementShieldCount;

		/** Step 1: Attacking a dummy */
		PlayKnightCharDialogue(1);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.ResetFocus();

		tutorialTaskView.Init("Destroy the dummy", false);
		GameObject trainingDummy = enemyManager.SpawnEnemy(trainingDummyPrefab, map.CenterPosition);
		Enemy trainingDummyEnemy = trainingDummy.GetComponentInChildren<Enemy>();
		trainingDummyEnemy.OnEnemyDamaged += SetOuchText;
		ouchText.GetComponent<UIFollow>().Init(trainingDummy.transform, trainingDummyEnemy.healthBarPos * 1.5f);
		while (trainingDummy.gameObject.activeInHierarchy)
			yield return null;
		tutorialTaskView.SetCompleted(true);
		sound.PlayUISound(taskCompleteSound);
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);

		/** Step 2: Learn the parry */
		PlayKnightCharDialogue(2);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.ResetFocus();

		//tutorialTaskView.Init("Parry 5 times (0/5)", false);
		Enemy attackingDummy = enemyManager.SpawnEnemy(attackingDummyPrefab, map.CenterPosition + (Vector3.right * 2f)).GetComponentInChildren<Enemy>();
		attackingDummy.invincible = true;
		knight.onParrySuccess += IncrementParryCount;
		tutorialTaskView.SetCompleted(false);
		while (parryCount < 2)
		{
			tutorialTaskView.SetText(string.Format("Parry 2 times ({0}/2)", parryCount));
			yield return null;
		}
		tutorialTaskView.Init("Parry 2 times (2/2)", true);
		sound.PlayUISound(taskCompleteSound);
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);
		attackingDummy.invincible = false;
		attackingDummy.Damage(999, null);
		parryCount = 0;
		knight.onParrySuccess -= IncrementParryCount;
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);

		/** Tip to learn about the hero info panel */
		highlighter.Highlight(pauseButton.position, pauseButton.sizeDelta);
		tipButton.SetActive(false);
		tipText.UpdateText("You can learn how to use a character by viewing their character card. Do this now by pausing the game.");
		while (!pauseMenuPawnIcon.gameObject.activeInHierarchy)		// Wait until the player pauses
			yield return null;

		highlighter.Highlight(pauseMenuPawnIcon.position, pauseMenuPawnIcon.sizeDelta);
		tipText.UpdateText("Tap the info card to open up the character card for your hero.");
		while (!pausePawnInfoPanel.gameObject.activeInHierarchy)	// Wait until the player opens the info panel	
			yield return null;

		highlighter.mask.gameObject.SetActive(false);
		tipText.UpdateText("You can view your hero's abilities and powers here. Tap the icons to learn more about each item.");
		tipButton.SetActive(true);
		while (pauseMenuPawnIcon.gameObject.activeInHierarchy)		// Wait until the player unpauses
			yield return null;
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);

		/** Step 3: Finish tutorial */
		PlayKnightCharDialogue(3);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		yield return new WaitForSeconds(1.0f);

		PlayerPrefs.SetInt(SaveGame.TUTORIAL_COMPLETE_KEY, 1);
		GameManager.instance.GoToScene("MainMenu");
	}

	private void PlayKnightCharDialogue(int step)
	{
		CameraControl.instance.SetFocus(knightCharacter.transform);
		//knightCharacter.Play();
		dialogueView.Init(dialogueSteps[step]);
	}

	private void DisableSwipe()
	{
		storedOnSwipe = player.hero.onDragRelease;
		player.hero.onDragRelease = null;
	}

	private void EnableSwipe()
	{
		player.hero.onDragRelease = storedOnSwipe;
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

	private void SetOuchText(int num)
	{
		CancelInvoke();
		ouchText.gameObject.SetActive(true);
		ouchTextIndex++;
		switch (ouchTextIndex)
		{
			case (1):
				ouchText.text = "Ow";
				break;
			case (2):
				ouchText.text = "Ouch";
				break;
			case (3):
				ouchText.text = "Please stop";
				break;
			case (4):
				ouchText.text = "I'm in so much pain";
				break;
			case (5):
				ouchText.text = "The light";
				break;
			case (6):
				ouchText.text = "I see the light...";
				break;
		}
		Invoke("DisableOuchText", 2.0f);
	}

	private void DisableOuchText()
	{
		ouchText.gameObject.SetActive(false);
	}

	public void SkipTutorial() {
		PlayerPrefs.SetInt(SaveGame.TUTORIAL_COMPLETE_KEY, 1);
		GameManager.instance.GoToScene("MainMenu");
	}
}
