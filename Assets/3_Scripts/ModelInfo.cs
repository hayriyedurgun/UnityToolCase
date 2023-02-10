using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;

public class ModelInfo
{
    public GameObject Prefab { get; }
    public ModelMaterialPair Pair { get; }
    public string Name => Pair.ToString();

    public ModelInfo(ModelMaterialPair pair, GameObject prefab = null)
    {
        Pair = pair;
        Prefab = prefab;
    }
}

public class ModelMaterialPair
{
    private UnityEngine.Object m_Model;
    private Material m_Material;
    private RuntimeAnimatorController m_AnimatorController;

    public bool IsValid => m_Model != null && 
                           m_Material != null &&
                           m_AnimatorController != null;

    public ModelMaterialPair(StoreSettingsItem item)
    {
        m_Model = item.Model;
        m_Material = item.Material;
        m_AnimatorController = item.AnimatorController;
    }

    public override string ToString()
    {
        return $"{m_Model.name}_{m_Material?.name ?? string.Empty}_{m_AnimatorController?.name ?? string.Empty}";
    }
}

