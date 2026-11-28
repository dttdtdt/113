using GameSystem.CardSystem.Card.Mono;
using GameSystem.Utils;
using TMPro;
using UnityEngine;

namespace GameSystem.UISystem.UIBound
{
    public class CardUIBound : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image cardColor;
        [SerializeField] private TMP_Text labelName;
        [SerializeField] private TMP_Text cost;
        [SerializeField] private UnityEngine.UI.RawImage picture;
        [SerializeField] private TMP_Text cardDescription;
        [SerializeField] private TMP_Text cardClass;
        private CardMono data;
        private bool subscribed;

        public CardMono Data
        {
            get => data;
            set
            {
                Unsubscribe();
                data = value;
                Subscribe();
                Refresh();
            }
        }

        private void OnEnable() { Subscribe(); Refresh(); }
        private void OnDisable() => Unsubscribe();
        private void OnDestroy() => Unsubscribe();

        private void Subscribe()
        {
            if (subscribed || data == null || !isActiveAndEnabled) return;
            data.DataChanged += Refresh;
            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (subscribed && data != null) data.DataChanged -= Refresh;
            subscribed = false;
        }

        public void Refresh()
        {
            var info = data?.GetBaseCardInfo();
            if (labelName != null) labelName.text = info != null ? info.GetCardName() ?? "" : "";
            if (cost != null) cost.text = data != null ? data.GetCardData().GetCost().ToString() : "";
            if (cardDescription != null) cardDescription.text = info != null ? StringMontage.Montage(info.GetDescription()) : "";
            if (cardClass != null) cardClass.text = info != null ? TypeName(info.GetCardType()) : "";
            if (cardColor != null) cardColor.color = info != null ? CardOccupationToColor.Get(info.GetOccupation()) : Color.gray;
            if (picture != null)
            {
                picture.texture = info != null ? info.GetCardImage() : null;
                picture.enabled = picture.texture != null;
            }
        }

        private static string TypeName(CardSystem.Card.Enum.CardType type) => type switch
        {
            CardSystem.Card.Enum.CardType.Normal => "常规",
            CardSystem.Card.Enum.CardType.Ability => "能力",
            CardSystem.Card.Enum.CardType.Curse => "诅咒",
            _ => type.ToString()
        };
    }
}
