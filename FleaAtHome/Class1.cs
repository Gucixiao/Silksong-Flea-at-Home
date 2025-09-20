using BepInEx;
using BepInEx.Configuration;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace FleaAtHome
{
    [BepInPlugin("com.gcx.fleaathome", "Flea At Home", "0.1.9")]
    public class FleaAtHomePlugin : BaseUnityPlugin
    {
        private GameObject fleaObj;
        private VideoPlayer videoPlayer;
        private RenderTexture renderTexture;
        private GameObject canvasObj;
        private RawImage rawImage;

        private string videoPath;

        private float moveSpeed = 0.1f;
        private float zStep = 0.01f;
        private Vector3 initialPos;

        // Config entries
        private ConfigEntry<string> targetSceneName;
        private ConfigEntry<Vector3> autoSpawnPosition;

        private ConfigEntry<KeyboardShortcut> keySpawn;
        private ConfigEntry<KeyboardShortcut> keyDestroy;
        private ConfigEntry<KeyboardShortcut> keyResetPosition;

        private ConfigEntry<KeyboardShortcut> keyMoveUp;
        private ConfigEntry<KeyboardShortcut> keyMoveDown;
        private ConfigEntry<KeyboardShortcut> keyMoveLeft;
        private ConfigEntry<KeyboardShortcut> keyMoveRight;
        private ConfigEntry<KeyboardShortcut> keyZUp;
        private ConfigEntry<KeyboardShortcut> keyZDown;

        private void Awake()
        {
            Logger.LogInfo("FleaAtHome plugin loaded!");
            videoPath = Path.Combine(Paths.PluginPath, "FleaAtHome", "sleepingflea.webm");

            // Config
            targetSceneName = Config.Bind("AutoSpawn", "SceneName", "Belltown_Room_Spare", "Target scene for automatic flea spawn.");
            autoSpawnPosition = Config.Bind("AutoSpawn", "Position", new Vector3(27.45f, 7.68f, 0.01f), "Position for automatic flea spawn.");

            keySpawn = Config.Bind("Keys", "SpawnFlea", new KeyboardShortcut(KeyCode.F9), "Spawn flea");
            keyDestroy = Config.Bind("Keys", "DestroyFlea", new KeyboardShortcut(KeyCode.F10), "Destroy flea");
            keyResetPosition = Config.Bind("Keys", "ResetPosition", new KeyboardShortcut(KeyCode.End), "Reset flea position");

            // 移动键，默认左Ctrl + 方向键
            keyMoveUp = Config.Bind(
                "Keys",
                "MoveUp",
                new KeyboardShortcut(KeyCode.UpArrow, KeyCode.LeftControl),
                "Move flea up (default: Left Ctrl + UpArrow)"
            );

            keyMoveDown = Config.Bind(
                "Keys",
                "MoveDown",
                new KeyboardShortcut(KeyCode.DownArrow, KeyCode.LeftControl),
                "Move flea down (default: Left Ctrl + DownArrow)"
            );

            keyMoveLeft = Config.Bind(
                "Keys",
                "MoveLeft",
                new KeyboardShortcut(KeyCode.LeftArrow, KeyCode.LeftControl),
                "Move flea left (default: Left Ctrl + LeftArrow)"
            );

            keyMoveRight = Config.Bind(
                "Keys",
                "MoveRight",
                new KeyboardShortcut(KeyCode.RightArrow, KeyCode.LeftControl),
                "Move flea right (default: Left Ctrl + RightArrow)"
            );

            keyZUp = Config.Bind(
                "Keys",
                "MoveZUp",
                new KeyboardShortcut(KeyCode.PageUp, KeyCode.LeftControl),
                "Move flea forward (Z-) (default: Left Ctrl + PageUp)"
            );

            keyZDown = Config.Bind(
                "Keys",
                "MoveZDown",
                new KeyboardShortcut(KeyCode.PageDown, KeyCode.LeftControl),
                "Move flea backward (Z+) (default: Left Ctrl + PageDown)"
            );

            SceneManager.sceneLoaded += OnSceneLoaded;

            // 自动生成，如果插件加载时已在目标场景
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == targetSceneName.Value)
            {
                Logger.LogInfo($"Current scene is target scene {currentScene.name}, spawning flea after 0.1s...");
                StartCoroutine(SpawnFleaAfterDelay(autoSpawnPosition.Value, 0.1f));
            }
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (keySpawn.Value.IsDown() && fleaObj == null)
                SpawnFlea();

            if (keyDestroy.Value.IsDown())
                DestroyFlea();

            if (rawImage != null)
            {
                Vector3 pos = canvasObj.transform.position;
                bool moved = false;

                if (keyMoveUp.Value.IsPressed()) { pos.y += moveSpeed; moved = true; }
                if (keyMoveDown.Value.IsPressed()) { pos.y -= moveSpeed; moved = true; }
                if (keyMoveLeft.Value.IsPressed()) { pos.x -= moveSpeed; moved = true; }
                if (keyMoveRight.Value.IsPressed()) { pos.x += moveSpeed; moved = true; }
                if (keyZUp.Value.IsPressed()) { pos.z -= zStep; moved = true; }
                if (keyZDown.Value.IsPressed()) { pos.z += zStep; moved = true; }

                canvasObj.transform.position = pos;

                if (moved)
                    Logger.LogInfo($"Flea current position: {pos}");

                if (keyResetPosition.Value.IsDown())
                {
                    canvasObj.transform.position = initialPos;
                    Logger.LogInfo($"Flea position reset to {initialPos}");
                }
            }
        }

        private void SpawnFlea()
        {
            GameObject player = GameObject.FindWithTag("Player");
            Vector3 spawnPos = player != null
                ? player.transform.position + new Vector3(1f, 0f, 0.01f)
                : Vector3.zero;

            SpawnFleaAtPosition(spawnPos);
        }

        private void SpawnFleaAtPosition(Vector3 position)
        {
            initialPos = position;

            DestroyFlea();
            Logger.LogInfo($"SpawnFleaAtPosition called at {position}");

            // Canvas
            canvasObj = new GameObject("FleaCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "Default";
            canvas.sortingOrder = 0;

            var canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(5f, 5f);
            canvasObj.transform.position = initialPos;

            // RawImage
            var rawObj = new GameObject("FleaImage");
            rawObj.transform.SetParent(canvasObj.transform, false);
            rawImage = rawObj.AddComponent<RawImage>();
            rawImage.rectTransform.sizeDelta = new Vector2(5f, 5f);
            rawImage.color = new Color(0.675f, 0.7f, 0.8f, 1f);

            // VideoPlayer
            fleaObj = new GameObject("FleaVideoPlayer");
            videoPlayer = fleaObj.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = true;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            renderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
            videoPlayer.targetTexture = renderTexture;
            rawImage.texture = renderTexture;

            if (File.Exists(videoPath))
                videoPlayer.url = videoPath;
            else
            {
                Logger.LogError($"Video file not found at {videoPath}");
                return;
            }

            videoPlayer.Play();
            Logger.LogInfo($"Flea spawned at {initialPos} with size {rawImage.rectTransform.sizeDelta}");
        }

        private void DestroyFlea()
        {
            if (fleaObj != null)
            {
                Logger.LogInfo("Destroying flea...");
                Destroy(fleaObj);
                fleaObj = null;
            }
            if (canvasObj != null)
            {
                Destroy(canvasObj);
                canvasObj = null;
            }
            videoPlayer = null;
            rawImage = null;
            renderTexture?.Release();
            renderTexture = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.LogInfo($"Scene loaded: {scene.name}");
            if (scene.name == targetSceneName.Value)
            {
                Logger.LogInfo($"Entered target scene {scene.name}, spawning flea after 0.1s...");
                StartCoroutine(SpawnFleaAfterDelay(autoSpawnPosition.Value, 0.1f));
            }
        }

        private IEnumerator SpawnFleaAfterDelay(Vector3 position, float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnFleaAtPosition(position);
        }
    }
}
