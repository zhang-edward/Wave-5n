using UnityEngine;
using System.Collections.Generic;

public class NewFeatureIndicatorGroup : MonoBehaviour {

	public List<NewFeatureIndicator> newFeatureIndicators;

	void Update() {
		gameObject.SetActive(!AllViewed());
	}

	private bool AllViewed() {
		foreach (NewFeatureIndicator nfi in newFeatureIndicators) {
			if(nfi.gameObject.activeInHierarchy)
				return false;
		}
		return true;
	}
}