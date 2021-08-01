using System;
using System.Collections;
using System.Collections.Generic;
using InspectorGadgets.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.XR;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    private SerializedProperty _levelPlaylist;
    private LevelManager _target;
    private ReorderableList _list;

    private void OnEnable()
    {
        _target = (LevelManager) target;
        _levelPlaylist = serializedObject.FindProperty("levelPlaylist");

        _list = new ReorderableList(serializedObject, _levelPlaylist, true, true, true, true)
        {
            drawElementCallback = DrawListItem, drawHeaderCallback = DrawHeader
        };
    }

    void DrawListItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = _list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        if (GUI.Button(new Rect(rect.x + 120, rect.y, 100, EditorGUIUtility.singleLineHeight), "Load level"))
        {
            _target.LoadLevel(index);
        }
    }

    void DrawHeader(Rect rect)
    {
        string name = "Level Playlist";
        EditorGUI.LabelField(rect, name);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Populate Playlist")) _target.AutoPopulate();
    }
}