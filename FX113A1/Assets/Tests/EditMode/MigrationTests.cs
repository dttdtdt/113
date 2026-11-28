using System.Collections.Generic;
using GameSystem.BattleSystem.BattleScene;
using GameSystem.BattleSystem.Enum;
using GameSystem.CardSystem.Card.Entity;
using GameSystem.CardSystem.Card.Manager;
using GameSystem.CardSystem.Card.So;
using GameSystem.CardSystem.Deck.DeckData;
using GameSystem.CardSystem.Deck.Manager;
using GameSystem.CardSystem.Effect;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using CardDeck = GameSystem.CardSystem.Deck.DeckData.Deck;
using EffectData = GameSystem.CardSystem.Effect.impact.Effect;
using EffectTrigger = GameSystem.CardSystem.Effect.Trigger.Trigger;

namespace GameSystem.Tests
{
    public class MigrationTests
    {
        private readonly List<Object> objects = new();
        private readonly List<string> assetFolders = new();

        private T Config<T>() where T : ScriptableObject
        {
            var config = ScriptableObject.CreateInstance<T>();
            objects.Add(config);
            return config;
        }

        private GameObject SceneObject(string name)
        {
            var go = new GameObject(name);
            objects.Add(go);
            return go;
        }

        private string AssetFolder()
        {
            string name = "__MigrationTests_" + System.Guid.NewGuid().ToString("N");
            string guid = AssetDatabase.CreateFolder("Assets", name);
            Assert.That(guid, Is.Not.Empty, "Could not create the temporary test asset folder.");
            string folder = AssetDatabase.GUIDToAssetPath(guid);
            assetFolders.Add(folder);
            return folder;
        }

        private T SaveConfig<T>(T config, string path) where T : ScriptableObject
        {
            AssetDatabase.CreateAsset(config, path);
            // Persistent objects are cleaned up by deleting only their test folder.
            objects.Remove(config);
            return config;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var value in objects)
            {
                if (value != null) Object.DestroyImmediate(value);
            }
            objects.Clear();
            foreach (string folder in assetFolders) AssetDatabase.DeleteAsset(folder);
            assetFolders.Clear();
        }

        [Test]
        public void CardInstancesCopyTemplateValuesAndNotifyChanges()
        {
            var effect = Config<EffectData>();
            var template = Config<CardSo>();
            template.SetCost(2);
            template.SetEffects(new Dictionary<EffectData, List<float>> { [effect] = new() { 10f } });
            var first = CardManager.CreateCard(template);
            var second = CardManager.CreateCard(template);
            int notifications = 0;
            first.DataChanged += () => notifications++;

            first.AddEffectBonus(effect, 0, 2f);
            first.MultiplyEffectBonus(effect, 0, 1.5f);
            first.ModifyCost(-1);

            Assert.That(first.GetCardData().TryGetEffect(effect, out var firstData), Is.True);
            Assert.That(second.GetCardData().TryGetEffect(effect, out var secondData), Is.True);
            Assert.That(firstData.GetCurrent()[0], Is.EqualTo(18f));
            Assert.That(secondData.GetCurrent()[0], Is.EqualTo(10f));
            Assert.That(template.GetEffects()[effect][0], Is.EqualTo(10f));
            Assert.That(first.GetCardData().GetCost(), Is.EqualTo(1));
            Assert.That(second.GetCardData().GetCost(), Is.EqualTo(2));
            Assert.That(notifications, Is.EqualTo(3));

            first.ResetAllEffects();
            Assert.That(firstData.GetCurrent()[0], Is.EqualTo(10f));
        }

        [Test]
        public void EffectEntriesSurviveUnitySerialization()
        {
            // EditorJsonUtility resolves Unity object references through asset GUID/file ID.
            // A transient CreateInstance object has no persistent reference to restore.
            string folder = AssetFolder();
            var effect = SaveConfig(Config<EffectData>(), folder + "/Effect.asset");
            var template = Config<CardSo>();
            template.SetCardName("迁移测试");
            template.SetDescription(new List<string> { "第一段", "第二段" });
            template.SetEffects(new Dictionary<EffectData, List<float>> { [effect] = new() { 3f, 5f } });
            var restored = Config<CardSo>();

            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(template), restored);

