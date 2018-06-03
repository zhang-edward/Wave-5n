using UnityEngine;
using System.Collections;

public class HeroExpMenu : MonoBehaviour
{
	public struct HeroExpMenuData {
		public Pawn startState;
		public Pawn endState;

		public HeroExpMenuData(Pawn startState, Pawn endState) {
			this.startState = startState;
			this.endState = endState;
		}
	}

	GameManager gm = GameManager.instance;

	public Transform contentFolder;
	public GameObject pawnIconAnimatedPrefab;

	private PawnIconAnimated pawnIcon;
	private float endingExperience;
	private int numLevelUps;

	void Start() {
	}

	public void Init(HeroExpMenuData data) {
		pawnIcon = Instantiate(pawnIconAnimatedPrefab).GetComponent<PawnIconAnimated>();
		pawnIcon.transform.SetParent(contentFolder, false);
		pawnIcon.Init(data.startState, data.endState);

		Invoke("StartAnimation", 1.0f);
	}

	private void StartAnimation() {
		pawnIcon.AnimateGetExperience();
	}
}
