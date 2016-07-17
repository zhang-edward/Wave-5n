using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IDamageable
{
	public delegate void PlayerInitialized();
	public event PlayerInitialized OnPlayerInitialized;

	public delegate void EnemyDamaged (float strength);
	public event EnemyDamaged OnEnemyDamaged;

	public delegate void PlayerDamaged (int damage);
	public event PlayerDamaged OnPlayerDamaged;

	[HideInInspector]
	public float DEFAULT_SPEED;

	[Header("Entity Base Values")]
	public SpriteRenderer sr;
	public PlayerInput input;
	public EntityPhysics body;
	public Animator anim;

	[Header("Player Ability")]
	public PlayerAbility[] abilities;
	public PlayerAbility ability;

	[Header("Player direction")]
	public Vector2 dir;

	[Header("Stats")]
	public int maxHealth = 10;
	private int health = 10;
	public bool isInvincible = false;

	public float damagedCooldownTime = 1.0f;

	[HideInInspector]
	public ObjectPooler effectPool;

	void Start()
	{
		effectPool = ObjectPooler.GetObjectPooler ("Effect");
		DEFAULT_SPEED = body.moveSpeed;
		input.isInputEnabled = false;
	}

	public void Init(string name)
	{
		ability = GetAbilityWithName (name);
		anim.runtimeAnimatorController = ability.animatorController;
		ability.Init (this, body, anim);
		OnPlayerInitialized ();
		StartCoroutine (SpawnState ());
	}

	private PlayerAbility GetAbilityWithName(string name)
	{
		PlayerAbility answer = null;
		foreach (PlayerAbility playerAbility in abilities)
		{
			if (playerAbility.className.Equals (name))
				answer = playerAbility;
			else
				playerAbility.gameObject.SetActive (false);
		}
		if (answer == null)
			Debug.LogError ("Cannot find specified class name: " + name);
		return answer;
	}

	private IEnumerator SpawnState()
	{
		// Make sure the player animator has a state named "Spawn"
		UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("Spawn")));
		anim.CrossFade ("Spawn", 0f);

		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("Spawn"))
			yield return null;
		
		input.isInputEnabled = true;
	}

	/// <summary>
	/// Triggers the on enemy damaged event.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void TriggerOnEnemyDamagedEvent(int damage)
	{
		OnEnemyDamaged (damage);
	}

	/// <summary>
	/// Flashes red. Used when the player is damaged by an enemy.
	/// </summary>
	public IEnumerator FlashRed()
	{
		isInvincible = true;
		sr.color = Color.red;
		float t = 0;
		while (sr.color != Color.white)
		{
			sr.color = Color.Lerp (Color.red, Color.white, t);
			t += Time.deltaTime;
			yield return null;
		}
		isInvincible = false;
	}


	/// <summary>
	/// Damage by the specified amt.
	/// </summary>
	/// <param name="amt">Amount to deduct from health.</param>
	public void Damage(int amt)
	{
		if (isInvincible)
			return;

		body.AddRandomImpulse ();
		StartCoroutine (FlashRed ());

		health -= amt;
		// TODO: check if player is dead
		OnPlayerDamaged(amt);
	}

	/// <summary>
	/// Heal the specified amt.
	/// </summary>
	/// <param name="amt">Amount to add to health.</param>
	public void Heal(int amt)
	{
		health += amt;
		if (health >= maxHealth)
			health = maxHealth;
	}
}

