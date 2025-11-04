using UnityEngine;

public class ItemDB : MonoBehaviour
{
    public ItemInfo[] items;

    public ItemInfo GetItemInfo(string guid)
    {
        foreach (var item in items)
        {
            if (item.guid == guid)
                return item;
        }
        return null;
        
    }

}
