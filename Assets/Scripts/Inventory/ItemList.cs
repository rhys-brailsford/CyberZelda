using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    // Singleton ItemList variable
    public static ItemList IL;

    [System.Serializable]
    public struct ItemProperties
    {
        public ItemName name;
        public Mesh mesh;
        public Mesh col;
        public Material mat;
    }
    public List<ItemProperties> itemProperties;

    //public ItemProperties props;

    public Dictionary<ItemName, Item> items;

    void Awake()
    {
        if (IL != null)
        {
            Debug.LogError("Only one ItemList allowed.");
            Destroy(IL);
        }
        else
        {
            IL = this;
        }

        DontDestroyOnLoad(this);

        items = new Dictionary<ItemName, Item>();

        // Dictionary of ItemName:Class pairs
        Dictionary<ItemName, System.Type> itemTypes = new Dictionary<ItemName, System.Type>();

        // To add a new item, add associated item class to dictionary
        itemTypes.Add(ItemName.Ammo, typeof(AmmoItem));
        itemTypes.Add(ItemName.Heart, typeof(HeartItem));
        itemTypes.Add(ItemName.Gun, typeof(GunItem));
        itemTypes.Add(ItemName.TempInvItem1, typeof(TempInvItem1));
        itemTypes.Add(ItemName.Undefined, null);

        // Assert that every item type is accounted for
        Debug.Assert(itemTypes.Count == System.Enum.GetNames(typeof(ItemName)).Length, "Item count in ItemList singleton mismatch!");

        // Create a new item object for each item defined in itemProperties
        foreach (ItemProperties curItemProps in itemProperties)
        {
            // Get the object type of the current item
            var itemType = itemTypes[curItemProps.name];
            // Get the constructor for the current item and call it, passing the current properties
            var constructors = itemType.GetConstructors();

            Item itemObj = (Item)ScriptableObject.CreateInstance(itemType.ToString());
            itemObj.Init(curItemProps.name, curItemProps.mesh, curItemProps.mat, curItemProps.col);
            items.Add(curItemProps.name, itemObj);
        }
    }

    public Item GetItem(ItemName itemToGet)
    {
        return items[itemToGet];
    }
}
