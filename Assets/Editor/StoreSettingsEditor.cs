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
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[CustomEditor(typeof(StoreSettings))]

public class StoreSettingsEditor : UnityEditor.Editor
{
    private SerializedProperty m_Items;

    public void OnEnable()
    {
        m_Items = serializedObject.FindProperty("Items");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var settings = (target as StoreSettings);
        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < m_Items.arraySize; i++)
        {
            var serializedProperty = m_Items.GetArrayElementAtIndex(i);

            var item = settings.Items[i];
            var id = serializedProperty.FindPropertyRelative("Id");
            var name = serializedProperty.FindPropertyRelative("Name");
            var price = serializedProperty.FindPropertyRelative("Price");
            var icon = serializedProperty.FindPropertyRelative("Icon");
            var material = serializedProperty.FindPropertyRelative("Material");
            var model = serializedProperty.FindPropertyRelative("Model");
            var controller = serializedProperty.FindPropertyRelative("AnimatorController");

            EditorGUILayout.BeginHorizontal("box");
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {

                        var style = new GUIStyle(GUI.skin.label);
                        style.richText = true;
                        style.fontStyle = FontStyle.Bold;
                        EditorGUILayout.LabelField($@"Item#{item.Id}", style);

                        if (!item.IsValid)
                        {
                            EditorGUILayout.HelpBox(item.GetValidationStr(), MessageType.Error);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        icon.objectReferenceValue = EditorGUILayout.ObjectField(icon.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(80), GUILayout.Height(80));

                        EditorGUILayout.BeginVertical();
                        {
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(id);
                            GUI.enabled = true;

                            EditorGUILayout.PropertyField(name);
                            EditorGUILayout.PropertyField(price);
                            EditorGUILayout.PropertyField(material);
                            EditorGUILayout.PropertyField(model);
                            EditorGUILayout.PropertyField(controller);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Read CVS"))
        {
            ReadCvs();
        }

        if (settings.Items.Any(x => !x.IsValid))
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Apply"))
        {
            var storeObj = GameObject.Find("Store");
            var store = storeObj.GetComponent<Store>();
            GameObject prefab;

            foreach (var item in settings.Items)
            {
                if (CanCreatePrefab(item, out prefab))
                {
                    store.UpdateItems(item, prefab);
                }
                else
                {
                    Debug.Log($"Unexpected! Prefab could not created for id: {item.Id}, name: {item.Name}");
                }
            }
        }

        if (settings.Items.Any(x => !x.IsValid))
        {
            GUI.enabled = true;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void ReadCvs()
    {
        var settings = (target as StoreSettings);
        settings.Items.Clear();

        var content = (TextAsset)Resources.Load("Contents", typeof(TextAsset));

        StoreSettingsItem item;
        int price;
        string path;

        var lines = content.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            var cells = lines[i].Split(',');
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

    private bool CanCreatePrefab(StoreSettingsItem item, out GameObject prefab)
    {
        prefab = null;
        var pair = new ModelMaterialPair(item);

        if (!item.IsPrefabValid)
        {
            return false;
        }

        var settings = (target as StoreSettings);

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
            //collider.radius = bounds.size.x;
            collider.radius = .2f;
            collider.height = bounds.size.y;


            //Set materials
            var sharedMaterials = renderer.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                sharedMaterials[i] = item.Material;
            }

            renderer.sharedMaterials = sharedMaterials;

            //Set animator
            var animator = sceneObject.GetComponentInChildren<Animator>();
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.runtimeAnimatorController = item.AnimatorController;

            prefab = PrefabUtility.SaveAsPrefabAsset(sceneObject, path);
            DestroyImmediate(sceneObject);

            var modelInfo = new ModelInfo(pair, prefab);
            settings.ModelInfos.Add(modelInfo);
        }

        return true;
    }
}