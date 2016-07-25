using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{

	public static bool ContainsParam(this Animator _Anim, string _ParamName)
	{
		foreach (AnimatorControllerParameter param in _Anim.parameters)
		{
			if (param.name == _ParamName) return true;
		}
		return false;
	}
}

