using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnScreenStick : UnityEngine.UI.Graphic, IPointerDownHandler, IPointerUpHandler, IDragHandler, ICanvasRaycastFilter
{
    [SerializeField] RectTransform Panel;
    [SerializeField] RectTransform Stick;
    [SerializeField] float MovementRange = 50;

    public Vector3 Direction { get; private set; }

    Vector2 PointerDownPos;
    System.Action<Vector3> OnDirectionChanged;

    public void SetCallback(System.Action<Vector3> callback)
    {
        OnDirectionChanged = callback;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
        var delta = position - PointerDownPos;

        delta = Vector2.ClampMagnitude(delta, MovementRange);
        Stick.anchoredPosition = PointerDownPos + delta;

        Direction = new Vector3(delta.x / MovementRange, 0, delta.y / MovementRange);
        OnDirectionChanged?.Invoke(Direction);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out PointerDownPos))
        {
            Panel.anchoredPosition = PointerDownPos;
            Stick.anchoredPosition = PointerDownPos;

            Panel.gameObject.SetActive(true);
            Stick.gameObject.SetActive(true);

        }
    
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Panel.gameObject.SetActive(false);
        Stick.gameObject.SetActive(false);
        Direction = Vector2.zero;
        OnDirectionChanged?.Invoke(Direction);
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return true;
    }
}
