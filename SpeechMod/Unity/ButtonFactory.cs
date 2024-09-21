using Kingmaker.Blueprints;
using SpeechMod.Unity.Extensions;
using System.Linq;
using Kingmaker;
using Kingmaker.UI.FullScreenUITypes;
using Kingmaker.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameObject = UnityEngine.GameObject;
using Object = UnityEngine.Object;

namespace SpeechMod.Unity;

public static class ButtonFactory
{
	//private const string ARROW_BUTTON_PATH = "/ServiceWindow/Encyclopedia/PathView/Next";
	private const string ARROW_BUTTON_PATH = "/StaticCanvas/Dialogue/Body/View/Scroll View/ButtonEdge";
	private const string ARROW_BUTTON_PREFAB_NAME = "SpeechMod_ArrowButtonPrefab";

    //private const string TEMP_ARROW_BUTTON_PATH = "SurfaceStaticPartPCView/StaticCanvas/SurfaceHUD/SurfaceActionBarPCView/MainContainer/ActionBarContainer/LeftSide/BackgroundContainer/Mask/Container/SurfaceActionBarPatyWeaponsView/CurrentSet/Layout/WeaponSlotsContainer/ConvertButton";
    //private const string TEMP_ARROW_BUTTON_PREFAB_NAME = "SpeechMod_Temporary_ArrowButtonPrefab";

    private static GameObject ArrowButton => UIHelper.TryFind(ARROW_BUTTON_PATH)?.gameObject;

    private static GameObject CreatePlayButton(Transform parent, UnityAction action, string text)
    {
        GameObject buttonGameObject;

        if (ArrowButton != null)
        {
            buttonGameObject = Object.Instantiate(ArrowButton, parent);
            buttonGameObject!.name = ARROW_BUTTON_PREFAB_NAME;
        }
        else
        {
			Debug.LogWarning("ArrowButton not found! Try loading resource...");
			return null;
        }

        buttonGameObject.transform!.localRotation = Quaternion.Euler(0, 0, 90);
        buttonGameObject.transform!.localScale = new Vector3(0.75f, 0.75f, 1f);

		SetupUIButton(buttonGameObject, action, text);

        return buttonGameObject;
    }

    private static void SetupUIButton(GameObject buttonGameObject, UnityAction action, string text)
    {
        if (buttonGameObject == null)
            return;

        var button = buttonGameObject.GetComponent<Button>();
        if (button == null)
        {
	        Debug.LogWarning("Button not found!");
			button = buttonGameObject.AddComponent<Button>();
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);

        button.interactable = true;
    }

    public static GameObject TryCreatePlayButton(Transform parent, UnityAction action, string text = null)
    {
        return CreatePlayButton(parent, action, text);
    }

    public static GameObject TryAddButtonToTextMeshPro(this TextMeshProUGUI textMeshPro, string buttonName, Vector2? anchoredPosition = null, Vector3? scale = null, TextMeshProUGUI[] textMeshProUguis = null)
    {
        var transform = textMeshPro?.transform;
        var tmpButton = transform.TryFind(buttonName)?.gameObject;
        if (tmpButton != null)
            return null;

#if DEBUG
        Debug.Log($"Adding playbutton to {textMeshPro?.name}...");
#endif

        var button = TryCreatePlayButton(transform, () =>
        {
            var text = textMeshPro?.text;
            if (textMeshProUguis != null)
            {
                text = textMeshProUguis.Where(textOverride => textOverride != null).Select(to => to.text).Aggregate("", (previous, current) => $"{previous}, {current}");
            }
            Main.Speech?.Speak(text);
        });

        if (button == null || button.transform == null)
            return null;

        button.name = buttonName;
        button.RectAlignTopLeft(anchoredPosition);

        if (scale.HasValue)
            button.transform!.localScale = scale.Value;

        button.SetActive(true);
        return button;
    }
}
