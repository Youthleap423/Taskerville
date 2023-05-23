
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;

namespace RainbowArt.CleanFlatUI
{
    [CustomEditor(typeof(ModalWindow))]
    public class ModalWindowEditor : Editor
    {
        SerializedProperty iconTitle;
        SerializedProperty title;
        SerializedProperty buttonClose;
        SerializedProperty description; 
        SerializedProperty view;
        SerializedProperty content;        
        SerializedProperty buttonBar;
        SerializedProperty buttonConfirm; 
        SerializedProperty buttonCancel;           
        SerializedProperty animator;               
        SerializedProperty onConfirmClick;
        SerializedProperty onCancelClick;  

        protected virtual void OnEnable()
        {
            iconTitle = serializedObject.FindProperty("iconTitle");
            title = serializedObject.FindProperty("title");  
            buttonClose = serializedObject.FindProperty("buttonClose");
            description = serializedObject.FindProperty("description");
            view = serializedObject.FindProperty("view");  
            content = serializedObject.FindProperty("content");  
            buttonBar = serializedObject.FindProperty("buttonBar");  
            buttonConfirm = serializedObject.FindProperty("buttonConfirm");  
            buttonCancel = serializedObject.FindProperty("buttonCancel");  
            animator = serializedObject.FindProperty("animator");
            onConfirmClick = serializedObject.FindProperty("onConfirmClick");   
            onCancelClick = serializedObject.FindProperty("onCancelClick");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(iconTitle); 
            EditorGUILayout.PropertyField(title); 
            EditorGUILayout.PropertyField(buttonClose); 
            EditorGUILayout.PropertyField(description); 
            EditorGUILayout.PropertyField(view); 
            EditorGUILayout.PropertyField(content); 
            EditorGUILayout.PropertyField(buttonBar); 
            EditorGUILayout.PropertyField(buttonConfirm); 
            EditorGUILayout.PropertyField(buttonCancel); 
            EditorGUILayout.PropertyField(animator); 
            EditorGUILayout.PropertyField(onConfirmClick); 
            EditorGUILayout.PropertyField(onCancelClick);                
            serializedObject.ApplyModifiedProperties();           
        }
    }
}
