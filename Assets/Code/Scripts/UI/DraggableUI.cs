using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUIElement : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform _dragTransform;

    private Vector2 _offset;

    private void Start()
    {
        _dragTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _offset = (Vector2)_dragTransform.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragTransform.position = eventData.position + _offset;
    }
}
