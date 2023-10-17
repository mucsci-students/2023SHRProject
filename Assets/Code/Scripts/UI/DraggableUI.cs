using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUIElement : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform dragTransform;

    private Vector2 offset;

    void Start()
    {
        dragTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = (Vector2)dragTransform.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragTransform.position = eventData.position + offset;
    }
}