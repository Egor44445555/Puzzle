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
    Item itemComponent;
    Animator anim;
    bool animationPlay = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = FindObjectOfType<Canvas>();
        itemComponent = GetComponent<Item>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (animationPlay)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
            if (stateInfo.IsName("Decrease") && stateInfo.normalizedTime >= 1.0f)
            {
                anim.SetBool("Decrease", false);
                animationPlay = false;
            }
        }
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
        
        transform.SetParent(GridManager.main.GetWrapperTransform());
        UIManager.main.PlayGrabEffect();
        anim.SetBool("Increase", true);
        animationPlay = true;
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

        anim.SetBool("Increase", false);
        anim.SetBool("Decrease", true);
        transform.SetParent(GridManager.main.GetParentItemsTransform());
        
        if (itemComponent != null)
        {
            bool match = false;
            Slot oldSlot = itemComponent.GetCurrentSlot();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            foreach (RaycastResult result in results)
            {        
                Slot slot = result.gameObject.GetComponent<Slot>(); 

                if (slot != null)
                {
                    slot.GetCurrentItem().SetCurrentSlot(oldSlot);
                    oldSlot.SetCurrentItem(slot.GetCurrentItem());

                    slot.SetCurrentItem(itemComponent);                    

                    itemComponent.SetCurrentSlot(slot);
                    match = true;
                    GridManager.main.CheckCurrentPositionItems();
                    break;
                }
            }

            if (!match)
            {
                ReturnItem();
            }
        }
        else
        {
            ReturnItem();
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }
    void ReturnItem()
    {
       itemComponent.ReturnPosition();
    }
}