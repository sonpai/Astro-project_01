//// InventorySlot.cs
//[System.Serializable] // Important for JSON serialization
//public class InventorySlot
//{
//    public string itemID; // References ItemData.itemID
//    public int quantity;

//    public InventorySlot(string id, int qty)
//    {
//        itemID = id;
//        quantity = qty;
//    }

//    public bool IsEmpty()
//    {
//        return string.IsNullOrEmpty(itemID) || quantity <= 0;
//    }
//}

[System.Serializable]
public class InventorySlot
{
    public string itemID;
    public int quantity;

    public InventorySlot(string id, int qty)
    {
        itemID = id;
        quantity = qty;
    }

    public bool IsEmpty() => string.IsNullOrEmpty(itemID) || quantity <= 0;
}
