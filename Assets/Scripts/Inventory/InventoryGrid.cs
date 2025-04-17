using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    
    public int columns = 4;
    public int rows = 4;

    public Slot[,] slots;

    void Awake()
    {
        slots = new Slot[columns, rows];

        Slot[] allSlots = GetComponentsInChildren<Slot>();

        if (allSlots.Length != columns * rows)
        {
            return;
        }

        int index = 0;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Slot slot = allSlots[index];
                slots[x, y] = slot;

                slot.x = x;
                slot.y = y;

                index++;
            }
        }
    }
}
