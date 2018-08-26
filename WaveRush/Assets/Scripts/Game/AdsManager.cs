using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour {

	private static string REWARDED_VIDEO_ID = "rewardedVideo";
	public delegate void AdShown(int id);
	public event AdShown OnRewardedVideoAdShown;
	public int id;

	public void ShowRewardedAd(int id) {
		if (Advertisement.IsReady(REWARDED_VIDEO_ID)) {
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
			this.id = id;
		}
	}

	private void HandleShowResult(ShowResult result) {
		switch (result) {
			case ShowResult.Finished:
			if (OnRewardedVideoAdShown != null)
					OnRewardedVideoAdShown(id);
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