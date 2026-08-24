using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Builds the main menu scene from scratch: a dark panel with a title, a Start
/// button that loads Scene01, and an Exit button. Also puts both scenes in
/// Build Settings with the menu first, so a build boots into the menu.
/// </summary>
static class MainMenuSceneBuilder
{
    const string ScenePath = "Assets/Scenes/mainmenu.unity";
    const string GameScenePath = "Assets/Scenes/Scene01.unity";
    const string GameSceneName = "Scene01";
    const string RunOnceKey = "MainMenuSceneBuilder.applied.v1";

    const string TitleText = "BOWLING";
    static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);
    static readonly Vector2 PanelSize = new Vector2(760f, 620f);
    static readonly Vector2 ButtonSize = new Vector2(420f, 120f);

    [MenuItem("Tools/Bowling/Build Main Menu Scene")]
    public static void Run()
    {
        Apply();
    }

    [InitializeOnLoadMethod]
    static void AutoRunOnce()
    {
        if (EditorPrefs.GetBool(RunOnceKey, false))
            return;

        EditorApplication.delayCall += () =>
        {
            if (EditorPrefs.GetBool(RunOnceKey, false))
                return;

            EditorPrefs.SetBool(RunOnceKey, true);
            Apply();
        };
    }

    static void Apply()
    {
        if (System.IO.File.Exists(ScenePath))
        {
            RegisterScenes();
            Debug.Log($"[MainMenuSceneBuilder] {ScenePath} already exists, left untouched. " +
                      "Build Settings order re-checked.");
            return;
        }

        var font = TMP_Settings.defaultFontAsset != null
            ? TMP_Settings.defaultFontAsset
            : Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        // Build in an additive scene so whatever is already open, saved or not,
        // is left alone.
        var previous = EditorSceneManager.GetActiveScene();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        EditorSceneManager.SetActiveScene(scene);

        try
        {
            BuildCamera();
            BuildEventSystem();
            var canvas = BuildCanvas();
            var menu = BuildPanel(canvas.transform, font);

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                Debug.LogError($"[MainMenuSceneBuilder] Could not save the scene to {ScenePath}");
                return;
            }

            RegisterScenes();

            Debug.Log($"[MainMenuSceneBuilder] Built and saved {ScenePath}. " +
                      $"Start loads \"{GameSceneName}\"; the menu is now build index 0. " +
                      $"Menu component: {menu.name}.");
        }
        finally
        {
            if (previous.IsValid())
                EditorSceneManager.SetActiveScene(previous);

            EditorSceneManager.CloseScene(scene, true);
        }
    }

    static void BuildCamera()
    {
        var go = new GameObject("Main Camera",
            typeof(Camera), typeof(AudioListener), typeof(UniversalAdditionalCameraData));
        go.tag = "MainCamera";

        var camera = go.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.05f, 0.06f, 0.09f, 1f);
        camera.orthographic = true;

        go.transform.position = new Vector3(0f, 0f, -10f);
    }

    static void BuildEventSystem()
    {
        // The project runs the new Input System, so the old StandaloneInputModule
        // would sit there doing nothing and the buttons would never click.
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    static Canvas BuildCanvas()
    {
        var go = new GameObject("Canvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = ReferenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    static MainMenu BuildPanel(Transform parent, TMP_FontAsset font)
    {
        var panel = CreateUI("Panel", parent);
        panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = Vector2.zero;
        panel.sizeDelta = PanelSize;

        var plate = panel.gameObject.AddComponent<Image>();
        plate.sprite = BuiltinSprite();
        plate.type = plate.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        plate.color = new Color(0f, 0f, 0f, 0.55f);

        // The menu logic lives on the panel, next to the buttons that call it.
        var menu = panel.gameObject.AddComponent<MainMenu>();

        var title = CreateText("Title", panel, TitleText, 96f, font);
        title.rectTransform.anchoredPosition = new Vector2(0f, 200f);
        title.rectTransform.sizeDelta = new Vector2(PanelSize.x - 60f, 160f);
        title.fontStyle = FontStyles.Bold;

        var start = CreateButton("ButtonStart", panel, "Start", font);
        start.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 10f);
        UnityEventTools.AddPersistentListener(start.onClick, menu.StartGame);

        var exit = CreateButton("ButtonExit", panel, "Exit", font);
        exit.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -140f);
        UnityEventTools.AddPersistentListener(exit.onClick, menu.ExitGame);

        return menu;
    }

    static Button CreateButton(string name, Transform parent, string label, TMP_FontAsset font)
    {
        var rect = CreateUI(name, parent);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = ButtonSize;

        var image = rect.gameObject.AddComponent<Image>();
        image.sprite = BuiltinSprite();
        image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = Color.white;

        var button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;

        var text = CreateText("Text (TMP)", rect, label, 54f, font);
        Stretch(text.rectTransform);
        text.color = new Color(0.08f, 0.08f, 0.1f, 1f);

        return button;
    }

    static TextMeshProUGUI CreateText(string name, Transform parent, string content,
                                      float size, TMP_FontAsset font)
    {
        var rect = CreateUI(name, parent);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

        var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = size;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        text.color = Color.white;

        if (font != null)
        {
            text.font = font;
            text.fontSharedMaterial = font.material;
        }

        return text;
    }

    static RectTransform CreateUI(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    static Sprite BuiltinSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    static void RegisterScenes()
    {
        var wanted = new List<EditorBuildSettingsScene>
        {
            // Index 0 is what a build starts on, so the menu goes first.
            new EditorBuildSettingsScene(ScenePath, true),
        };

        bool hasGameScene = false;
        foreach (var existing in EditorBuildSettings.scenes)
        {
            if (existing.path == ScenePath)
                continue;

            wanted.Add(existing);
            hasGameScene |= existing.path == GameScenePath;
        }

        if (!hasGameScene)
            wanted.Add(new EditorBuildSettingsScene(GameScenePath, true));

        EditorBuildSettings.scenes = wanted.ToArray();
    }
}
