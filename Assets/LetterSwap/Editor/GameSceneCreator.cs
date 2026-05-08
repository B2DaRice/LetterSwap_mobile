using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LetterSwap.Editor
{
    public static class GameSceneCreator
    {
        private const string ScenePath = "Assets/LetterSwap/Scenes/GameScene.unity";
        private static readonly Color BackgroundColor = new Color(0.07f, 0.08f, 0.10f);
        private static readonly Color PanelColor = new Color(0.13f, 0.16f, 0.20f);
        private static readonly Color TileColor = new Color(0.93f, 0.89f, 0.78f);
        private static readonly Color TileAltColor = new Color(0.86f, 0.91f, 0.88f);
        private static readonly Color AccentColor = new Color(0.18f, 0.58f, 0.62f);
        private static readonly Color TextColor = new Color(0.96f, 0.97f, 0.94f);
        private static readonly Color DarkTextColor = new Color(0.13f, 0.15f, 0.18f);

        [MenuItem("LetterSwap/Create Game Scene")]
        public static void Create()
        {
            Directory.CreateDirectory("Assets/LetterSwap/Scenes");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "GameScene";

            CreateCamera();
            CreateSystemsRoot();
            CreateCanvas();
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("LetterSwap/Upgrade Open Scene Input Module")]
        public static void UpgradeOpenSceneInputModule()
        {
            var eventSystem = Object.FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                CreateEventSystem();
            }
            else
            {
                var standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
                if (standaloneInputModule != null)
                {
                    Object.DestroyImmediate(standaloneInputModule);
                }

                if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        [MenuItem("LetterSwap/Configure Open Scene Board Generation")]
        public static void ConfigureOpenSceneBoardGeneration()
        {
            var boardArea = GameObject.Find("BoardArea");
            if (boardArea == null)
            {
                Debug.LogError("Could not find BoardArea in the open scene.");
                return;
            }

            var boardRoot = boardArea.GetComponent<RectTransform>();
            var boardView = boardArea.GetComponent<BoardView>();
            if (boardView == null)
            {
                boardView = boardArea.AddComponent<BoardView>();
            }

            boardView.SetBoardRoot(boardRoot);
            ClearChildren(boardArea.transform);

            var gameControllerObject = GameObject.Find("GameController");
            if (gameControllerObject == null)
            {
                var systems = GameObject.Find("_Systems");
                gameControllerObject = new GameObject("GameController");
                if (systems != null)
                {
                    gameControllerObject.transform.SetParent(systems.transform);
                }
            }

            var gameController = gameControllerObject.GetComponent<GameController>();
            if (gameController == null)
            {
                gameController = gameControllerObject.AddComponent<GameController>();
            }

            gameController.SetBoardView(boardView);
            UpgradeOpenSceneInputModule();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = BackgroundColor;
            camera.orthographic = true;
            camera.orthographicSize = 10f;
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }

        private static void CreateSystemsRoot()
        {
            var systems = new GameObject("_Systems");
            new GameObject("GameController").transform.SetParent(systems.transform);
            new GameObject("InputController").transform.SetParent(systems.transform);
            new GameObject("BoardModel").transform.SetParent(systems.transform);
            new GameObject("BoardResolver").transform.SetParent(systems.transform);
            new GameObject("WordValidator").transform.SetParent(systems.transform);
            new GameObject("ScoreController").transform.SetParent(systems.transform);
        }

        private static void CreateCanvas()
        {
            var canvasObject = new GameObject("GameCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            var root = CreateUiObject("ScreenRoot", canvasObject.transform);
            Stretch(root);
            AddImage(root, BackgroundColor);

            CreateHeader(root.transform);
            CreateBoard(root.transform);
            CreateLevelCompletePanel(root.transform);
        }

        private static void CreateHeader(Transform parent)
        {
            var header = CreateUiObject("Header", parent);
            AnchorTop(header, 260f, 0f);

            var title = CreateText("Title", header.transform, "LetterSwap", 76, TextAnchor.MiddleLeft, TextColor);
            SetRect(title, new Vector2(64f, -36f), new Vector2(560f, 96f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            var score = CreateText("ScoreText", header.transform, "Score 0", 44, TextAnchor.MiddleRight, TextColor);
            SetRect(score, new Vector2(-64f, -46f), new Vector2(360f, 72f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

            var target = CreateText("TargetText", header.transform, "Target 100", 32, TextAnchor.MiddleRight, new Color(0.74f, 0.81f, 0.82f));
            SetRect(target, new Vector2(-64f, -116f), new Vector2(360f, 56f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));
        }

        private static void CreateBoard(Transform parent)
        {
            var boardArea = CreateUiObject("BoardArea", parent);
            SetRect(boardArea, Vector2.zero, new Vector2(980f, 980f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            AddImage(boardArea, PanelColor);

            var grid = boardArea.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 8;
            grid.cellSize = new Vector2(112f, 112f);
            grid.spacing = new Vector2(8f, 8f);
            grid.padding = new RectOffset(14, 14, 14, 14);

            var letters = "LETTRSWAPWORDGAMESORTPUZZLEBOARDTILESCOREVALIDMOVEFUNPLAY";
            for (var i = 0; i < 64; i++)
            {
                var tile = CreateUiObject($"Tile_{i / 8}_{i % 8}", boardArea.transform);
                AddImage(tile, i % 2 == 0 ? TileColor : TileAltColor);

                var letter = letters[i % letters.Length].ToString();
                var text = CreateText("Letter", tile.transform, letter, 54, TextAnchor.MiddleCenter, DarkTextColor);
                Stretch(text);
            }
        }

        private static void CreateLevelCompletePanel(Transform parent)
        {
            var overlay = CreateUiObject("LevelCompletePanel", parent);
            Stretch(overlay);
            AddImage(overlay, new Color(0f, 0f, 0f, 0.68f));

            var panel = CreateUiObject("Dialog", overlay.transform);
            SetRect(panel, Vector2.zero, new Vector2(760f, 460f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            AddImage(panel, PanelColor);

            var title = CreateText("Title", panel.transform, "Level Complete", 64, TextAnchor.MiddleCenter, TextColor);
            SetRect(title, new Vector2(0f, 118f), new Vector2(680f, 96f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            var score = CreateText("FinalScoreText", panel.transform, "Score 100", 42, TextAnchor.MiddleCenter, new Color(0.74f, 0.81f, 0.82f));
            SetRect(score, new Vector2(0f, 34f), new Vector2(680f, 68f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            var button = CreateUiObject("RestartButton", panel.transform);
            SetRect(button, new Vector2(0f, -112f), new Vector2(380f, 96f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            AddImage(button, AccentColor);
            button.AddComponent<Button>();

            var label = CreateText("Label", button.transform, "Restart", 38, TextAnchor.MiddleCenter, TextColor);
            Stretch(label);

            overlay.SetActive(false);
        }

        private static void CreateEventSystem()
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        private static GameObject CreateUiObject(string name, Transform parent)
        {
            var gameObject = new GameObject(name);
            gameObject.AddComponent<RectTransform>();
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        private static void ClearChildren(Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }

        private static Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment, Color color)
        {
            var textObject = CreateUiObject(name, parent);
            var text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = GetDefaultFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(12, fontSize / 2);
            text.resizeTextMaxSize = fontSize;
            return text;
        }

        private static Font GetDefaultFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static Image AddImage(GameObject gameObject, Color color)
        {
            var image = gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static void Stretch(Component component)
        {
            Stretch(component.gameObject);
        }

        private static void Stretch(GameObject gameObject)
        {
            var rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void AnchorTop(GameObject gameObject, float height, float topOffset)
        {
            var rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -topOffset);
            rect.sizeDelta = new Vector2(0f, height);
        }

        private static void SetRect(Component component, Vector2 anchoredPosition, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            SetRect(component.gameObject, anchoredPosition, size, anchorMin, anchorMax, pivot);
        }

        private static void SetRect(GameObject gameObject, Vector2 anchoredPosition, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            var rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }
    }
}
