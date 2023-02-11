using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

namespace Assets.Editor
{
    public class ReportEditorWindow : EditorWindow
    {
        private List<string> m_Materials;
        private List<string> m_Models;
        private List<string> m_AnimationControllers;
        private List<Sprite> m_Icons;
        private StoreSettingsEditor m_StoreEditor;
        private Vector2 m_ScrollPos;

        public void Load(List<string> materials, List<string> models, List<string> animControllers, List<Sprite> icons, StoreSettingsEditor storeEditor)
        {
            m_Materials = materials;
            m_Models = models;
            m_AnimationControllers = animControllers;
            m_Icons = icons;
            m_StoreEditor = storeEditor;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            {
                GUILayout.Space(10);

                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, false, false);
                {
                    GUILayout.Label("Icons", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    foreach (var icon in m_Icons)
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        {
                            EditorGUILayout.LabelField(icon.name);
                            if (GUILayout.Button("Create"))
                            {
                                m_StoreEditor.Create(icon: icon);
                                this.Close();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    DrawUILine();
                    EditorGUI.indentLevel--;

                    GUILayout.Label("Materials", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    foreach (var materialPath in m_Materials)
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        {
                            EditorGUILayout.LabelField(GetShortName(materialPath));
                            if (GUILayout.Button("Create"))
                            {
                                var material = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
                                m_StoreEditor.Create(mat: material);
                                this.Close();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    DrawUILine();
                    EditorGUI.indentLevel--;


                    GUILayout.Label("Models", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    foreach (var modelPath in m_Models)
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        {
                            EditorGUILayout.LabelField(GetShortName(modelPath));
                            if (GUILayout.Button("Create"))
                            {
                                var model = (GameObject)AssetDatabase.LoadAssetAtPath(modelPath, typeof(GameObject));
                                m_StoreEditor.Create(model: model);
                                this.Close();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    DrawUILine();
                    EditorGUI.indentLevel--;

                    EditorGUI.indentLevel++;

                    GUILayout.Label("Animation Controllers", EditorStyles.boldLabel);
                    foreach (var animPath in m_AnimationControllers)
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        {
                            EditorGUILayout.LabelField(GetShortName(animPath));
                            if (GUILayout.Button("Create"))
                            {
                                var anim = (AnimatorController)AssetDatabase.LoadAssetAtPath(animPath, typeof(AnimatorController));
                                m_StoreEditor.Create(controller: anim);
                                this.Close();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();

                GUILayout.Space(10);
            }
            EditorGUILayout.EndVertical();

        }

        private void DrawUILine(Color color = default, int thickness = 1, int padding = 10)
        {
            color = color != default ? color : Color.grey;
            Rect r = EditorGUILayout.GetControlRect(false, GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding * 0.5f;

            EditorGUI.DrawRect(r, color);
        }

        private string GetShortName(string str)
        {
            var startIndex = str.LastIndexOf("/");
            var endIndex = str.Length - startIndex - 1;

            if (startIndex < 0 || endIndex < 0 || endIndex <= 0)
            {
                return string.Empty;
            }

            return str.Substring(startIndex + 1, endIndex);

        }

    }
}
