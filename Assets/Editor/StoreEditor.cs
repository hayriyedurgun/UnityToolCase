using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
using UnityEngine;

[CustomEditor(typeof(Store))]
public class StoreEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Read CVS"))
        {
            var store = (target as Store);
            //store.StoreItems.Clear();

            var content = (TextAsset)Resources.Load("Test", typeof(TextAsset));

            StoreItem item;
            int price;
            TextureImporter importer;
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
                    item = new StoreItem();
                    item.Id = store.StoreItems.Count; //TODO:
                    item.Name = cells[0];

                    if (!int.TryParse(cells[1], out price))
                    {
                        Debug.LogError($"Unexpected input at line {i} cell 1!!");
                        continue;
                    }

                    item.Price = price;
                    path = $"Assets/1_Graphics/Store/{item.Name}.png";

                    importer = AssetImporter.GetAtPath(path) as TextureImporter;

                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        AssetDatabase.WriteImportSettingsIfDirty(path);
                    }

                    var sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
                    if (sprite == null)
                    {
                        Debug.LogError($"Unexpected! Sprite could not found. Sprite name: {sprite}");
                        continue;
                    }

                    item.Icon = sprite;
                    store.StoreItems.Add(item);
                }
            }
        }
    }
}
