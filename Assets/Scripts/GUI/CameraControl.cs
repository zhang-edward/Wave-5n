﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public static CameraControl instance;
	public Camera cam;
	public Player player;

	public SpriteRenderer screenFlash;
	public SpriteRenderer screenOverlay;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);

		cam.orthographicSize = 5;
		transform.position = player.transform.position;

		float height = cam.orthographicSize * 2.0f;
		float width = height * Screen.width / Screen.height;
		screenFlash.transform.localScale = new Vector3 (width, height, 1);
		screenFlash.color = new Color (1, 1, 1, 0);
		screenOverlay.transform.localScale = new Vector3 (width, height, 1);
		screenOverlay.color = new Color (1, 1, 1, 0);
	}

	void Update()
	{
		if (player.targetedEnemy == null)
		{
			transform.position = Vector3.Lerp (transform.position, player.transform.position, Time.deltaTime * 8f);
		}
		else
		{
			Vector3 dest = Vector3.Lerp (player.transform.position, player.targetedEnemy.position, 0.3f);
			transform.position = Vector3.Lerp (transform.position, dest, Time.deltaTime * 3f);
		}
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
		StartShake (0.05f, strength * 0.2f);
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
			float randX = UtilMethods.RandSign() * magnitude;
			float randY = UtilMethods.RandSign() * magnitude;
		
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
		screenFlash.color = color;
		float t = 0.4f;
		while (t > 0)
		{
			t -= Time.deltaTime;
			screenFlash.color = new Color (color.r, color.b, color.g, t);
			yield return null;
		}
	}

	public void SetOverlayColor(Color color, float a)
	{
		StartCoroutine (FadeOverlayColor (color, a));
	}

	private IEnumerator FadeOverlayColor(Color color, float a)
	{
		screenOverlay.color = new Color(color.r, color.g, color.b, 0);
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime;
			screenOverlay.color = new Color (color.r, color.g, color.b, Mathf.Lerp(screenOverlay.color.a, a, t));
			yield return null;
		}
		screenOverlay.color = new Color(color.r, color.g, color.b, a);

	}
}