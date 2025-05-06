using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    public GameObject fieldItemPrefab;
    public static ItemDataManager Instance { get; private set; }

    [System.Serializable]
    public class ItemPrefabEntry
    {
        public string itemId;
        public GameObject prefab;
    }

    public List<ItemPrefabEntry> itemPrefabs;
    private Dictionary<string, GameObject> itemDict;

    private void Awake()
    {
        Instance = this;
        itemDict = new Dictionary<string, GameObject>();

        foreach (var entry in itemPrefabs)
        {
            itemDict[entry.itemId] = entry.prefab;
        }
    }

    public GameObject GetItemUIPrefabById(string id)
    {
        return itemDict.ContainsKey(id) ? itemDict[id] : null;
    }
    
    public GameObject GetFieldItemUIPrefab()
    {
        return fieldItemPrefab;
    }
}
