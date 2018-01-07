using PlayerActions;

public class Knight_ThornShield : HeroPowerUp {

	private const float STUN_DURATION = 2.0f;
	private const float STUN_RADIUS = 1.0f;

	public PA_AreaEffect stunArea;

	private KnightHero knight;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		this.knight = (KnightHero)hero;
		stunArea.Init(hero.player, StunEnemy);
		knight.OnKnightShieldHit += ExecuteAreaAttack;
	}

	private void ExecuteAreaAttack()
	{
		stunArea.SetPosition(knight.transform.position);
		stunArea.Execute();
	}

	private void StunEnemy(Enemy e)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = 2.0f;
		e.AddStatus(stun.gameObject);
	}
}
