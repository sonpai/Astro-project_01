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

// InventorySlot.cs
[System.Serializable] // Makes it visible in the Inspector and allows saving to JSON
public class InventorySlot
{
    public string itemID;
    public int quantity;

    public InventorySlot(string id, int qty)
    {
        itemID = id;
        quantity = qty;
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(itemID) || quantity <= 0;
    }
}
