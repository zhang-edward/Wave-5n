using UnityEngine;
using System.Collections;

public class PawnIconAnimated : PawnIconStandard
{
	private const string STATE_LEVELUP = "LevelUp";

	private Animator anim;
	private Pawn endState;

	void Awake() {
		anim = GetComponent<Animator>();
	}

	public void Init(Pawn startState, Pawn endState) {
		base.Init(startState);
		this.endState = endState;
	}

	public void PlayAnim(string state) {
		anim.CrossFade(state, 0);
	}

	public void AnimateGetExperience() {
		StartCoroutine(AnimateGetExperienceRoutine());
	}


	private IEnumerator AnimateGetExperienceRoutine() {
		int levelCounter = pawnData.level;
		int numLevelUps = endState.level - pawnData.level;
		float endingExp = endState.Experience;
		while (numLevelUps > 0) {
			while (experienceSlider.value < experienceSlider.maxValue) {
				experienceSlider.value += experienceSlider.maxValue / 64;
				yield return null;
			}
			if (experienceSlider.value >= experienceSlider.maxValue) {
				PlayAnim(STATE_LEVELUP);
				SetLevel(++levelCounter);
				experienceSlider.value = 0;
			}
			numLevelUps--;
		}
		experienceSlider.maxValue = endState.MaxExperience;
		while (experienceSlider.value < endingExp) {
			experienceSlider.value += experienceSlider.maxValue / 64;
			yield return null;
		}

		Init(endState);
	}
}