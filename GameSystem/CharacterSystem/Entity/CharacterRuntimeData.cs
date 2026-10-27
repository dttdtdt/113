using System.Collections.Generic;

public class CharacterRuntimeData
{
	private readonly Dictionary<CharacterAttributeType, CharacterDataAttribute> attributes = new();

	public CharacterSO Config { get; }

	public CharacterRuntimeData(CharacterSO config)
	{
		Config = config;
		attributes[CharacterAttributeType.Health] = new CharacterDataAttribute(config.GetHealth());
		attributes[CharacterAttributeType.Strength] = new CharacterDataAttribute(config.GetStrength());
		attributes[CharacterAttributeType.Durability] = new CharacterDataAttribute(config.GetDurability());
		attributes[CharacterAttributeType.Psycho] = new CharacterDataAttribute(config.GetPsycho());
		attributes[CharacterAttributeType.Faith] = new CharacterDataAttribute(config.GetFaith());
		attributes[CharacterAttributeType.Luck] = new CharacterDataAttribute(config.GetLuck());
	}

	public int GetValue(CharacterAttributeType type) => attributes[type].Current;

	public int GetBaseValue(CharacterAttributeType type) => attributes[type].Base;

	public CharacterDataAttribute GetAttribute(CharacterAttributeType type) => attributes[type];

	public IReadOnlyDictionary<CharacterAttributeType, CharacterDataAttribute> GetAll() => attributes;
}
