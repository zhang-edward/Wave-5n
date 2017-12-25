namespace PlayerActions
{
	public abstract class PlayerAction
	{
		protected Player player;
		protected PlayerHero hero;

		public float duration = -1;

		public delegate void ExecutedAction();
		public event ExecutedAction OnExecutedAction;

		public virtual void Init(Player player)
		{
			this.player = player;
			this.hero = player.hero;
		}

		public void Execute() {
			DoAction();
			if (OnExecutedAction != null)
				OnExecutedAction();
		}

		protected abstract void DoAction();
	}
}
