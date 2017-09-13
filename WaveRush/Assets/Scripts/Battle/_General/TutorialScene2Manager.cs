using UnityEngine;
using System.Collections;

public class TutorialScene2Manager : MonoBehaviour
{
	private const float TASK_DELAY_INTERVAL = 1.0f;

	public static TutorialScene2Manager instance;
	private GameManager gm;
	[Header("Game")]
	public Map map;
	public EnemyManager enemyManager;
	public Player player;
	public SimpleAnimationPlayer knightCharacter;
	public SimpleAnimationPlayer darkLordCharacter;
	public GameObject[] knightPropCharacters;
	public Transform knightPropsCenterFocusPoint;
	[Header("UI")]
	public GUIManager gui;
	public DialogueView dialogueView;
	public AbilityIconBar abilitiesBar;
	public Animator controlPointer;
	[Header("Data")]
	public DialogueSet[] dialogueSteps;
	public GameObject trappedHeroShardPrefab;

	public MapType mapType;

	private KnightHero knight;

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

		gm.OnSceneLoaded -= Init;   // Remove the listener because it is only run once per scene

		player.input.isInputEnabled = false;
		yield return new WaitForSeconds(TASK_DELAY_INTERVAL);
		// Step 0: Congratulations
		PlayKnightCharDialogue(0);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.ResetFocus();

		// Step 1: What's going on?
		cam.StartShake(3.0f, 0.05f, true, true);
		yield return new WaitForSeconds(3.0f);
		PlayKnightCharDialogue(1);
		while (dialogueView.dialoguePlaying)
		{
			cam.StartShake(0.1f, 0.05f, true, true);
			yield return new WaitForSeconds(0.1f);
		}

		cam.SetFocus(darkLordCharacter.transform);
		yield return new WaitForSeconds(1.0f);
		cam.StartFlashColor(Color.white, 1, 0, 0, 1);
		darkLordCharacter.gameObject.SetActive(true);
		darkLordCharacter.Play();
		yield return new WaitForSeconds(2.0f);

		// Step 2: Dark Lord appears!
		PlayDarkLordCharDialogue(2);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.SetFocus(knightPropsCenterFocusPoint.transform);
		yield return new WaitForSeconds(1.0f);
		cam.StartFlashColor(Color.white, 1, 0, 0, 1);
		foreach(GameObject o in knightPropCharacters)
		{
			o.SetActive(false);
			enemyManager.SpawnEnemy(trappedHeroShardPrefab, o.transform.position);
		}
		yield return new WaitForSeconds(2.0f);

		// Step 3: Dark Lord hahahaha!
		PlayDarkLordCharDialogue(3);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		yield return new WaitForSeconds(1.0f);
		cam.ResetFocus();
		yield return new WaitForSeconds(2.0f);

		// Step 4: Hmm that usually works on all heroes; anyways, farewell
		PlayDarkLordCharDialogue(4);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.StartFlashColor(Color.white, 1, 0, 0, 1);
		darkLordCharacter.gameObject.SetActive(false);
		yield return new WaitForSeconds(1.0f);

		// Step 5: Knight - Dark Lord is weak, so he can't have gone far
		PlayKnightCharDialogue(5);
		yield return new WaitUntil(() => !dialogueView.dialoguePlaying);
		cam.StartShake(3.0f, 0.05f, true, true);
		yield return new WaitForSeconds(3.0f);

		// Step 6: Enemies coming, defend the trapped heroes
		PlayKnightCharDialogue(6);
		while (dialogueView.dialoguePlaying)
		{
			cam.StartShake(0.1f, 0.05f, true, true);
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(1.0f);
		player.input.isInputEnabled = true;

		enemyManager.SpawnEndPortal();
	}

	private void PlayKnightCharDialogue(int step)
	{
		CameraControl.instance.SetFocus(knightCharacter.transform);
		knightCharacter.Play();
		dialogueView.Init(dialogueSteps[step]);
	}

	private void PlayDarkLordCharDialogue(int step)
	{
		CameraControl.instance.SetFocus(darkLordCharacter.transform);
		dialogueView.Init(dialogueSteps[step]);
	}

	private void DisableSpecialAbility()
	{
		abilitiesBar.specialAbilityIcon.gameObject.SetActive(false);
	}

	private void EnableSpecialAbility()
	{
		abilitiesBar.specialAbilityIcon.gameObject.SetActive(true);
	}
}
