using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

public class LevelDataEditorWindow : EditorWindow
{
    public GameObject _levelData;
    private GameObject _levelMap;
    private Vector2 scroll;
    private GameObject _levelPrefab;
    private HashSet<string> _existingLevels;

    private void OnEnable()
    {
        if (_levelMap == null) _levelMap = GameObject.FindGameObjectWithTag("Level Map");
        _existingLevels = new HashSet<string>();
    }

    [MenuItem("Tools/Level Editor/Level Data Window", false, 15)]
    public static void EnableWindow()
    {
        var window = GetWindow<LevelDataEditorWindow>();
        window.titleContent = new GUIContent("Level Data Editor");
        window.Show();
    }

    private void GetListOfExistingLevels()
    {
        DirectoryInfo dirInfo = new DirectoryInfo("Assets/Prefabs/Levels");
        FileInfo[] fileInf = dirInfo.GetFiles("*.prefab");
        _existingLevels.Clear();

        foreach (FileInfo fileInfo in fileInf)
        {
            string name = fileInfo.Name.Replace(".prefab", "");
            _existingLevels.Add(name);
        }
    }

    private void OnGUI()
    {
        GetListOfExistingLevels();
        _levelMap = GameObject.FindGameObjectWithTag("Level Map");
        GUILayout.BeginVertical();
        {
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Currently Editing: ");
                GUILayout.Label(_levelMap == null ? "None" : _levelMap.name);
                if (_levelMap != null)
                    GUILayout.Label(_existingLevels.Contains(_levelMap.name) ? "(Existing Prefab)" : "(New Unsaved)");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("New Level"))
            {
                NewLevelData();
            }

            if (GUILayout.Button("Save"))
            {
                SaveLevelData();
            }

            if (GUILayout.Button("Save as..."))
            {
                SaveAsPrefab();
            }

            GUILayout.BeginHorizontal();
            {
                _levelPrefab =
                    EditorGUILayout.ObjectField("Level Prefab", _levelPrefab, typeof(GameObject), false) as GameObject;

                if (GUILayout.Button("Load") && EditorUtility.DisplayDialog("Load Confirmation",
                    "Are you sure? Unsaved objects on scene will be overwritten!", "Load",
                    "Cancel"))
                {
                    LoadLevelData();
                }

                if (GUILayout.Button("Load Fix") && EditorUtility.DisplayDialog("Load Confirmation",
                    "Are you sure? Unsaved objects on scene will be overwritten!", "Load",
                    "Cancel"))
                {
                    LoadLevelDataFix();
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        _levelData = GameObject.FindGameObjectWithTag("Level Map");
    }

    void SaveAsPrefab()
    {
        if (_levelMap == null)
        {
            EditorUtility.DisplayDialog(
                "Save Level",
                "You Must create a level object in hierarchy first!",
                "Ok");
            return;
        }

        var path = EditorUtility.SaveFilePanel(
            "Save Level as Prefab",
            "Assets/Prefabs/Levels/",
            _levelMap.name + ".prefab",
            "prefab");

        if (path.Length != 0)
        {
            string s = path.Substring(path.LastIndexOf("Asset", StringComparison.Ordinal));
            string name = s.Substring(s.LastIndexOf("/", StringComparison.Ordinal)).Remove(0, 1);
            name = name.Substring(0, name.IndexOf(".", StringComparison.Ordinal));
            _levelMap.name = name;
            PrefabUtility.SaveAsPrefabAsset(_levelData, s);
        }
    }

    void NewLevelData()
    {
        bool confirm = true;
        if (_levelMap != null)
        {
            confirm = EditorUtility.DisplayDialog("Create New Level",
                "Are you sure? Unsaved level data will be overwritten!", "Create",
                "Cancel");
        }

        if (!confirm) return;

        //load template
        GameObject levelTemplate = (GameObject) Resources.Load("LevelEditorResources/Level Template");

        //destroy existing level
        if (_levelData != null) DestroyImmediate(_levelData);

        //spawn & assign level
        var lvl = Instantiate(levelTemplate, levelTemplate.transform.position, levelTemplate.transform.rotation);
        lvl.name = lvl.name.Replace("(Clone)", "");
        _levelData = _levelData = GameObject.FindGameObjectWithTag("Level Map");
    }

    void LoadLevelData()
    {
        if (_levelPrefab == null)
        {
            EditorUtility.DisplayDialog("Load Failed",
                "No level prefab provided!", "Cancel");
            return;
        }

        //destroy existing level
        if (_levelData != null) DestroyImmediate(_levelData);

        //load & assign level prefab
        var lvl = (GameObject) PrefabUtility.InstantiatePrefab(_levelPrefab);
        lvl.transform.position = _levelPrefab.transform.position;
        lvl.transform.rotation = _levelPrefab.transform.rotation;
        PrefabUtility.UnpackPrefabInstance(lvl, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        lvl.transform.SetAsLastSibling();
        lvl.name = lvl.name.Replace("(Clone)", "");
        _levelData = _levelData = GameObject.FindGameObjectWithTag("Level Map");
    }

    void LoadLevelDataFix()
    {
        if (_levelPrefab == null)
        {
            EditorUtility.DisplayDialog("Load Failed",
                "No level prefab provided!", "Cancel");
            return;
        }

        if (_levelData != null) DestroyImmediate(_levelData);

        var lvlTemplatePrefab = (GameObject) Resources.Load("LevelEditorResources/Level Template");
        var lvlTemplate =
            Instantiate(lvlTemplatePrefab, lvlTemplatePrefab.transform.position, lvlTemplatePrefab.transform.rotation);


        lvlTemplate.transform.position = lvlTemplate.transform.position;
        lvlTemplate.transform.rotation = lvlTemplate.transform.rotation;
        lvlTemplate.transform.SetAsLastSibling();
        
        //ground
        RespawnPrefabs(_levelPrefab.transform.GetChild(0), lvlTemplate.transform.GetChild(0));
        
        //rotating platform check
        foreach (Transform tr in _levelPrefab.transform.GetChild(0))
        {
            if (tr.name.Contains("Rotating Platform"))
            {
                RespawnPrefabs(tr.GetChild(1), lvlTemplate.transform.GetChild(0).GetChild(tr.GetSiblingIndex()).GetChild(1));
            }
        }
        
        //obstacles
        RespawnPrefabs(_levelPrefab.transform.GetChild(1), lvlTemplate.transform.GetChild(1));
        
        //interactables
        RespawnPrefabs(_levelPrefab.transform.GetChild(2), lvlTemplate.transform.GetChild(2));
        
        lvlTemplate.name = _levelPrefab.name.Replace("(Clone)", "");
        _levelData = _levelData = GameObject.FindGameObjectWithTag("Level Map");
    }

    void RespawnPrefabs(Transform sourceParent, Transform newParent)
    {
        foreach (Transform child in sourceParent)
        {
            var prefabName = child.name;
            if (prefabName.Contains("("))
            {
                prefabName = prefabName.Remove(prefabName.IndexOf("(", StringComparison.Ordinal) - 1);
            }
            
            GameObject prefab;
            if (int.TryParse(prefabName, out int n))
            {
                prefab = (GameObject) Resources.Load("LevelEditorResources/Cube Spawner");                
            }
            else prefab = (GameObject) Resources.Load($"LevelEditorResources/{prefabName}");
            
            var spawnObject = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
            if (n != 0) spawnObject.GetComponent<CubeSpawner>().numberOfCube = n;
            spawnObject.transform.position = child.position;
            spawnObject.transform.rotation = child.rotation;
            spawnObject.transform.localScale = child.localScale;

            spawnObject.transform.parent = newParent;
        }
    }
    
    void SaveLevelData()
    {
        if (_levelMap == null)
        {
            EditorUtility.DisplayDialog(
                "Save Level",
                "You Must create a level object in hierarchy first!",
                "Ok");
            return;
        }

        if (_existingLevels.Contains(_levelMap.name))
        {
            if (!EditorUtility.DisplayDialog("Save Confirmation",
                "Are you sure? Level data will be overwritten!", "Save",
                "Cancel")) return;
        }

        PrefabUtility.SaveAsPrefabAsset(_levelData, $"Assets/Prefabs/Levels/{_levelData.name}.prefab");
    }
}