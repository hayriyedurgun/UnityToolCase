using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "StoreSettings", menuName = "ScriptableObjects/StoreSettings")]
public class StoreSettings : ScriptableObject
{
    [HideInInspector]
    public List<ModelInfo> ModelInfos = new List<ModelInfo>();

    public List<StoreSettingsItem> Items = new List<StoreSettingsItem>();

    public string IconPath => @"Assets/1_Graphics/Store";
    public string PrefabPath => @"Assets/2_Prefabs/CharacterPrefabs";

    public void UpdateIds()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].Id = i;
        }
    }
}