            Assert.That(restored.GetCardName(), Is.EqualTo("迁移测试"));
            Assert.That(restored.GetDescription(), Has.Count.EqualTo(2));
            Assert.That(restored.GetEffects().ContainsKey(effect), Is.True);
            Assert.That(restored.GetEffects()[effect], Is.EqualTo(new[] { 3f, 5f }));
        }

        [Test]
        public void EffectEntriesSurviveAssetSaveAndReload()
        {
            string folder = AssetFolder();
            var effect = SaveConfig(Config<EffectData>(), folder + "/Effect.asset");
            var template = Config<CardSo>();
            template.SetCardName("资产序列化测试");
            template.SetEffects(new Dictionary<EffectData, List<float>> { [effect] = new() { 3f, 5f } });
            SaveConfig(template, folder + "/Card.asset");

            // Load a separate copy from disk, rather than asking for the cached original.
            string restoredPath = folder + "/RestoredCard.asset";
            Assert.That(AssetDatabase.CopyAsset(folder + "/Card.asset", restoredPath), Is.True);
            AssetDatabase.ImportAsset(restoredPath, ImportAssetOptions.ForceSynchronousImport);
            var restored = AssetDatabase.LoadAssetAtPath<CardSo>(restoredPath);

            Assert.That(restored, Is.Not.Null);
            Assert.That(restored, Is.Not.SameAs(template));
            Assert.That(restored.GetCardName(), Is.EqualTo("资产序列化测试"));
            Assert.That(restored.GetEffects().ContainsKey(effect), Is.True);
            Assert.That(restored.GetEffects()[effect], Is.EqualTo(new[] { 3f, 5f }));
        }

        [Test]
        public void EmptyTemplateCreatesAnEmptyRuntime()
        {
            var card = CardManager.CreateCard(Config<CardSo>());
            card.ResetAllEffects();
            Assert.That(card.GetCardData().GetCost(), Is.Zero);
        }

        [Test]
        public void AttributeResetRestoresBaseAndCurrentValuesCannotBeMutatedExternally()
        {
            var attributes = new CardAttributeData(new List<float> { 10f, 20f });
            attributes.AddBonus(1, 4f);
            attributes.MultiplyBonus(1, 2f);
            attributes.SetAllCurrent();
            Assert.That(attributes.GetCurrent()[1], Is.EqualTo(48f));
            attributes.GetCurrent()[1] = -100f;
            Assert.That(attributes.GetCurrent()[1], Is.EqualTo(48f));
            attributes.Reset();
            Assert.That(attributes.GetCurrent(), Is.EqualTo(new[] { 10f, 20f }));
        }

        [Test]
        public void DeckCanRemoveLastAndOnlyCardAndRejectInvalidIndices()
        {
            var deck = new CardDeck();
            var card = CardManager.CreateCard(Config<CardSo>());
            deck.AddToDeck(null);
            Assert.That(deck.Count, Is.Zero);
            Assert.That(deck.DrawFromDeck(), Is.Null);
            deck.AddToDeck(card);
            Assert.That(deck.DeleteCard(-1), Is.Null);
            Assert.That(deck.DeleteCard(deck.Count), Is.Null);
            Assert.That(deck.DeleteCard(0), Is.SameAs(card));
            Assert.That(deck.Count, Is.Zero);
        }

        [Test]
        public void TransferPreservesCardIdentityAndFailedTransfersDoNotChangeDecks()
        {
            var manager = SceneObject("DeckManager").AddComponent<DeckManager>();
            var source = new DrawDeck();
            var hand = new Hand();
            var card = CardManager.CreateCard(Config<CardSo>());
            source.AddToDeck(card);

            manager.TakeCardFromDeck(hand, source, 1);
            manager.DrawCardFromDeck(null, source);
            manager.DrawCardFromDeck(source, source);
            Assert.That(source.Count, Is.EqualTo(1));
            Assert.That(hand.Count, Is.Zero);

            manager.DrawCardFromDeck(hand, source);
            manager.DrawCardFromDeck(hand, source);
            Assert.That(source.Count, Is.Zero);
            Assert.That(hand.Cards, Is.EqualTo(new[] { card }));
        }

        [Test]
        public void ShufflePreservesEveryCardInstance()
        {
            var template = Config<CardSo>();
            var deck = new CardDeck();
            for (int i = 0; i < 20; i++) deck.AddToDeck(CardManager.CreateCard(template));
            var before = new List<GameSystem.CardSystem.Card.Mono.CardMono>(deck.Cards);
            deck.Shuffle();
            Assert.That(deck.Cards, Is.EquivalentTo(before));
        }

        [Test]
        public void EventBusAllowsEmptyPublishAndSubscriptionChangesDuringCallbacks()
        {
            var bus = new EventBus();
            bus.Invoke(TriggerTurn.TurnStart);
            int calls = 0;
            var trigger = new EffectTrigger { TriggerType = TriggerTurn.TurnStart };
            trigger.Action = _ => { calls++; bus.Unsubscribe(trigger); };
            bus.Subscribe(trigger);
            bus.Subscribe(trigger);
            bus.Invoke(TriggerTurn.TurnStart);
            bus.Invoke(TriggerTurn.TurnStart);
            Assert.That(calls, Is.EqualTo(1));
        }

        [Test]
        public void TriggerPassesItsContextAndAllowsMissingAction()
        {
            var context = new EffectContext { Caster = new object() };
            var trigger = new EffectTrigger { Context = context };
            trigger.TriggerInvoke();
            EffectContext received = null;
            trigger.Action = value => received = value;
            trigger.TriggerInvoke();
            Assert.That(received, Is.SameAs(context));
        }

        [Test]
        public void BattlePositionsUseIndependentPlayerAndEnemyLists()
        {
            var player = SceneObject("Player");
            var other = SceneObject("Other");
            var enemy = SceneObject("Enemy");
            var config = Config<BattleSceneSO>();
            config.SetPlayerPosition(new[] { player, other });
            config.SetEnemyPosition(new[] { enemy });
            var battle = SceneObject("Battle").AddComponent<BattleSceneMono>();
            Assert.That(battle.Initialize(config), Is.True);
            battle.ExchangePosition(0, 1);
            battle.ExchangePosition(-1, 0);
            battle.SetCharacter(battle.Player.Count, enemy);
            Assert.That(battle.Player[0], Is.SameAs(other));
            Assert.That(config.GetPlayerPosition()[0], Is.SameAs(player));
            Assert.That(battle.Enemy[0], Is.SameAs(enemy));
            Assert.That(battle.Initialize(null), Is.False);
        }
    }
}
