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

public enum CardEffect
{
	drawCard,
	restorecost,
	damage,
	defense,
	restore,
}


public enum OutOrIn
{
	Both,
	Out,
	In,
	Neither,
}
