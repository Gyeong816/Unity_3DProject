using System;

[System.Serializable]
public class FieldItemInfo
{
    public int id;
    public string itemName;
    public int price;
    public string itemUuId;

    public int slotX;  // 추가
    public int slotY;  // 추가

    public FieldItemInfo(int id, string itemName, int price)
    {
        this.id = id;
        this.itemName = itemName;
        this.price = price;
        this.itemUuId = Guid.NewGuid().ToString();
        this.slotX = -1;
        this.slotY = -1;
    }
    
    public FieldItemInfo Clone()
    {
        return new FieldItemInfo(id, itemName, price); // 새 인스턴스 생성
    }
}
