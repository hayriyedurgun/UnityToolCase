using HomaGames.Internal.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StoreItem
{
    public int Id;
    public string Name;
    public int Price;
    public Sprite Icon;
    public GameObject Prefab;
}

public class Store : Singleton<Store>
{
    public List<StoreItem> StoreItems;
    public Action<StoreItem> OnItemSelected;

    public void SelectItem(StoreItem item)
    {
        OnItemSelected?.Invoke(item);
    }

    public void AddItem(StoreSettingsItem item, GameObject prefab)
    {
        var storeItem = new StoreItem();
        storeItem.Id = item.Id;
        storeItem.Name = item.Name;
        storeItem.Price = item.Price;
        storeItem.Icon = item.Icon;
        storeItem.Prefab = prefab;

        StoreItems.Add(storeItem);
    }
}
