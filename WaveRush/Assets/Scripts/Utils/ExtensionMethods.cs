using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
	private static System.Random rng = new System.Random(); 	// used in Shuffle extension method

	public static bool ContainsParam(this Animator _Anim, string _ParamName)
	{
		foreach (AnimatorControllerParameter param in _Anim.parameters)
		{
			if (param.name == _ParamName) return true;
		}
		return false;
	}

	public static void Shuffle<T>(this IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}

