using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class Item : MonoBehaviour
{
    Slot currentSlot;
    RectTransform rectTransform;
    Canvas canvas;
    bool isDraggable = false;
    bool moveToTarget = false;
    float speed = 2000f;
    
    
    void Start()
    {
        canvas = LevelManager.main.GetCanvas();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (currentSlot != null && moveToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentSlot.transform.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentSlot.transform.position) < 0.001f)
            {
                moveToTarget = false;
            }
        }
    }

    public void FindEmptySlot()
    {
        RemoveCurrentSlot();

        var emptySlots = GridManager.main.GetAllSlots().Where(slot => slot.IsEmpty()).ToArray();

        isDraggable = false;
        moveToTarget = true;
        
        if (emptySlots.Length == 0 && canvas == null) return;

        Slot nearestSlot = null;
        float minDistance = float.MaxValue;
        Vector2 mousePosition = Input.mousePosition;

        foreach (Slot slot in emptySlots)
        {
            Vector2 slotScreenPosition = Camera.main.WorldToScreenPoint(
                slot.GetRectTransform().position
            );
            
            float distance = Vector2.Distance(mousePosition, slotScreenPosition);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSlot = slot;
            }
        }

        if (nearestSlot != null)
        {
            SetCurrentSlot(nearestSlot);
            nearestSlot.SetCurrentItem(this);
        }
    }

    public void SetCurrentSlot(Slot _slot)
    {
        currentSlot = _slot;
    }

    public Slot GetCurrentSlot()
    {
        return currentSlot;
    }

    public void RemoveCurrentSlot()
    {
        if (currentSlot != null)
        {
            currentSlot.RemoveCurrentItem();
            currentSlot = null;
        }
    }

    public void SetDraggableStatus()
    {
        isDraggable = true;
    }
}
