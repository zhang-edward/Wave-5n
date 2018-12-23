using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PawnShopMenu : MonoBehaviour
{
	public const string TIMER_KEY = "PawnShopTimer";
	GameManager gm;

	[Header("ScrollView Settings")]
	public PawnSelectionView pawnSelectionView;
	public RectTransform scrollContent;
	public float contentDistance;

	[Space]
	public Button recruitButton;
	public Button infoButton;
	public ScrollingText infoText;
	public TMP_Text goldReqText;
	public TMP_Text soulsReqText;
	public Animator resourceReqsAnim;
	public TMP_Text refreshTimerText;
	public PawnInfoPanel infoPanel;
	[Header("Audio")]
	public AudioClip onPawnSelectedSound;
	public AudioClip onRecruitFailedSound;
	public AudioClip onRecruitSuccessSound;

	private int selectedPawnCostMoney;
	private int selectedPawnCostSouls;
	private PawnShop pawnShop;
	private PawnIconStandard selectedIcon;
	private Coroutine LerpToContentRoutine;
	private RealtimeTimer refreshTimer;

	public void Init() {
		gm = GameManager.instance;
		pawnShop = new PawnShop(gm.save);
		// Initialize pawn selection view
		pawnShop.OnSaveGameLoaded();
		pawnSelectionView.Init(pawnShop.AvailablePawns.ToArray(), PawnSelectionView.PawnSortMode.Shuffled);
		SetSelectionViewOnClick();
		// Set recruitButton onClick
		recruitButton.onClick.AddListener(TryRecruitHero);
		refreshTimer = RealtimeTimerCounter.instance.GetTimer(TIMER_KEY);
	}

	void Update() {
		refreshTimerText.text = string.Format("Refreshes in {0}h {1}m {2}s",
								   Mathf.Max(refreshTimer.GetHours(), 0).ToString(),
								   Mathf.Max(refreshTimer.GetMinutes(), 0).ToString(),
								   Mathf.Max(refreshTimer.GetSeconds(), 0).ToString());

	}

	void OnEnable() {
		// Reset resourceReqs and buttons
		goldReqText.text = selectedPawnCostMoney.ToString();
		soulsReqText.text = selectedPawnCostSouls.ToString();
		infoButton.interactable = false;
		recruitButton.interactable = false;
		// Reset pawn pool if refresh timer reached 0
		pawnShop.RefreshPawnPool();
		if (refreshTimer.time <= 0) {
			pawnShop.RefreshAvailablePawns();
			pawnSelectionView.UpdatePawnList(pawnShop.AvailablePawns.ToArray());

			float resetTime = RealtimeTimerCounter.KEY_LIST[TIMER_KEY];
			float time = refreshTimer.time % resetTime;
			RealtimeTimerCounter.instance.SetTimer(TIMER_KEY, resetTime + time);
		}
		SetSelectionViewOnClick();
	}

	void OnDisable() {
		DeselectIcon();
		scrollContent.anchoredPosition = new Vector2(0, scrollContent.anchoredPosition.y);
	}

	public void OpenInfoPanel() {
		infoPanel.gameObject.SetActive(true);
		infoPanel.Init(selectedIcon.pawnData);
	}

	private void SelectIcon(PawnIcon icon) {
		if (selectedIcon != null)
			DeselectIcon();
		PawnIconStandard standardIcon = (PawnIconStandard)icon;
		standardIcon.SetHighlight(true, Color.white);
		selectedIcon = standardIcon;
		StartLerpToContent();
		infoText.UpdateText(DataManager.GetHeroData(icon.pawnData.type).heroDescription);
		infoButton.interactable = true;
		recruitButton.interactable = true;
		Formulas.PawnCost(icon.pawnData, out selectedPawnCostMoney, out selectedPawnCostSouls);
		goldReqText.text = selectedPawnCostMoney.ToString();
		soulsReqText.text = selectedPawnCostSouls.ToString();
	}

	private void DeselectIcon() {
		if (selectedIcon == null)
			return;
		selectedIcon.SetHighlight(false, Color.white);
		selectedIcon = null;
		infoButton.interactable = false;
		recruitButton.interactable = false;
		infoText.SetToDefaultText();
	}

	private void TryRecruitHero() {
		// Not enough money
		if (selectedPawnCostMoney > gm.save.money ||
			selectedPawnCostSouls > gm.save.souls) {
			resourceReqsAnim.Play("Warning", -1);
			SoundManager.instance.PlaySingle(onRecruitFailedSound);
		}
		// Not enough room in party
		else if (!gm.save.AddPawn(selectedIcon.pawnData)) {
			gm.DisplayAlert("You can't recruit any more pawns!");
			SoundManager.instance.PlaySingle(onRecruitFailedSound);
		}
		else {
			gm.save.TrySpendMoney(selectedPawnCostMoney);
			gm.save.TrySpendSouls(selectedPawnCostSouls);
			selectedIcon.button.interactable = false;
			pawnShop.RemovePawn(selectedIcon.pawnData);
			DeselectIcon();
			SoundManager.instance.PlaySingle(onRecruitSuccessSound);
		}
	}

	private void StartLerpToContent() {
		if (LerpToContentRoutine != null)
			StopCoroutine(LerpToContentRoutine);
		LerpToContentRoutine = StartCoroutine(LerpToContent());
	}

	private IEnumerator LerpToContent() {
		float dest = pawnSelectionView.IndexOfPawnIcon(selectedIcon) * -contentDistance;
		while (Mathf.Abs(scrollContent.anchoredPosition.x - dest) > 0.05f) {
			// set new position
			float newX = Mathf.Lerp(scrollContent.anchoredPosition.x, dest, Time.deltaTime * 20);
			scrollContent.anchoredPosition = new Vector2(newX, scrollContent.anchoredPosition.y);
			yield return null;
		}
	}

	public void OnDrag() {
		StopCoroutine(LerpToContentRoutine);
	}

	private void SetSelectionViewOnClick() {
		// Set onClick for each pawnIcon
		foreach (PawnIcon p in pawnSelectionView.pawnIcons) {
			PawnIconStandard icon = (PawnIconStandard)p;
			icon.onClick = (pawnIcon) => {
				if (pawnIcon != selectedIcon)
					SelectIcon(pawnIcon);
				else
					DeselectIcon();
				SoundManager.instance.RandomizeSFX(onPawnSelectedSound);
			};
		}
	}
}
