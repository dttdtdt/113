using Godot;
using System.Collections.Generic;
public partial class CharacterMono : Node
{
	[Export] private CharacterSO characterSO;

	private CharacterRuntimeData runtimeData;
	
	private List<CharacterEnum> Occupation = new();

	public override void _Ready()
	{
		if (runtimeData == null && characterSO != null)
		{
			runtimeData = new CharacterRuntimeData(characterSO);
			Occupation.Add(characterSO.GetCharacterType());
		}
			
	}

	public void SetRuntimeData(CharacterRuntimeData data)
	{
		runtimeData = data;
	}

	public CharacterRuntimeData GetRuntimeData() => runtimeData;

	public int GetValue(CharacterAttributeType type) => runtimeData.GetValue(type);

	public int GetBaseValue(CharacterAttributeType type) => runtimeData.GetBaseValue(type);
}
