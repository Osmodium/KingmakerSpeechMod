using UnityEngine;
using UnityEngine.EventSystems;

namespace SpeechMod.Unity;

public class BlockPointerPropagation : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        eventData.Use();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        eventData.Use();
    }
}