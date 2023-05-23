
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;

namespace RainbowArt.CleanFlatUI
{
    [CustomEditor(typeof(ContextMenuRightClick))]
    public class ContextMenuRightClickEditor : Editor
    {
        SerializedProperty contextMenu;  

        protected virtual void OnEnable()
        {
            contextMenu = serializedObject.FindProperty("contextMenu");  
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(contextMenu);            
            serializedObject.ApplyModifiedProperties();         
        }
    }
}
