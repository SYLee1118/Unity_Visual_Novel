using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryScrollViewBehaviour : MonoBehaviour
{
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private GameObject inventorySlotPrefab;

    private Dictionary<string, GameObject> itemDictionary = new Dictionary<string, GameObject>();

    public void sortItems()
    {
        // 1줄 높이 300, 1줄당 4개의 아이템 슬롯.
        contentTransform.sizeDelta = new Vector2(contentTransform.rect.width, 300.0f * (itemDictionary.Count/4 + 1));

        int count = 0;
        foreach(var slot in itemDictionary)
        {
            // slot 위치 지정.
            slot.Value.GetComponent<RectTransform>().localPosition = new Vector3(150.0f + 300.0f * (count % 4), -150.0f - 300.0f * (count / 4), 0);
            count++;
        }
    }

    public void Add(string itemName)
    {
        // 이미 인벤토리에 들어있는 아이템의 경우,
        if (itemDictionary.ContainsKey(itemName) == true)
        {
            Debug.LogError("이미 추가된 아이템!");
            return;
        }
        
        GameObject newItem = Instantiate(inventorySlotPrefab, contentTransform.transform);
        newItem.name = itemName;
        // 새로 추가한 아이템슬롯의 sprite를 지정.
        newItem.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CG/boxImage/" + itemName);
        itemDictionary.Add(itemName, newItem);
        sortItems();
    }

    public bool Remove(string itemName)
    {
        if (itemDictionary.ContainsKey(itemName) == false)
        {
            Debug.LogError("존재하지 않는 아이템의 삭제시도!");
            return false;
        }

        GameObject go = itemDictionary[itemName];
        itemDictionary.Remove(itemName);
        Destroy(go);
        sortItems();
        return true;
    }
}
