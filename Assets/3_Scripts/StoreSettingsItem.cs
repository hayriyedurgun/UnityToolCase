using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class StoreSettingsItem
{
    public int Id;
    public string Name;
    public int Price;
    public Sprite Icon;
    public Material Material;
    public GameObject Model;
    public RuntimeAnimatorController AnimatorController;
}
