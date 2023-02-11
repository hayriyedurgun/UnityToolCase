using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

namespace Assets.Editor
{
    public class ReportEditorWindow : EditorWindow
    {
        private List<string> m_Materials;
        private List<string> m_Models;
        private List<string> m_AnimationControllers;
        private Vector2 m_ScrollPos;

        public void Load(List<string> materials, List<string> models, List<string> animControllers)
        {
            m_Materials = materials;
            m_Models = models;
            m_AnimationControllers = animControllers;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            {
                GUILayout.Space(10);

                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, false, false);
                {
                    GUILayout.Label("Materials", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    foreach (var material in m_Materials)
                    {
                        EditorGUILayout.LabelField(material);
                    }

                    DrawUILine();
                    EditorGUI.indentLevel--;


                    GUILayout.Label("Models", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    foreach (var model in m_Models)
                    {
                        EditorGUILayout.LabelField(model);
                    }

                    DrawUILine();
                    EditorGUI.indentLevel--;

                    EditorGUI.indentLevel++;
                    GUILayout.Label("Animation Controllers", EditorStyles.boldLabel);
                    foreach (var anim in m_AnimationControllers)
                    {
                        EditorGUILayout.LabelField(anim);
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

    }
}
