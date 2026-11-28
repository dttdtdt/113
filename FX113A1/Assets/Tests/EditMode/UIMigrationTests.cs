using System.Collections.Generic;
using System.Linq;
using GameSystem.CardSystem.Card.Manager;
using GameSystem.CardSystem.Card.So;
using GameSystem.UISystem.UIBound;
using GameSystem.UISystem.UIManager;
using GameSystem.Utils;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace GameSystem.Tests
{
    public class UIMigrationTests
    {
        private readonly List<Object> objects = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var obj in objects) if (obj != null) Object.DestroyImmediate(obj);
            objects.Clear();
        }

        private CardSo Template(int cost)
        {
            var config = ScriptableObject.CreateInstance<CardSo>();
            config.SetCardName("基础打击");
            config.SetCost(cost);
            config.SetDescription(new List<string> { "造成 6 点伤害。", "练习挥击。" });
            objects.Add(config);
            return config;
        }

        private CardUIBound Prefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI/CardDemo/Card.prefab");
            Assert.That(prefab, Is.Not.Null, "Create UI Migration Demo before running the UI tests.");
            return prefab.GetComponent<CardUIBound>();
        }

        private static TMP_Text Text(CardUIBound view, string name)
            => view.GetComponentsInChildren<TMP_Text>(true).Single(label => label.name == name);

        [Test]
        public void DescriptionKeepsSingleAndLastSegments()
        {
            Assert.That(StringMontage.Montage(null), Is.Empty);
            Assert.That(StringMontage.Montage(new[] { "单段。" }), Is.EqualTo("单段。"));
            Assert.That(StringMontage.Montage(new[] { "第一段。", "", "末段。" }), Is.EqualTo("第一段。\n末段。"));
        }

        [Test]
        public void CardPrefabRefreshesCostAndClearsData()
        {
            var view = Object.Instantiate(Prefab());
            objects.Add(view.gameObject);
            var card = CardManager.CreateCard(Template(2));
            view.Data = card;
            Assert.That(Text(view, "Name").text, Is.EqualTo("基础打击"));
            Assert.That(Text(view, "Description").text, Does.Contain("练习挥击。"));
            Assert.That(Text(view, "Cost").text, Is.EqualTo("2"));
            card.ModifyCost(-1);
            Assert.That(Text(view, "Cost").text, Is.EqualTo("1"));
            view.Data = null;
            card.ModifyCost(10);
            Assert.That(Text(view, "Cost").text, Is.Empty);
            Assert.That(Text(view, "Name").text, Is.Empty);
        }

        [Test]
        public void PoolReusesViewsWithoutOldBindingsOrDuplicateRentals()
        {
            var root = new GameObject("TestPool", typeof(RectTransform));
            objects.Add(root);
            var content = (RectTransform)new GameObject("Content", typeof(RectTransform)).transform;
            content.SetParent(root.transform, false);
            var pool = root.AddComponent<UIPool>();
            var fields = new SerializedObject(pool);
            fields.FindProperty("itemPrefab").objectReferenceValue = Prefab();
            fields.FindProperty("contentRoot").objectReferenceValue = content;
            fields.FindProperty("poolSize").intValue = 0;
            fields.ApplyModifiedPropertiesWithoutUndo();
            var oldCard = CardManager.CreateCard(Template(2));
            var view = pool.Get();
            view.Data = oldCard;
            view.transform.localScale = Vector3.one * 2;
            pool.ReturnToPool(view);
            pool.ReturnToPool(view);
            Assert.That(pool.ActiveCount, Is.Zero);
            Assert.That(view.Data, Is.Null);
            var reused = pool.Get();
            Assert.That(reused, Is.SameAs(view));
            reused.Data = CardManager.CreateCard(Template(5));
            oldCard.SetCost(99);
            Assert.That(Text(reused, "Cost").text, Is.EqualTo("5"));
            Assert.That(reused.transform.localScale, Is.EqualTo(Vector3.one));
            Assert.That(pool.Get(), Is.Not.SameAs(reused));
            Assert.That(pool.ActiveCount, Is.EqualTo(2));
        }
    }
}
