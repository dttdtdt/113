using System;
using System.Collections.Generic;
using System.Linq;
using GameSystem.CardSystem.Card.Enum;
using GameSystem.CardSystem.Card.So;
using GameSystem.UISystem.UIBound;
using GameSystem.UISystem.UIManager;
using GameSystem.Utils.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace GameSystem.Editor
{
    public static class UIMigrationDemoBuilder
    {
        private const string Root = "Assets/UI/CardDemo";
        private static readonly Color Ink = new(0.16f, 0.18f, 0.20f);
        private static readonly Color Paper = new(0.94f, 0.91f, 0.83f);
        private static readonly Color Muted = new(0.66f, 0.70f, 0.73f);
        private static readonly Color Accent = new(0.78f, 0.62f, 0.37f);
        private static TMP_FontAsset font;

        [MenuItem("Tools/GameSystem/Create UI Migration Demo %#m")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            var loadedDemo = SceneManager.GetSceneByPath("Assets/Scenes/CardUIDemo.unity");
            if (loadedDemo.IsValid() && loadedDemo.isDirty)
                throw new InvalidOperationException("Save the demo scene before regenerating it.");
            EnsureFolder("Assets/UI");
            EnsureFolder(Root);
            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/TextMesh Pro/Resources/TMP Settings.asset") == null)
            {
                AssetDatabase.importPackageCompleted -= OnEssentialsImported;
                AssetDatabase.importPackageCompleted += OnEssentialsImported;
                TMP_PackageResourceImporter.ImportResources(true, false, false);
                return;
            }
            font = CreateFont();

            var template = AssetDatabase.LoadAssetAtPath<CardSo>(Root + "/TrainingStrike.asset");
            if (template == null)
            {
                template = ScriptableObject.CreateInstance<CardSo>();
                template.SetCardName("基础打击");
                template.SetCost(2);
                template.SetOccupation(CardOccupation.Warrior);
                template.SetCardType(CardType.Normal);
                template.SetDescription(new List<string> { "造成 6 点伤害。", "练习挥击，寻找敌人的破绽。" });
                AssetDatabase.CreateAsset(template, Root + "/TrainingStrike.asset");
            }

            var previous = SceneManager.GetActiveScene();
            bool canOpen = !Enumerable.Range(0, SceneManager.sceneCount).Any(i => SceneManager.GetSceneAt(i).isDirty);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            // Unity 不允许覆盖另一个已打开场景的路径；只关闭本工具生成的干净演示场景。
            if (loadedDemo.IsValid()) EditorSceneManager.CloseScene(loadedDemo, true);
            try
            {
                var card = CreateCard();
                var prefab = PrefabUtility.SaveAsPrefabAsset(card.gameObject, Root + "/Card.prefab").GetComponent<CardUIBound>();
                UnityEngine.Object.DestroyImmediate(card.gameObject);

                var camera = new GameObject("Main Camera", typeof(Camera));
                camera.tag = "MainCamera";
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
                camera.GetComponent<Camera>().backgroundColor = new Color(0.07f, 0.10f, 0.13f);
                var canvasObject = new GameObject("CardDemoCanvas", typeof(RectTransform), typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
                canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObject.GetComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
                var canvas = (RectTransform)canvasObject.transform;
                var background = Panel("Background", canvas, new Color(0.07f, 0.10f, 0.13f));
                Stretch(background.rectTransform);
                var header = Label("Eyebrow", canvas, "训练场  /  卡牌", 23, Accent);
                Position(header.rectTransform, 80, 72, 1100, 36);
                var title = Label("Title", canvas, "每一张牌，都是新的选择。", 48, Paper);
                Position(title.rectTransform, 80, 120, 1600, 80);
                var subtitle = Label("Subtitle", canvas, "生成卡牌，调整费用，再将它收回。", 25, Muted);
                Position(subtitle.rectTransform, 84, 212, 1400, 40);
                var line = Panel("HeaderRule", canvas, Accent);
                Position(line.rectTransform, 80, 280, 1760, 2);

                var content = Rect("Cards", canvas);
                content.anchorMin = new Vector2(0, 0.5f);
                content.anchorMax = new Vector2(1, 0.5f);
                content.pivot = new Vector2(0.5f, 0.5f);
                content.sizeDelta = new Vector2(-120, 430);
                content.anchoredPosition = new Vector2(0, -20);
                var layout = content.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                layout.spacing = 18;
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.childControlWidth = false;
                layout.childControlHeight = false;
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;

                var controls = Rect("Controls", canvas);
                controls.anchorMin = controls.anchorMax = new Vector2(0.5f, 0);
                controls.pivot = new Vector2(0.5f, 0);
                controls.anchoredPosition = new Vector2(0, 98);
                controls.sizeDelta = new Vector2(1020, 70);
                var spawn = Button("SpawnButton", controls, "生成卡牌", 0, true);
                var reduce = Button("CostButton", controls, "费用 -1", 350, false);
                var recycle = Button("ReturnButton", controls, "回收末张", 700, false);
                var count = Label("Count", canvas, "场上卡牌  0 / 6", 23, Paper);
                count.alignment = TextAlignmentOptions.TopRight;
                count.rectTransform.anchorMin = count.rectTransform.anchorMax = new Vector2(1, 1);
                count.rectTransform.pivot = new Vector2(1, 1);
                count.rectTransform.anchoredPosition = new Vector2(-84, -78);
                count.rectTransform.sizeDelta = new Vector2(400, 40);
                var hint = Label("Hint", canvas, "费用调整与回收作用于最后一张卡牌", 21, Muted);
                hint.alignment = TextAlignmentOptions.Center;
                hint.rectTransform.anchorMin = hint.rectTransform.anchorMax = new Vector2(0.5f, 0);
                hint.rectTransform.pivot = new Vector2(0.5f, 0);
                hint.rectTransform.anchoredPosition = new Vector2(0, 45);
                hint.rectTransform.sizeDelta = new Vector2(1300, 35);

                var services = new GameObject("UIManager");
                var pool = services.AddComponent<UIPool>();
                Set(pool, "itemPrefab", prefab);
                Set(pool, "contentRoot", content);
                var manager = services.AddComponent<UIManager>();
                Set(manager, "cardPool", pool);
                var input = services.AddComponent<TextButton>();
                Set(input, "testCard", template);
                Set(input, "manager", manager);
                Set(input, "spawnButton", spawn);
                Set(input, "costButton", reduce);
                Set(input, "returnButton", recycle);
                Set(input, "countLabel", count);
                var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
                EditorSceneManager.SaveScene(scene, "Assets/Scenes/CardUIDemo.unity");
            }
            finally
            {
                if (SceneManager.sceneCount == 1)
                    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                EditorSceneManager.CloseScene(scene, true);
                if (previous.IsValid()) SceneManager.SetActiveScene(previous);
            }
            AssetDatabase.SaveAssets();
            if (canOpen) EditorSceneManager.OpenScene("Assets/Scenes/CardUIDemo.unity");
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/CardUIDemo.unity");
        }

        private static void OnEssentialsImported(string packageName)
        {
            AssetDatabase.importPackageCompleted -= OnEssentialsImported;
            EditorApplication.delayCall += Build;
        }

        private static TMP_FontAsset CreateFont()
        {
            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Root + "/DemoChinese.asset");
            string characters = "训练场卡牌每一张都是新的选择生成调整费用再将它收回基础打击常规能力诅咒造成点伤害练习挥寻找敌人破绽与作用于最后上末";
            characters += new string(Enumerable.Range(32, 95).Select(i => (char)i).ToArray()) + "。，、/：-";
            if (existing != null && characters.All(c => existing.HasCharacter(c))) return existing;
            var result = TMP_FontAsset.CreateFontAsset("Microsoft YaHei", "Regular", 64);
            if (result == null) throw new InvalidOperationException("Microsoft YaHei font is unavailable.");
            result.name = "DemoChinese";
            if (!result.TryAddCharacters(characters, out string missing))
                throw new InvalidOperationException("Missing demo font characters: " + missing);
            result.atlasPopulationMode = AtlasPopulationMode.Static;
            if (existing == null) AssetDatabase.CreateAsset(result, Root + "/DemoChinese.asset");
            else
            {
                EditorUtility.CopySerialized(result, existing);
                UnityEngine.Object.DestroyImmediate(result);
                result = existing;
                result.ReadFontAssetDefinition();
            }
            if (!AssetDatabase.Contains(result.material)) AssetDatabase.AddObjectToAsset(result.material, result);
            foreach (var atlas in result.atlasTextures)
                if (!AssetDatabase.Contains(atlas)) AssetDatabase.AddObjectToAsset(atlas, result);
            EditorUtility.SetDirty(result);
            AssetDatabase.SaveAssetIfDirty(result);
            return result;
        }

        private static CardUIBound CreateCard()
        {
            var root = Rect("Card", null);
            root.sizeDelta = new Vector2(270, 410);
            var face = root.gameObject.AddComponent<UnityEngine.UI.Image>();
            face.color = Paper;
            face.raycastTarget = false;
            var shadow = root.gameObject.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.35f);
            shadow.effectDistance = new Vector2(0, -8);
            var stripe = Panel("Occupation", root, new Color(0.72f, 0.25f, 0.20f));
            Position(stripe.rectTransform, 0, 0, 270, 9);
            var name = Label("Name", root, "基础打击", 28, Ink);
            Position(name.rectTransform, 22, 28, 175, 44);
            var costBg = Panel("CostBadge", root, Ink);
            Position(costBg.rectTransform, 208, 24, 42, 48);
            var cost = Label("Cost", costBg.transform, "2", 28, Paper);
            Stretch(cost.rectTransform);
            cost.alignment = TextAlignmentOptions.Center;
            var art = Panel("Artwork", root, new Color(0.84f, 0.81f, 0.73f));
            Position(art.rectTransform, 22, 90, 226, 124);
            var accent = Panel("Blade", art.transform, new Color(0.62f, 0.30f, 0.24f));
            Position(accent.rectTransform, 110, 17, 8, 90);
            accent.rectTransform.localRotation = Quaternion.Euler(0, 0, -35);
            var cross = Panel("Guard", art.transform, Ink);
            Position(cross.rectTransform, 88, 88, 48, 6);
            cross.rectTransform.localRotation = Quaternion.Euler(0, 0, -35);
            var pictureObject = Rect("Picture", art.transform);
            Stretch(pictureObject);
            var picture = pictureObject.gameObject.AddComponent<UnityEngine.UI.RawImage>();
            picture.raycastTarget = false;
            picture.enabled = false;
            var description = Label("Description", root, "", 24, Ink);
            Position(description.rectTransform, 24, 236, 222, 112);
            var rule = Panel("Rule", root, new Color(0.75f, 0.71f, 0.63f));
            Position(rule.rectTransform, 24, 360, 222, 1);
            var type = Label("Type", root, "常规", 21, new Color(0.35f, 0.33f, 0.28f));
            Position(type.rectTransform, 24, 371, 222, 28);
            type.alignment = TextAlignmentOptions.Center;
            var view = root.gameObject.AddComponent<CardUIBound>();
            Set(view, "cardColor", stripe);
            Set(view, "labelName", name);
            Set(view, "cost", cost);
            Set(view, "picture", picture);
            Set(view, "cardDescription", description);
            Set(view, "cardClass", type);
            return view;
        }

        private static UnityEngine.UI.Button Button(string name, Transform parent, string text, float x, bool primary)
        {
            var image = Panel(name, parent, primary ? Accent : new Color(0.15f, 0.20f, 0.24f));
            image.raycastTarget = true;
            Position(image.rectTransform, x, 0, 320, 70);
            var button = image.gameObject.AddComponent<UnityEngine.UI.Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.highlightedColor = new Color(1.16f, 1.16f, 1.16f);
            colors.pressedColor = new Color(0.75f, 0.75f, 0.75f);
            colors.disabledColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            button.colors = colors;
            var label = Label("Label", image.transform, text, 26, primary ? Ink : Paper);
            Stretch(label.rectTransform);
            label.alignment = TextAlignmentOptions.Center;
            return button;
        }

        private static RectTransform Rect(string name, Transform parent)
        {
            var rect = (RectTransform)new GameObject(name, typeof(RectTransform)).transform;
            if (parent != null) rect.SetParent(parent, false);
            return rect;
        }

        private static UnityEngine.UI.Image Panel(string name, Transform parent, Color color)
        {
            var image = Rect(name, parent).gameObject.AddComponent<UnityEngine.UI.Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static TMP_Text Label(string name, Transform parent, string value, float size, Color color)
        {
            var text = Rect(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.fontSize = size;
            text.color = color;
            text.text = value;
            text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.lineSpacing = 6;
            return text;
        }

        private static void Position(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        private static void Set(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int index = path.LastIndexOf('/');
            AssetDatabase.CreateFolder(path.Substring(0, index), path.Substring(index + 1));
        }
    }
}
