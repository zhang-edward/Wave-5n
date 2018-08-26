using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BoxesMenu : MonoBehaviour {

	public const string TIMER_KEY = "FreeBoxTimer";
	public const string SAVEDICT_KEY = "BoxesOpened";
	GameManager gm;

	/** Public serialized members */
	public TMP_Text winChanceText;
	public Sprite openedBoxSprite, unopenedBoxSprite;
	public RectTransform[] boxes;
	[Header("Refresh Menu")]
	public TMP_Text refreshTimerText;
	public TMP_Text instructionsText;
	public Button rewardedVideoAdButton;
	public Button rewardedNoAdButton;
	public AdsManager ads;
	[Header("Highlight Menu")]
	public ReverseMaskHighlight highlighter;
	public TMP_Text moneyCostText;
	public TMP_Text soulsCostText;
	public Animator resourceReqAnim;
	[Header("Box Open Animator")]
	public Animator boxOpenAnim;
	public TMP_Text skinNameText;
	public HeroTypeIcon skinRewardIcon;
	public GameObject reward_heroSkin;
	public GameObject reward_nothing;
	public TMP_Text rewardText;
	/** Private members */
	private int selectedBoxIndex;
	private RealtimeTimer refreshTimer;
	private int boxesOpened; 	// Bitwise representation of which boxes were opened (order matters)
	private bool freeBoxReward;
	/** Properties */
	public float winChance {
		get {
			return 1f / (boxes.Length - numBoxesOpened);
		}
	}
	private int numBoxesOpened { 
		get {
			int ans = 0;
			for (int i = 0; i < boxes.Length; i ++) {
				if (IsBoxOpen(i))
					ans ++;
			}
			return ans;
		}
	}


	void Awake() {
		Init();
	}

	public void Init() {
		gm = GameManager.instance;
		// Initialize timer
		refreshTimer = RealtimeTimerCounter.instance.GetTimer(TIMER_KEY);
		// Load saved value
		gm.save.GetSaveDict(SAVEDICT_KEY, out boxesOpened, 0);
		// Initialize sprites
		RefreshSprites();
		// Set text
		SetWinChanceText();
		instructionsText.text = "Tap on a box to open it!";
		// Initialize ads reward
		ads.OnRewardedVideoAdShown += ClaimFreeBoxReward;
	}

	private void RefreshSprites() {
		if (boxesOpened >= 31) {
			SetBoxesOpened(0);
		}
		for (int i = 0; i < boxes.Length; i ++) {
			if (IsBoxOpen(i)) {
				boxes[i].GetComponent<Image>().sprite = openedBoxSprite;
			}
			else {
				boxes[i].GetComponent<Image>().sprite = unopenedBoxSprite;
			}
		}
	}

	void Update() {
		refreshTimerText.text = string.Format("Free Box: {0}h {1}m {2}s",
			Mathf.Max(refreshTimer.GetHours(), 0).ToString(),
			Mathf.Max(refreshTimer.GetMinutes(), 0).ToString(),
			Mathf.Max(refreshTimer.GetSeconds(), 0).ToString());
		rewardedVideoAdButton.interactable = refreshTimer.time <= 0;
		rewardedNoAdButton.interactable = refreshTimer.time <= 0;
	}

	public void SelectBox(int index) {
		if (IsBoxOpen(index))
			return;
		selectedBoxIndex = index;
		highlighter.gameObject.SetActive(true);
		highlighter.Highlight(boxes[index].position, boxes[index].sizeDelta);
		// Set cost text
		int moneyCost;
		int soulsCost;
		if (freeBoxReward) {
			moneyCost = 0;
			soulsCost = 0;
		}
		else {
			Formulas.BoxCost(out moneyCost, out soulsCost);
		}
		moneyCostText.text = moneyCost.ToString();
		soulsCostText.text = soulsCost.ToString();
	}

	private bool IsBoxOpen(int box) {
		return System.Convert.ToBoolean(boxesOpened & 1 << box);
	}

	public void OpenBox() {
		if (IsBoxOpen(selectedBoxIndex)) {
			// Debug.LogError("Box has already been opened! This shouldn't happen with this check");
			return;
		}
		// Cost
		int moneyCost;
		int soulsCost;
		if (freeBoxReward) {
			moneyCost = 0;
			soulsCost = 0;
			ResetFreeBoxReward();
		}
		else {
			Formulas.BoxCost(out moneyCost, out soulsCost);
		}
		// Try spending currency
		if (gm.save.money >= moneyCost && gm.save.souls >= soulsCost) {
			bool spentMoney = gm.save.TrySpendMoney(moneyCost);
			bool spentSouls = gm.save.TrySpendSouls(soulsCost);
			UnityEngine.Assertions.Assert.IsTrue(spentMoney && spentSouls);
			StartCoroutine(RevealReward());
			// Save game
			gm.Save();
			
			// boxes[selectedBoxIndex].GetComponent<Image>().sprite = openedBoxSprite;
			highlighter.gameObject.SetActive(false);
		}
		else {
			resourceReqAnim.Play("Warning", -1, 0);
		}
	}

	private void SetBoxesOpened(int num) {
		boxesOpened = num;
		gm.save.SetSaveDict(SAVEDICT_KEY, boxesOpened);
	}

	private IEnumerator RevealReward() {
		// Mark box as opened
		SetBoxesOpened(boxesOpened + (1 << selectedBoxIndex));

		// Win or not
		float r = Random.value;
		if (r < winChance) {
			UnlockSkin();
			SetBoxesOpened(0);
		}
		else {
			reward_heroSkin.gameObject.SetActive(false);
			reward_nothing.SetActive(true);
			rewardText.text = "Better luck next time!";
		}

		boxOpenAnim.gameObject.SetActive(true);
		yield return new WaitForEndOfFrame();
		while (boxOpenAnim.GetCurrentAnimatorStateInfo (0).IsName ("BoxOpen"))
			yield return null;
		
		while (!Input.GetMouseButtonDown(0))
			yield return null;
		boxOpenAnim.gameObject.SetActive(false);
		SetWinChanceText();
		RefreshSprites();
	}

	private void UnlockSkin() {
		// Get all unlocked heroes
		List<int> unlockedHeroes = new List<int>();
		for (int i = 0; i < gm.save.UnlockedHeroes.Length; i ++) {
			if (gm.save.UnlockedHeroes[i]) {
				// Tier 1 character skins have a 50% chance of dropping
				if (i % 3 == 0) {
					unlockedHeroes.Add(i);
					unlockedHeroes.Add(i);
				}
				// Tier 2 character skins have a 33% chance of dropping
				else if (i % 3 == 1) {
					unlockedHeroes.Add(i);
				}
				// Tier 3 character skins have a 17% chance of dropping
				unlockedHeroes.Add(i);
			}
		}
		// Choose a random hero
		int heroIndex = unlockedHeroes[Random.Range(0, unlockedHeroes.Count)];
		HeroType heroType = (HeroType)(heroIndex / 3);
		HeroTier heroTier = (HeroTier)(heroIndex % 3);
		// Choose a skin
		AnimationSet[] skins;
		switch (heroTier) {
			case HeroTier.tier1:
				skins = DataManager.GetHeroData(heroType).t1Skins;
				break;
			case HeroTier.tier2:
				skins = DataManager.GetHeroData(heroType).t1Skins;
				break;
			case HeroTier.tier3:
				skins = DataManager.GetHeroData(heroType).t1Skins;
				break;
			default:
				skins = new AnimationSet[0];
				Debug.LogError("Unexpected tier!");
				break;
		}
		int unlockedSkinIndex = Random.Range(1, skins.Length);
		skinRewardIcon.Init(heroType, heroTier, true, unlockedSkinIndex);
		skinNameText.text = skins[unlockedSkinIndex].animationSetName;
		if (gm.save.IsSkinUnlocked(heroType, heroTier, unlockedSkinIndex))
			rewardText.text = "Duplicate!";
		else
			rewardText.text = "New skin unlocked!";
		gm.save.UnlockSkin(heroType, heroTier, unlockedSkinIndex);

		// Play animation
		reward_heroSkin.gameObject.SetActive(true);
		reward_nothing.SetActive(false);
	}

	public void ClaimFreeBoxReward(int id) {
		if (id != 0)
			return;
		freeBoxReward = true;
		instructionsText.text = "Tap on a box to open it for free!";
		instructionsText.CrossFadeColor(Color.yellow, 1.0f, true, true);
		RealtimeTimerCounter.instance.ResetTimer(TIMER_KEY);
	}

	private void ResetFreeBoxReward() {
		freeBoxReward = false;
		instructionsText.CrossFadeColor(Color.white, 1.0f, true, true);
		instructionsText.text = "Tap on a box to open it!";
	}

	/** Helper methods */
	private void SetWinChanceText() {
		winChanceText.text = "Win Chance: " + ((int)(winChance * 100)) + "%!";
	}
}