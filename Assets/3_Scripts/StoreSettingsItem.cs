using System;
using System.Text;
using UnityEngine;
using static UnityEditor.Progress;

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

    public bool IsValid => Model != null &&
                           Material != null &&
                           AnimatorController != null &&
                           Icon != null &&
                           Price > 0;

    public string GetValidationStr()
    {
        var sb = new StringBuilder();

        if (Price < 0)
        {
            sb.AppendLine("Price must be positive number.");
        }
        if (Icon == null)
        {
            sb.AppendLine("Icon must be set.");
        }
        if (Model == null)
        {
            sb.AppendLine("Model is not set!");
        }
        if (Material == null)
        {
            sb.AppendLine("Material is not set!");
        }
        if (AnimatorController == null)
        {
            sb.AppendLine("AnimatorController is not set!");
        }

        return sb.ToString();
    }
}
