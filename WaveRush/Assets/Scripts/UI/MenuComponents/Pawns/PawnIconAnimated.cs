using UnityEngine;
using System.Collections;

public class PawnIconAnimated : PawnIconStandard
{
	private const string STATE_LEVELUP = "LevelUp";

	private Animator anim;
	private int startingLevel;

	void Awake() {
		anim = GetComponent<Animator>();
	}

	public void Init(Pawn pawnData, int startingLevel) {
		base.Init(pawnData);
		SetLevel(startingLevel);
		this.startingLevel = startingLevel;
	}

	public void PlayAnim(string state) {
		anim.CrossFade(state, 0);
	}

	public void AnimateGetExperience(float endingExp, int numLevelUps) {
		StartCoroutine(AnimateGetExperienceRoutine(endingExp, numLevelUps));
	}


	private IEnumerator AnimateGetExperienceRoutine(float endingExp, int numLevelUps) {
		while (numLevelUps > 0) {
			while (experienceSlider.value < experienceSlider.maxValue) {
				experienceSlider.value += experienceSlider.maxValue / 64;
				yield return null;
			}
			if (experienceSlider.value >= experienceSlider.maxValue) {
				PlayAnim(STATE_LEVELUP);
				SetLevel(++startingLevel);
				experienceSlider.value = 0;
			}
			numLevelUps--;
		}
		while (experienceSlider.value < endingExp) {
			experienceSlider.value += experienceSlider.maxValue / 64;
			yield return null;
		}
		experienceSlider.value = endingExp;
	}
}
