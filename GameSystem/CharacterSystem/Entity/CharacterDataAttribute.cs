public partial class CharacterDataAttribute
{
	public int Base { get; }
	public int Current { get; private set; }

	public CharacterDataAttribute(int baseValue)
	{
		Base = baseValue;
		Current = baseValue;
	}

	public void SetCurrent(int value)
	{
		Current = value;
	}
}
