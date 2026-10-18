using Godot;
using System;

public enum CardOccupation
{
	warrior,
	ranger,
	caster,
	priest,
}

public enum CardType
{
	normal,
	ability,
	Curse,
}

public enum Rarity
{
	normal,
	rare,
	superrare,
	legendary,
	epic,
}

public enum CardAttribute
{
	cost,
	damage,
	defense,
	restore,
}


public enum DataOperation
{
	Addition,
	Minus,
	Multiply,
	Division,
}
