using Assets.Editor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(StoreSettings))]

public class StoreSettingsEditor : UnityEditor.Editor
{
    private bool m_IsInitialized;
    private Store m_Store;
    private SerializedProperty m_Items;
    private StoreSettings m_Settings;

    private List<Sprite> m_Icons = new List<Sprite>();
    private List<string> m_Materials = new List<string>();
    private List<string> m_Models = new List<string>();
    private List<string> m_AnimationControllers = new List<string>();

    public void OnEnable()
    {
        m_Items = serializedObject.FindProperty("Items");
        m_Settings = (target as StoreSettings);

        if (!m_IsInitialized)
        {
            var storeObj = GameObject.Find("Store");
            m_Store = storeObj.GetComponent<Store>();

            m_IsInitialized = true;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < m_Items.arraySize; i++)
        {
            var serializedProperty = m_Items.GetArrayElementAtIndex(i);

            var item = m_Settings.Items[i];
            var id = serializedProperty.FindPropertyRelative("Id");
            var name = serializedProperty.FindPropertyRelative("Name");
            var price = serializedProperty.FindPropertyRelative("Price");
            var icon = serializedProperty.FindPropertyRelative("Icon");
            var material = serializedProperty.FindPropertyRelative("Material");
            var model = serializedProperty.FindPropertyRelative("Model");
            var controller = serializedProperty.FindPropertyRelative("AnimatorController");

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.normal.textColor = item.IsValid ? Color.white : Color.red;

            EditorGUILayout.BeginHorizontal("box");
            {
                serializedProperty.isExpanded = EditorGUILayout.Foldout(serializedProperty.isExpanded, $"Item#{item.Id}", foldoutStyle);
                if (GUILayout.Button("\u2715", GUILayout.Width(22), GUILayout.Height(20)))
                {
                    Remove(item);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (serializedProperty.isExpanded)
            {
                EditorGUILayout.BeginHorizontal("box");
                {
                    EditorGUILayout.BeginVertical();
                    {
                        if (!item.IsValid)
                        {
                            EditorGUILayout.HelpBox(item.GetValidationStr(), MessageType.Error);
                        }

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
        }

        if (GUILayout.Button("Read from Spreadsheet"))
        {
            ReadCvs();
        }

        if (GUILayout.Button("Show Report"))
        {
            ShowReport();
        }

        if (GUILayout.Button("Create"))
        {
            Create();
        }

        if (m_Settings.Items.Any(x => !x.IsValid))
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Apply"))
        {
            Apply();
        }
        if (m_Settings.Items.Any(x => !x.IsValid))
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
        m_Settings.Items.Clear();

        var content = (TextAsset)Resources.Load("Contents", typeof(TextAsset));

        StoreSettingsItem item;
        int price;
        string path;

        var lines = content.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            var cells = lines[i].Split(',');
            if (cells.Length > 1)
            {
                item = new StoreSettingsItem();
                item.Name = cells[0];

                if (!int.TryParse(cells[1], out price))
                {
                    Debug.LogError($"Unexpected input at line: {i} cell: 1! Please check the spreadsheet.");
                    continue;
                }

                item.Price = price;
                path = $"{m_Settings.IconPath}/{item.Name}.png";

                var sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;

                item.Id = m_Settings.Items.Count;
                item.Icon = sprite;

                m_Settings.Items.Add(item);
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

        var name = pair.ToString();

        if (m_Settings.ModelInfos.Any(x => x.Name == name))
        {
            prefab = m_Settings.ModelInfos.FirstOrDefault(x => x.Name == name).Prefab;
        }
        else
        {
            //create prefab
            var path = $"{m_Settings.PrefabPath}/{pair}.prefab";
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
            m_Settings.ModelInfos.Add(modelInfo);
        }

        return true;
    }

    private void Apply()
    {
        GameObject prefab;

        foreach (var item in m_Settings.Items)
        {
            if (CanCreatePrefab(item, out prefab))
            {
                m_Store.UpdateItems(item, prefab);
            }
            else
            {
                Debug.Log($"Unexpected! Prefab could not created for id: {item.Id}, name: {item.Name}");
            }
        }
    }

    private void Scan()
    {
        string[] temp = AssetDatabase.GetAllAssetPaths();
        m_Icons.Clear();
        m_Materials.Clear();
        m_Models.Clear();
        m_AnimationControllers.Clear();

        int startIndex;
        int lastIndex;
        int endIndex;

        foreach (string s in temp)
        {
            if (!s.StartsWith("Assets/") ||
                (!s.Contains(".png") &&
                !s.Contains(".mat") &&
                !s.Contains(".prefab") &&
                !s.Contains(".controller")))
            {
                continue;
            }

            startIndex = s.LastIndexOf("/");
            lastIndex = s.LastIndexOf(".");
            endIndex = lastIndex - startIndex - 1;

            if (startIndex < 0 || endIndex < 0 || endIndex <= 0)
            {
                continue;
            }

            var fileName = s.Substring(startIndex + 1, endIndex);

            if (s.Contains(".png"))
            {
                if (m_Settings.Items.Where(x => x.Icon != null).All(x => !x.Icon.name.Equals(fileName)))
                {
                    var icon = AssetDatabase.LoadAssetAtPath(s, typeof(Sprite)) as Sprite;
                    if (icon)
                    {
                        m_Icons.Add(icon);
                    }
                }
            }

            else if (s.Contains(".mat") &&
                    m_Settings.Items.Where(x => x.Material != null).All(x => !x.Material.name.Equals(fileName)))
            {
                m_Materials.Add(s);
            }

            else if (s.Contains(".prefab") &&
                    m_Settings.Items.Where(x => x.IsPrefabValid).All(x => x.GetPotentialPrefabName() != s) &&
                    s.Count(x => x == '_') >= 2)
            {
                m_Models.Add(s);
            }
            else if (s.Contains(".controller") &&
                m_Settings.Items.Where(x => x.AnimatorController != null).All(x => !x.AnimatorController.name.Equals(fileName)))
            {
                m_AnimationControllers.Add(s);
            }
        }
    }

    private void ShowReport()
    {
        var window = (ReportEditorWindow)EditorWindow.GetWindow(typeof(ReportEditorWindow));

        Scan();
        window.Load(m_Materials, m_Models, m_AnimationControllers, m_Icons, this);
        window.Show();
    }

    public void Create(Material mat = null, GameObject model = null, AnimatorController controller = null, Sprite icon = null)
    {
        var item = new StoreSettingsItem();
        item.Id = m_Settings.Items.Count;
        item.Icon = icon;
        item.Material = mat;
        item.Model = model;
        item.AnimatorController = controller;

        m_Settings.Items.Add(item);
    }

    private void Remove(StoreSettingsItem item)
    {
        if (m_Settings.Items.Contains(item))
        {
            m_Settings.Items.Remove(item);
            m_Settings.UpdateIds();
        }
    }
}