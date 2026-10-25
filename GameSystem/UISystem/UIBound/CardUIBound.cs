// CardUIBound.cs
using Godot;

public partial class CardUIBound : Control
{
	[Export] public ColorRect CardColor { get; set; }
	[Export] public Label LabelName { get; set; }
	[Export] public Label Cost { get; set; }
	[Export] public TextureRect Picture { get; set; }
	[Export] public Label CardDescription { get; set; }
	[Export] public Label CardClass { get; set; }

	private CardMono _data;
	[Export] public CardMono Data
	{
		get => _data;
		set
		{
			if (_data != null) _data.DataChanged -= Refresh;
			_data = value;
			if (_data != null) _data.DataChanged += Refresh;
			Refresh();
		}
	}

	public override void _Ready()
	{
		Refresh();
	}

	/// <summary>
	/// 统一刷新入口，也可供外部手动调用
	/// </summary>
	public void Refresh()
	{
		if (!IsNodeReady() || _data == null) return;

		CardSo info = _data.GetBaseCardInfo();

		LabelName.Text = info.GetCardName() ?? "";
		Cost.Text = _data.GetCardData().GetCost().ToString();
		Picture.Texture = info.GetCardImage();
		CardClass.Text = info.GetCardType().ToString() ?? "";
		CardColor.Color = CardOccupationToColor.Get(info.GetOccupation());
		CardDescription.Text = StringMontage.Montage(info.GetDescription());
	}
}
