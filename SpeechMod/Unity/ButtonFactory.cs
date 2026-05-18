using SpeechMod.Unity.Extensions;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameObject = UnityEngine.GameObject;
using Object = UnityEngine.Object;

namespace SpeechMod.Unity;

public static class ButtonFactory
{
    private const string ARROW_BUTTON_PATH = "/StaticCanvas/Dialogue/Body/View/Scroll View/ButtonEdge";
    private const string BUTTON_EDGE_SCENE_PATH = "Dialogue/Body/View/Scroll View/ButtonEdge";
    private const string PREFAB_NAME = "SpeechMod_ArrowButtonPrefab";
    private const int UI_INGAME_SCENE_BUILD_INDEX = 11;

    private static GameObject _storedPrefab;

    private static GameObject LiveArrowButton => UIHelper.TryFind(ARROW_BUTTON_PATH)?.gameObject;

    /// <summary>
    /// Call early on game load. Loads the UI scene additively, extracts the ButtonEdge
    /// as a DontDestroyOnLoad prefab, then unloads the scene.
    /// </summary>
    public static void Initialize()
    {
        if (_storedPrefab != null)
            return;

        var asyncOperation = SceneManager.LoadSceneAsync(UI_INGAME_SCENE_BUILD_INDEX, LoadSceneMode.Additive);
        if (asyncOperation == null)
        {
            Debug.LogWarning("[SpeechMod] Failed to start loading UI_Ingame_Scene!");
            return;
        }

        asyncOperation.allowSceneActivation = false;
        asyncOperation.priority = 100;
        asyncOperation.completed += _ => ExtractAndStorePrefab();
    }

    private static void ExtractAndStorePrefab()
    {
        var scene = SceneManager.GetSceneByBuildIndex(UI_INGAME_SCENE_BUILD_INDEX);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogWarning("[SpeechMod] UI_Ingame_Scene not valid after load!");
            return;
        }

        var roots = scene.GetRootGameObjects();

        foreach (var root in roots)
            root.SetActive(false);

        var buttonEdge = FindButtonEdgeInRoots(roots);

        if (buttonEdge != null)
        {
            _storedPrefab = Object.Instantiate(buttonEdge);
            _storedPrefab.name = PREFAB_NAME;
            _storedPrefab.SetActive(false);
            Object.DontDestroyOnLoad(_storedPrefab);
            Debug.Log("[SpeechMod] Successfully stored ArrowButton prefab.");
        }
        else
        {
            Debug.LogWarning("[SpeechMod] ButtonEdge not found in loaded scene!");
        }

        SceneManager.UnloadSceneAsync(scene);
    }

    private static GameObject FindButtonEdgeInRoots(GameObject[] roots)
    {
        foreach (var root in roots)
        {
            var found = root.transform.Find(BUTTON_EDGE_SCENE_PATH)?.gameObject;
            if (found != null)
                return found;
        }
        return null;
    }

    private static GameObject InstantiateButton(Transform parent)
    {
        if (LiveArrowButton != null)
            return Object.Instantiate(LiveArrowButton, parent);

        if (_storedPrefab != null)
        {
            var instance = Object.Instantiate(_storedPrefab, parent);
            instance.SetActive(true);
            return instance;
        }

        Debug.LogWarning("[SpeechMod] ArrowButton not available! Initialize() may not have completed yet.");
        return null;
    }

    private static GameObject CreatePlayButton(Transform parent, UnityAction action)
    {
        var buttonGameObject = InstantiateButton(parent);
        if (buttonGameObject == null)
            return null;

        buttonGameObject.name = PREFAB_NAME;
        buttonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
        buttonGameObject.transform.localScale = new Vector3(0.75f, 0.75f, 1f);

        SetupButton(buttonGameObject, action);

        return buttonGameObject;
    }

    private static void SetupButton(GameObject buttonGameObject, UnityAction action)
    {
        var button = buttonGameObject.GetComponent<Button>() ?? buttonGameObject.AddComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        button.interactable = true;
    }

    public static GameObject TryCreatePlayButton(Transform parent, UnityAction action, string text = null)
    {
        return CreatePlayButton(parent, action);
    }

    public static void TryAddButtonToTextMeshPro(this TextMeshProUGUI textMeshPro, string buttonName, Vector2? anchoredPosition = null, Vector3? scale = null, TextMeshProUGUI[] textMeshProUguis = null)
    {
        var transform = textMeshPro?.transform;
        if (transform == null)
        {
#if DEBUG
            Debug.Log($"Can't add button to {textMeshPro?.name} because it has no transform!");
#endif
            return;
        }

        if (transform.TryFind(buttonName)?.gameObject != null)
            return;

#if DEBUG
        Debug.Log($"Adding play button to {transform.name}...");
#endif

        var text = textMeshPro.text;
        var button = TryCreatePlayButton(transform, () =>
        {
            if (textMeshProUguis != null)
                text = textMeshProUguis.Where(t => t != null).Select(t => t.text).Aggregate("", (prev, curr) => $"{prev}, {curr}");

            Main.Speech?.Speak(text);
        });

        if (button == null || button.transform == null)
            return;

        button.name = buttonName;
        button.RectAlignTopLeft(anchoredPosition);

        if (scale.HasValue)
            button.transform.localScale = scale.Value;

        button.SetActive(true);
    }
}
