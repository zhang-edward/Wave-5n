using UnityEngine;
using System.Collections;

public class NinjaSmokeBomb : MonoBehaviour
{
	public float lifetime = 3f;
	private float cooldown = 3f;
	public Sprite[] smokeSprites;
	public SimpleAnimation smokeBombEffect;
	private ObjectPooler effectsPool;

	private Vector3 worldPosition;

	void Start()
	{
		effectsPool = ObjectPooler.GetObjectPooler ("Effect");
	}

	void Update()
	{
		transform.position = worldPosition;
		cooldown -= Time.deltaTime;
		if (cooldown <= 0)
			gameObject.SetActive (false);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 2f);
	}

	public void Init(Vector3 position)
	{
		this.worldPosition = position;
		transform.position = worldPosition;
		gameObject.SetActive (true);
		cooldown = lifetime;
		for (int i = 0; i < 2; i++)
			Invoke("SpawnSmoke", 0.3f);
		TempObject smokeBomb = effectsPool.GetPooledObject ().GetComponent<TempObject> ();
		TempObjectInfo info = new TempObjectInfo (true,
			0f,
			smokeBombEffect.TimeLength * 0.9f,
			smokeBombEffect.TimeLength * 0.1f,
			new Color (1, 1, 1, 0.7f));
		SimpleAnimationPlayer animPlayer = smokeBomb.GetComponent<SimpleAnimationPlayer> ();
		animPlayer.anim = smokeBombEffect;
		smokeBomb.Init(
			Quaternion.identity,
			transform.position,
			smokeBombEffect.frames[0],
			info
		);
		animPlayer.Play ();
	}

	private void SpawnSmoke()
	{
		TempObject smoke = effectsPool.GetPooledObject ().GetComponent<TempObject>();
		TempObjectInfo info = new TempObjectInfo (true, 0.2f, lifetime / 2f, lifetime / 2f, new Color (1, 1, 1, 0.5f));
		smoke.Init (
			Quaternion.Euler (0, 0, Random.Range (0, 360f)),
			transform.position,
			smokeSprites [Random.Range (0, smokeSprites.Length)],
			info
		);
	}
}

