using UnityEngine;

public class Slot : MonoBehaviour
{
    Item currentItem;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public Item GetCurrentItem()
    {
        return currentItem;
    }

    public void SetCurrentItem(Item _item)
    {
        currentItem = _item;
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void RemoveCurrentItem()
    {
        currentItem = null;
    }
}
