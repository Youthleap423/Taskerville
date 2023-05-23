using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;

namespace RainbowArt.CleanFlatUI
{
    [CustomEditor(typeof(Notification))]
    public class NotificationEditor : Editor
    {
        SerializedProperty icon;
        SerializedProperty title;
        SerializedProperty description;
        SerializedProperty animator;
        SerializedProperty showTime;

        protected virtual void OnEnable()
        {
            icon = serializedObject.FindProperty("icon");
            title = serializedObject.FindProperty("title");  
            description = serializedObject.FindProperty("description");         
            animator = serializedObject.FindProperty("animator");
            showTime = serializedObject.FindProperty("showTime");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(icon); 
            EditorGUILayout.PropertyField(title); 
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(animator); 
            EditorGUILayout.PropertyField(showTime);         
            serializedObject.ApplyModifiedProperties();           
        }
    }
}
