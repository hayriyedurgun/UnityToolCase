using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(StoreSettings))]

public class StoreSettingsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (GUILayout.Button("Read CVS"))
        {
            ReadCvs();
        }

        if (GUILayout.Button("Create"))
        {
            var settings = (StoreSettings)target;
            //settings.Items.ForEach(x => Create(x));
            CreatePrefab(settings.Items.First());
            //SimpleCreate(settings.Items.First());
        }
    }

    private void ReadCvs()
    {
        var settings = (target as StoreSettings);
        settings.Items.Clear();

        var content = (TextAsset)Resources.Load("Test", typeof(TextAsset));

        StoreSettingsItem item;
        int price;
        string path;

        var lines = content.text.Split(Environment.NewLine);
        for (int i = 1; i < lines.Length; i++)
        {
            var cells = lines[i].Split(",");
            if (cells.Length < 2)
            {
                Debug.LogError("Cell size incompetable in CVS!");
            }
            else
            {
                item = new StoreSettingsItem();
                item.Name = cells[0];

                if (!int.TryParse(cells[1], out price))
                {
                    Debug.LogError($"Unexpected input at line {i} cell 1!!");
                    continue;
                }

                item.Price = price;
                path = $"{settings.IconPath}/{item.Name}.png";

                var sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
                if (sprite == null)
                {
                    Debug.LogError($"Unexpected! Sprite could not found. Sprite name: {item.Name}");
                    continue;
                }

                item.Id = settings.Items.Count;
                item.Icon = sprite;

                settings.Items.Add(item);
            }
        }
    }

    private GameObject CreatePrefab(StoreSettingsItem item)
    {
        var settings = (target as StoreSettings);
        var pair = new ModelMaterialPair(item);
        GameObject prefab;
        var name = pair.ToString();

        if (settings.ModelInfos.Any(x => x.Name == name))
        {
            prefab = settings.ModelInfos.FirstOrDefault(x => x.Name == name).Prefab;
        }
        else
        {
            //create prefab
            var path = $"{settings.PrefabPath}/{pair}.prefab";

            var sceneObject = Instantiate(item.Model);

            //Set collider
            var collider = sceneObject.AddComponent<CapsuleCollider>();
            var renderer = sceneObject.GetComponentInChildren<Renderer>();
            var bounds = renderer.bounds;
            collider.center = bounds.center;
            collider.radius = bounds.size.x;
            collider.height = bounds.size.y;

            sceneObject.name = pair.ToString();

            //Set materials
            var sharedMaterials = renderer.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                sharedMaterials[i] = item.Material;
            }

            renderer.sharedMaterials = sharedMaterials;

            //Set animator
            var animator = sceneObject.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = item.AnimatorController;

            prefab = PrefabUtility.SaveAsPrefabAsset(sceneObject, path);
            DestroyImmediate(sceneObject);

            var modelInfo = new ModelInfo(pair, prefab);
            settings.ModelInfos.Add(modelInfo);
        }

        return prefab;
    }
}