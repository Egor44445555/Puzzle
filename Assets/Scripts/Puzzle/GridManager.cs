using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager main;

    [SerializeField] GameObject slotPrefab;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform itemsTransform;
    [SerializeField] List<Sprite> itemArray;

    Transform parentTransform;
    List<Slot> allSlots = new List<Slot>();
    float timer = 0f;
    bool isCreateGrid = false;

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        parentTransform = FindObjectOfType<LevelManager>().transform;
        CreateGrid();
    }

    void Update()
    {
        if (!isCreateGrid)
        {
            timer += Time.deltaTime;
        }
        
        if (timer > 0.5f && !isCreateGrid)
        {
            CreateItems();
            isCreateGrid = true;
            timer = 0f;
        }
    }

    void CreateGrid()
    {
        for (var i = 0; i < itemArray.Count; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, transform);
            Slot slotComponent = slotObj.GetComponent<Slot>();
            slotComponent.SetPositionSlot(i);
            allSlots.Add(slotComponent);
        }
    }

    void CreateItems()
    {
        List<Slot> availableSlots = allSlots.Where(slot => slot.IsEmpty()).ToList();
        int index = 0;
        
        foreach (Sprite item in itemArray)
        {
            if (availableSlots.Count == 0) break;

            List<Slot> validSlots = availableSlots.Where(slot => slot.IsEmpty()).ToList();
            
            if (validSlots.Count == 0)
            {
                validSlots = availableSlots;
            }
            
            if (validSlots.Count == 0) break;
            
            int randomIndex = Random.Range(0, validSlots.Count);
            Slot selectedSlot = validSlots[randomIndex];
            
            GameObject itemObj = Instantiate(itemPrefab, selectedSlot.transform.position, selectedSlot.transform.rotation, itemsTransform);
            itemObj.name = itemObj.name + "-" + index;
            itemObj.GetComponent<RectTransform>().sizeDelta = selectedSlot.GetComponent<RectTransform>().sizeDelta;
            itemObj.GetComponent<Image>().sprite = item;
            
            Item itemComponent = itemObj.GetComponent<Item>();
            itemComponent.SetPositionItem(index);
            itemComponent.SetCurrentSlot(selectedSlot);
            selectedSlot.SetCurrentItem(itemComponent);
            
            availableSlots.Remove(selectedSlot);
            index++;
        }
    }
   
    public void SetAllSlots(Slot _slot)
    {
        allSlots.Add(_slot);
    }

    public List<Slot> GetAllSlots()
    {
        return allSlots;
    }

    public Transform GetWrapperTransform()
    {
        return parentTransform;
    }

    public Transform GetParentItemsTransform()
    {
        return itemsTransform;
    }

    public void CheckCurrentPositionItems()
    {
        bool isAllMatch = true;

        foreach(Slot slot in allSlots)
        {
            Item currentItem = slot.GetCurrentItem();

            if (slot.GetPositionSlot() != currentItem.GetPositionItem())
            {
                isAllMatch = false;
            }
        }

        if (isAllMatch)
        {
            foreach (var item in FindObjectsOfType<ParticleCreator>())
            {
                item.StartCreate();
            }

            UIManager.main.EndLevel();
        }
    }
}
