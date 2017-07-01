using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyWaveText : MonoBehaviour {

	public MessageText.Message waveMsg, bossIncomingMsg, waveCompleteMsg;

	public MessageText messageText;

	public CanvasGroup mainMessageGroup;
	public SimpleAnimationPlayerImage waveAnim;
	public AudioClip victorySound;
	public AudioClip warningSound;

	public ParticleSystem waveCompleteParticles;

	private bool canDisplayNextMessage = true;

	public void DisplayWaveNumber (int waveNumber)
	{
		waveMsg.message = "Wave " + waveNumber;
		StartCoroutine (DisplayMessage (waveMsg, WaveEffect, 1.0f));
	}

	public void DisplayWaveComplete()
	{
		StartCoroutine (DisplayMessage(waveCompleteMsg, WaveCompleteEffect, 0f, true));
	}

	public void DisplayBossIncoming()
	{
		StartCoroutine (DisplayMessage(bossIncomingMsg, BossIncomingEffect));
	}

	public void DisplayCustomMessage(MessageText.Message message)
	{
		StartCoroutine(DisplayMessage(message));
	}

	private void WaveEffect()
	{
		StartCoroutine(WaveEffectRoutine());
	}

	private IEnumerator WaveEffectRoutine()
	{
		waveAnim.gameObject.SetActive(true);
		waveAnim.Play();
		yield return new WaitForSecondsRealtime(waveMsg.persistTime);
		StartCoroutine(FadeOutCanvasGroup(mainMessageGroup, 0.2f));
		yield return new WaitForSecondsRealtime(0.2f);
		waveAnim.gameObject.SetActive(false);
		mainMessageGroup.alpha = 1;
	}

	private void WaveCompleteEffect()
	{
		waveCompleteParticles.gameObject.SetActive (true);
		waveCompleteParticles.Play ();
		SoundManager.instance.PlayUISound (victorySound);
	}

	private void BossIncomingEffect()
	{
		SoundManager.instance.PlayUISound (warningSound);
	}

	private IEnumerator DisplayMessage(MessageText.Message msg, MessageText.FlashedMessage callback = null, float delay = 0, bool interrupt = false)
	{
		if (!interrupt)
		{
			while (messageText.displaying || !canDisplayNextMessage)
				yield return null;
		}

		canDisplayNextMessage = false;
		DoMessage(msg, callback);
		yield return new WaitForSecondsRealtime(msg.totalMessageTime);
		canDisplayNextMessage = true;
	}

	private void DoMessage(MessageText.Message msg, MessageText.FlashedMessage callback)
	{
		messageText.OnFlashMessage = callback;
		messageText.SetColor(msg.color);
		messageText.Display(msg);
	}

	private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup, float fadeTime)
	{
		float t = 0;
		while (t < fadeTime)
		{
			t += Time.deltaTime;
			canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeTime);
			yield return null;
		}
	}
}
