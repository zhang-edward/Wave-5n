using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public Camera cam;
	public Player player;

	public SpriteRenderer screenOverlay;

	void Awake()
	{
		float height = cam.orthographicSize * 2.0f;
		float width = height * Screen.width / Screen.height;
		screenOverlay.transform.localScale = new Vector3 (width, height, 1);
		screenOverlay.color = new Color (1, 1, 1, 0);
	}

	void OnEnable()
	{
		player.OnEnemyDamaged += OnEnemyDamaged;
		player.OnPlayerDamaged += OnPlayerDamaged;
	}

	void OnDisable()
	{
		player.OnEnemyDamaged -= OnEnemyDamaged;
		player.OnPlayerDamaged -= OnPlayerDamaged;
	}

	private void OnEnemyDamaged(float strength)
	{
		StartShake (0.01f, strength * 0.1f);
	}

	private void OnPlayerDamaged(int damage)
	{
		StartFlashColor (new Color(1, 0, 0, 1));
	}

	public void StartShake(float time, float magnitude)
	{
		StartCoroutine (CameraShake (time, magnitude));
	}

	private IEnumerator CameraShake(float time, float magnitude)
	{
		while (time > 0)
		{
			time -= Time.deltaTime;
			float randX = Random.Range (-1f, 1f) * magnitude;
			float randY = Random.Range (-1f, 1f) * magnitude;

			cam.transform.localPosition = new Vector3(randX, randY, -10);
			yield return null;
		}
		cam.transform.localPosition = new Vector3 (0, 0, -10);
	}

	public void StartFlashColor(Color color)
	{
		StartCoroutine(FlashColor(color));
	}

	private IEnumerator FlashColor(Color color)
	{
		screenOverlay.color = color;
		float t = 0.4f;
		while (t > 0)
		{
			t -= Time.deltaTime;
			screenOverlay.color = new Color (color.r, color.b, color.g, t);
			yield return null;
		}
	}
}
