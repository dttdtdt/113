using Godot;
using System;

public partial class CharacterSO : Resource
{
	[Export] private int Health{get;set;}
	[Export] private int Strength{get;set;}
	[Export] private int Durability{get;set;}
	[Export] private int Psycho{get;set;}
	[Export] private int Faith{get;set;}
	[Export] private int Luck{get;set;}
}
