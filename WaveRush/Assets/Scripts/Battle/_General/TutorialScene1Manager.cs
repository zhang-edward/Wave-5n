using UnityEngine;
using System.Collections;

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
	public ReverseMaskHighlight highlighter;
	public TMPro.TMP_Text tipText;
	public TMPro.TMP_Text ouchText;
	private int ouchTextIndex;
	[Header("Data")]
	public DialogueSet[] dialogueSteps;
	public GameObject trainingDummyPrefab;
	public GameObject attackingDummyPrefab;
	public AudioClip taskCompleteSound;

	public MapType mapType;

	private KnightHero knight;
	private PlayerHero.InputAction storedOnSwipe, storedOnTap;

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
		Pawn pawn = gm.selectedPawn;
		SoundManager sound = SoundManager.instance;
		CameraControl cam = CameraControl.instance;

		// Initialize components
		map.chosenMap = mapType;
		map.GenerateMap();
		player.Init(pawn);
		knight = (KnightHero)player.hero;
		enemyManager.level = 1;

		SoundManager.instance.PlayMusicLoop(map.data.musicLoop, map.data.musicIntro);

		gm.OnSceneLoaded -= Init;   // Remove the listener because it is only run once per scene


		// Step -2: Swipe
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

		// Step -1: Learn swipe controls
		tutorialTaskView.SetCompleted(false);
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

		// Step 0: Learn tap controls
		tutorialTaskView.SetCompleted(false);
		PlayKnightCharDialogue(0);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.ResetFocus();

		tutorialTaskView.Init("Use your Shield Ability 2 times (0/2)", false);
		//controlPointer.gameObject.SetActive(true);
		//controlPointer.CrossFade("Tap", 0f);
		//abilitiesBar.abilityIcons[1].FlashHighlight(Color.white);
		EnableTap();

		knight.OnKnightShield += IncrementShieldCount;

		// Tip about cooldown timers
		while (knightShieldCount < 1)
			yield return null;
		yield return new WaitForSecondsRealtime(1.0f);
		highlighter.Highlight(abilitiesBar.abilityIcons[1].image.rectTransform.position,
							  abilitiesBar.abilityIcons[1].image.rectTransform.sizeDelta);
		tipText.text = "Keep an eye on your cooldown timers. You can't use an ability while it is cooling down!";
		while (highlighter.gameObject.activeInHierarchy)
			yield return null;

		while (knightShieldCount < 2)
		{
			tutorialTaskView.SetText(string.Format("Use your Shield Ability 2 times ({0}/2)", knightShieldCount));
			yield return null;
		}
		tutorialTaskView.Init("Use your Shield Ability 2 times (2/2)", true);
		sound.PlayUISound(taskCompleteSound);
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);
		abilitiesBar.abilityIcons[1].StopFlashHighlight();
		//controlPointer.gameObject.SetActive(false);
		knightShieldCount = 0;
		knight.OnKnightShield -= IncrementShieldCount;

		// Step 1: Attacking a dummy
		PlayKnightCharDialogue(1);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.ResetFocus();

		tutorialTaskView.Init("Destroy the dummy", false);
		GameObject trainingDummy = enemyManager.SpawnEnemy(trainingDummyPrefab, map.CenterPosition);
		Enemy trainingDummyEnemy = trainingDummy.GetComponentInChildren<Enemy>();
		trainingDummyEnemy.OnEnemyDamaged += SetOuchText;
		ouchText.GetComponent<UIFollow>().Init(trainingDummy.transform, trainingDummyEnemy.healthBarOffset * 1.5f);
		while (trainingDummy.gameObject.activeInHierarchy)
			yield return null;
		tutorialTaskView.SetCompleted(true);
		sound.PlayUISound(taskCompleteSound);
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);

		// Step 2: Learn the parry
		PlayKnightCharDialogue(2);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.ResetFocus();

		//tutorialTaskView.Init("Parry 5 times (0/5)", false);
		Enemy attackingDummy = enemyManager.SpawnEnemy(attackingDummyPrefab, map.CenterPosition + (Vector3.right * 2f)).GetComponentInChildren<Enemy>();
		attackingDummy.invincible = true;
		knight.onParry += IncrementParryCount;
		tutorialTaskView.SetCompleted(false);
		while (parryCount < 5)
		{
			tutorialTaskView.SetText(string.Format("Parry 5 times ({0}/5)", parryCount));
			yield return null;
		}
		tutorialTaskView.Init("Parry 5 times (5/5)", true);
		sound.PlayUISound(taskCompleteSound);
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);
		attackingDummy.invincible = false;
		attackingDummy.GetComponentInChildren<Enemy>().Damage(999);
		parryCount = 0;
		knight.onParry -= IncrementParryCount;

		// Step 3: Finish tutorial
		PlayKnightCharDialogue(3);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		yield return new WaitForSeconds(1.0f);

		GameManager.instance.GoToScene("Tutorial2");
	}

	private void PlayKnightCharDialogue(int step)
	{
		CameraControl.instance.SetFocus(knightCharacter.transform);
		knightCharacter.Play();
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
}
