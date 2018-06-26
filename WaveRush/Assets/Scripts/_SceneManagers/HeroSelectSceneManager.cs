using UnityEngine;

public class HeroSelectSceneManager : MonoBehaviour {

	public static HeroSelectSceneManager instance;
	private GameManager gm;

	public StageSelectMenu stageSelectMenu;
	public HeroSelectMenu heroSelectMenu;
	public int maxPartySize;

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

	void OnEnable() {
		stageSelectMenu.StageIconSelected += UpdateStageSelection;
	}

	private void Init() {
		stageSelectMenu.InitStageSeriesSelectionView();
		heroSelectMenu.Init();
		gm.OnSceneLoaded -= Init;
	}

	private void UpdateStageSelection() {
		heroSelectMenu.SetNumPawnsAllowed(gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex).maxPartySize);
	}
}