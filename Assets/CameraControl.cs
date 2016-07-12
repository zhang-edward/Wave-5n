using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public Camera cam;
	public Player player;

	void OnEnable()
	{
		player.OnEnemyDamaged += OnEnemyDamaged;
	}

	void OnDisable()
	{
		player.OnEnemyDamaged -= OnEnemyDamaged;
	}

	private void OnEnemyDamaged(float strength)
	{
		Shake (0.01f, strength * 0.1f);
	}

	public void Shake(float time, float magnitude)
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
}
