using UnityEngine;

public class LockedMenuComponent : MonoBehaviour {

	[Header("Unlock Stage")]
	public int series;
	public int stage;
	[Tooltip("If this is true, then the object will be set to uninteractable instead of disabled. Make sure there is a canvasGroup")]
	public bool makeUninteractable;

	private GameManager gm;
	private CanvasGroup group;

	void Start() {
		gm = GameManager.instance;
		if (makeUninteractable) {
			group = GetComponent<CanvasGroup>();
			group.interactable = false;
		}
		else
			gameObject.SetActive(CheckStage());
	}

	private bool CheckStage() {
		return gm.save.LatestSeriesIndex > series || (gm.save.LatestSeriesIndex == series && gm.save.LatestStageIndex > stage);
	}	
}