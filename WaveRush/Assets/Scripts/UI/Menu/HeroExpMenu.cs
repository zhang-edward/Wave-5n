using UnityEngine;
using System.Collections;

public class HeroExpMenu : MonoBehaviour
{
	public struct HeroExpMenuData {
		public float endingExperience;
		public int numLevelUps;
		public int startingLevel;
		public HeroExpMenuData(float endingExperience, int numLevelUps, int startingLevel) {
			this.endingExperience = endingExperience;
			this.numLevelUps = numLevelUps;
			this.startingLevel = startingLevel;
		}
	}

	GameManager gm = GameManager.instance;

	public Transform contentFolder;
	public GameObject pawnIconAnimatedPrefab;

	private PawnIconAnimated pawnIcon;
	private Pawn pawn;
	private float endingExperience;
	private int numLevelUps;

	void Start() {
	}

	public void Init(Pawn p, HeroExpMenuData data) {
		this.pawn = p;
		this.endingExperience = data.endingExperience;
		this.numLevelUps = data.numLevelUps;
		pawnIcon = Instantiate(pawnIconAnimatedPrefab).GetComponent<PawnIconAnimated>();
		pawnIcon.transform.SetParent(contentFolder, false);
		pawnIcon.Init(pawn, data.startingLevel);

		Invoke("StartAnimation", 1.0f);
	}

	private void StartAnimation() {
		pawnIcon.AnimateGetExperience(endingExperience, numLevelUps);
	}
}
