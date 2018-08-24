using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour {

	private static string REWARDED_VIDEO_ID = "rewardedVideo";
	public delegate void AdShown();
	public event AdShown OnRewardedVideoAdShown;

	public void ShowRewardedAd() {
		if (Advertisement.IsReady(REWARDED_VIDEO_ID)) {
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}

	private void HandleShowResult(ShowResult result) {
		switch (result) {
			case ShowResult.Finished:
				if (OnRewardedVideoAdShown != null)
					OnRewardedVideoAdShown();
				// Debug.Log("The ad was successfully shown");
				break;
			case ShowResult.Skipped:
				// Debug.Log("The ad was skipped before reaching the end");
				break;
			case ShowResult.Failed:
				// Debug.LogError("The ad failed to be shown");
				break;
		}
	}
}