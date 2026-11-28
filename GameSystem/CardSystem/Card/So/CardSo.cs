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
	[Export] private Dictionary<Effect, Array<float>> Effects;
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

	public Dictionary<Effect, Array<float>> GetEffects() => Effects;
	public void SetEffects(Dictionary<Effect, Array<float>> value) => Effects = value;

	public Array<string> GetDescription() => Description;
	public void SetDescription(Array<string> value) => Description = value;

	public bool GetBattleOrManage() => BattleOrManage;
	public void SetBattleOrManage(bool value) => BattleOrManage = value;

	public bool GetTemp() => temp;
	public void SetTemp(bool value) => temp = value;

	public OutOrIn GetCardClass() => CardClass;
	public void SetCardClass(OutOrIn value) => CardClass = value;
	
	//效果例子
	//我想效果拆解为 条件 + 作用
	//在回合开始时抽一张牌，条件+作用，持续时间无限
	//下个回合开始时，获得一费用。持续时间1回合，条件 + 作用
	//每当抽33张牌，造成99点伤害。持续时间无限，条件 + 作用
	//造成11点伤害，挂2层易伤。作用 + 作用
	//易伤：每当受到攻击时，使得受到伤害增加1.5倍。持续时间按按层数 + 条件 + 作用
	//效果也可以是一种作用，易伤身为一个buff也是一种效果
	//因此为了设计出类似diy卡牌效果的设计，数据结构如下
	//我设计了作用类，效果继承自作用类，效果类会比作用类多出一个条件属性
	//条件类中定义了条件的持续时间，判断是否为瞬发效果而或延时或持续效果
	//其中还定义了条件的触发时机。
	//这是不是很像buff类，是的，其实我想让玩家能够diy效果，所以将buff类和效果类拆解拼装到了一起
	//原本的思路是buff是buff，作用是作用，buff也可以作为作用
}
