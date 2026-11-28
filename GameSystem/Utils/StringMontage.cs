using Godot;
using System;
using Godot.Collections;
public static class StringMontage
{
	public static string Montage(Array<string> Description)
	{
		string a = "";
		for(int i = 0 ; i < Description.Count - 1 ; i++)
		{
			a += Description[i] + "。";
		}
		return a;
	}
}
