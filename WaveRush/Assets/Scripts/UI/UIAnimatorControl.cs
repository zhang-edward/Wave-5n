using UnityEngine;

public class UIAnimatorControl : MonoBehaviour
{
	private Animator animator;
	public bool isOutByDefault;		// whether this ui element should be transitioned out by default

	void Awake()
	{
		animator = GetComponent<Animator>();
		if (isOutByDefault)
			animator.SetTrigger("DefaultOut");
	}
}