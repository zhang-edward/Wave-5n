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
	[HideInInspector] public bool doneAnimating;

	private PawnIconAnimated[] pawnIcons;
	private HeroExpMenuData[] data;
	private float endingExperience;
	private int numLevelUps;

	void Update() {
		if (Input.GetMouseButton(0)) {
			StopAllCoroutines();
			for (int i = 0; i < data.Length; i ++) {
				pawnIcons[i].Init(data[i].endState);
			}
			doneAnimating = true;
		}
	}

	public void Init(HeroExpMenuData[] data) {
		doneAnimating = false;
		this.data = data;
		pawnIcons = new PawnIconAnimated[data.Length];
		for (int i = 0; i < data.Length; i ++) {
			pawnIcons[i] = Instantiate(pawnIconAnimatedPrefab).GetComponent<PawnIconAnimated>();
			pawnIcons[i].transform.SetParent(contentFolder, false);
			pawnIcons[i].Init(data[i].startState, data[i].endState);
			pawnIcons[i].gameObject.SetActive(false);
		}
		StartCoroutine(StartAnimation());
	}

	private IEnumerator StartAnimation() {
		yield return new WaitForSeconds(0.5f);
		foreach (PawnIconAnimated pawnIcon in pawnIcons) {
			pawnIcon.gameObject.SetActive(true);
			yield return new WaitForSeconds(1.0f);
			pawnIcon.AnimateGetExperience();
		}
		doneAnimating = true;
	}
}
