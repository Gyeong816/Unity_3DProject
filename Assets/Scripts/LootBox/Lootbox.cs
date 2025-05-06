using System.Collections.Generic;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    public List<FieldItemInfo> lootItems = new List<FieldItemInfo>();

    private void Start()
    {
        int itemCount = Random.Range(1, 6);
        lootItems = ItemDatabase.Instance.GetRandomItems(itemCount);
        
        
    }

    public List<FieldItemInfo> GetItems()
    {
        return lootItems;
    }

    public void RemoveItemByUUID(string uuid)
    {
        lootItems.RemoveAll(item => item.itemUuId == uuid);
    }
}