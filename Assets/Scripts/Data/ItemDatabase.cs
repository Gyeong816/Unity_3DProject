using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Header("Item List")]
    public List<FieldItemInfo> allItems = new List<FieldItemInfo>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 예시 아이템들
        allItems = new List<FieldItemInfo>
        {
            new FieldItemInfo(1001, "Soap", 30),
            new FieldItemInfo(1002, "Duct Tape", 10),
            new FieldItemInfo(1003, "Toothpaste", 5),
            new FieldItemInfo(1004, "Hand Drill", 100),
            new FieldItemInfo(1005, "Screwdriver", 70),
            new FieldItemInfo(1006, "Wrench", 80),
            new FieldItemInfo(1007, "WD-40", 20),
        };
    }

    

   
    public List<FieldItemInfo> GetRandomItems(int count)
    {
        List<FieldItemInfo> result = new List<FieldItemInfo>();
        List<FieldItemInfo> pool = new List<FieldItemInfo>(allItems);

        count = Mathf.Min(count, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index].Clone()); // 복제해서 추가
            pool.RemoveAt(index);
        }

        return result;
    }
}