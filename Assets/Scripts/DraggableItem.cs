using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    Canvas canvas;
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 originalPosition;
    Vector2 pointerOffset;
    Vector2? lastTouchPosition;
    bool isButtonPressed = false;
    bool isDraggable = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastTouchPosition = eventData.position;
        isButtonPressed = true;
        originalPosition = rectTransform.anchoredPosition;

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.blocksRaycasts = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPos
        );

        pointerOffset = (Vector2)rectTransform.localPosition - localPointerPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        );

        rectTransform.localPosition = localPointerPosition + pointerOffset;
        isDraggable = true;       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonPressed = false;
        isDraggable = false;
        
        ReturnItem();

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }

    void ReturnItem()
    {
       rectTransform.anchoredPosition = originalPosition;
    }
}