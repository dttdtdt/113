using Godot;
using Godot.Collections;

public partial class CardSo : Resource
{
	[Export] private string name;
	[Export] private Texture2D CardImage;
	[Export] private CardType cardType;
	[Export] private CardOccupation Occupation;
	[Export] private Rarity CardRarity;
	[Export] private int Cost;
	[Export] private Dictionary<EffectKeyResource, Array<float>> Effects;
	[Export] private Array<string> Description;
	[Export] private bool BattleOrManage;
	[Export] private bool temp;
	[Export] private OutOrIn CardClass;

	public string GetCardName() => name;
	public void SetCardName(string value) => name = value;

	public Texture2D GetCardImage() => CardImage;
	public void SetCardImage(Texture2D value) => CardImage = value;

	public CardType GetCardType() => cardType;
	public void SetCardType(CardType value) => cardType = value;

	public CardOccupation GetOccupation() => Occupation;
	public void SetOccupation(CardOccupation value) => Occupation = value;

	public Rarity GetCardRarity() => CardRarity;
	public void SetCardRarity(Rarity value) => CardRarity = value;

	public int GetCost() => Cost;
	public void SetCost(int value) => Cost = value;

	public Dictionary<EffectKeyResource, Array<float>> GetEffects() => Effects;
	public void SetEffects(Dictionary<EffectKeyResource, Array<float>> value) => Effects = value;

	public Array<string> GetDescription() => Description;
	public void SetDescription(Array<string> value) => Description = value;

	public bool GetBattleOrManage() => BattleOrManage;
	public void SetBattleOrManage(bool value) => BattleOrManage = value;

	public bool GetTemp() => temp;
	public void SetTemp(bool value) => temp = value;

	public OutOrIn GetCardClass() => CardClass;
	public void SetCardClass(OutOrIn value) => CardClass = value;
}
