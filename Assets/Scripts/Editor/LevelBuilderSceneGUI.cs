using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using InspectorGadgets.Editor;
using PathCreation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[InitializeOnLoad]
static class LevelBuilderInitializer
{
    private const string ProjectOpened = "ProjectOpened";

    static LevelBuilderInitializer()
    {
        // if (!SessionState.GetBool(ProjectOpened, false))
        // {
        //     SessionState.SetBool(ProjectOpened, true);
        LevelBuilderSceneGUI.EnableGUI();
        //}
    }
}

public class LevelBuilderSceneGUI : EditorWindow
{
    private static LevelBuilderSceneGUI _instance;
    private GameObject _levelManager;
    private static bool isLoaded;
    private GameObject _previouslySpawned;
    private GameObject _levelGround;
    private GameObject _levelObstacle;
    private GameObject _levelInteractable;
    private GameObject _startingPlatform;
    private Vector3 _direction;

    private HashSet<string> _levelPlatform;
    private HashSet<string> _levelObstacleSet;
    private HashSet<string> _levelInteractableSet;

    private int leftId, rightId, forwardId, backId, upId, downId;
    private bool cached;

    int _selGridInt;
    readonly string[] _selStrings = {"Gem", "Cube Spawner", "Speed Boost", "Ramp", "Cube Obstacle"};
    private bool _toggleBuildMode;

    private float _spacing;
    private bool debugMode;
    private bool _showWindow;

    private void InitHashSets()
    {
        _levelPlatform = new HashSet<string>
        {
            "Straight Path",
            "Turn",
            "End Stair",
            "Rotating Platform",
            "Two Sided Lava",
            "One Sided Lava",
            "Lava Pool",
            "Half Path Left",
            "Half Path Right",
            "Straight Path Gap"
        };

        _levelObstacleSet = new HashSet<string>
        {
            "Cube Obstacle",
            "Ramp"
        };
        _levelInteractableSet = new HashSet<string>
        {
            "Gem",
            "Cube Spawner",
            "Speed Boost",
            "Goal"
        };
    }

    static void Init()
    {
        if (_instance == null)
            CreateInstance<LevelBuilderSceneGUI>().Initialize();
        else
            _instance.Initialize();
    }

    void Initialize()
    {
        InitHashSets();
        _toggleBuildMode = false;
        _direction = Vector3.forward;
        isLoaded = false;
        _instance = this;
        _selGridInt = 0;
        _showWindow = false;
    }

    [MenuItem("Tools/Level Editor/Enable", false, 0)]
    public static void EnableGUI()
    {
        if (!isLoaded)
        {
            Init();
            _instance.Show();
            _instance.Close();
            SceneView.duringSceneGui += _instance.OnScene;
            isLoaded = true;
        }
    }

    [MenuItem("Tools/Level Editor/Disable", false, 1)]
    public static void DisableGUI()
    {
        if (isLoaded)
        {
            SceneView.duringSceneGui -= _instance.OnScene;
            isLoaded = false;
        }
    }

    private void OnScene(SceneView sceneView)
    {
        if (EditorApplication.isPlaying || Application.isPlaying) return;

        if (_levelManager == null) _levelManager = GameObject.FindGameObjectWithTag("Level Map");
        if (_startingPlatform == null)
        {
            _startingPlatform = GameObject.Find("Starting Platform");
            _previouslySpawned = _startingPlatform;
        }

        if (_levelManager == null) return;

        if (_levelGround == null) _levelGround = GameObject.FindGameObjectWithTag("Level Ground");
        if (_levelObstacle == null) _levelObstacle = GameObject.FindGameObjectWithTag("Level Obstacle");
        if (_levelInteractable == null) _levelInteractable = GameObject.FindGameObjectWithTag("Level Interactable");

        ReorderPath();

        #region Snap Control

        if (!cached)
        {
            leftId = GUIUtility.GetControlID(FocusType.Passive);
            rightId = GUIUtility.GetControlID(FocusType.Passive);
            forwardId = GUIUtility.GetControlID(FocusType.Passive);
            backId = GUIUtility.GetControlID(FocusType.Passive);
            upId = GUIUtility.GetControlID(FocusType.Passive);
            downId = GUIUtility.GetControlID(FocusType.Passive);
            cached = true;
        }

        if (Selection.activeTransform != null && (Selection.activeTransform.parent == _levelGround.transform ||
                                                  Selection.activeTransform.parent == _levelObstacle.transform ||
                                                  Selection.activeTransform.parent == _levelInteractable.transform))
        {
            Transform transform = Selection.activeTransform;
            Handles.color = Color.yellow;
            var position1 = transform.position;
            Handles.DrawWireCube(position1, transform.GetComponent<MeshRenderer>().bounds.size);

            Vector3 posLeft = position1 +
                              Vector3.left * transform.GetComponent<MeshRenderer>().bounds.extents.x;
            Vector3 posRight = position1 +
                               Vector3.right * transform.GetComponent<MeshRenderer>().bounds.extents.x;
            Vector3 posForward = position1 +
                                 Vector3.forward * transform.GetComponent<MeshRenderer>().bounds.extents.z;
            Vector3 posBackward = position1 +
                                  Vector3.back * transform.GetComponent<MeshRenderer>().bounds.extents.z;
            Vector3 posUp = position1 + Vector3.up * transform.GetComponent<MeshRenderer>().bounds.extents.y;
            Vector3 posDown = position1 +
                              Vector3.down * transform.GetComponent<MeshRenderer>().bounds.extents.y;

            if (!_toggleBuildMode)
            {
                switch (Event.current.type)
                {
                    case EventType.Repaint:
                    {
                        Handles.color = Color.yellow;
                        Handles.ArrowHandleCap(leftId, posLeft, Quaternion.LookRotation(Vector3.left),
                            HandleUtility.GetHandleSize(posLeft), EventType.Repaint);

                        Handles.ArrowHandleCap(rightId, posRight, Quaternion.LookRotation(Vector3.right),
                            HandleUtility.GetHandleSize(posRight), EventType.Repaint);

                        Handles.ArrowHandleCap(forwardId, posForward, Quaternion.LookRotation(Vector3.forward),
                            HandleUtility.GetHandleSize(posForward), EventType.Repaint);

                        Handles.ArrowHandleCap(backId, posBackward, Quaternion.LookRotation(Vector3.back),
                            HandleUtility.GetHandleSize(posBackward), EventType.Repaint);

                        Handles.ArrowHandleCap(upId, posUp, Quaternion.LookRotation(Vector3.up),
                            HandleUtility.GetHandleSize(posUp), EventType.Repaint);

                        Handles.ArrowHandleCap(downId, posDown, Quaternion.LookRotation(Vector3.down),
                            HandleUtility.GetHandleSize(posDown), EventType.Repaint);
                        break;
                    }
                    case EventType.Layout:
                    {
                        Handles.ArrowHandleCap(leftId, posLeft, Quaternion.LookRotation(Vector3.left),
                            HandleUtility.GetHandleSize(posLeft), EventType.Layout);

                        Handles.ArrowHandleCap(rightId, posRight, Quaternion.LookRotation(Vector3.right),
                            HandleUtility.GetHandleSize(posRight), EventType.Layout);

                        Handles.ArrowHandleCap(forwardId, posForward, Quaternion.LookRotation(Vector3.forward),
                            HandleUtility.GetHandleSize(posForward), EventType.Layout);

                        Handles.ArrowHandleCap(backId, posBackward, Quaternion.LookRotation(Vector3.back),
                            HandleUtility.GetHandleSize(posBackward), EventType.Layout);

                        Handles.ArrowHandleCap(upId, posUp, Quaternion.LookRotation(Vector3.up),
                            HandleUtility.GetHandleSize(posUp), EventType.Layout);

                        Handles.ArrowHandleCap(downId, posDown, Quaternion.LookRotation(Vector3.down),
                            HandleUtility.GetHandleSize(posDown), EventType.Layout);
                        break;
                    }

                    case EventType.MouseDown:
                    {
                        int id = HandleUtility.nearestControl;
                        if (id == forwardId) Snap(Vector3.forward);
                        if (id == backId) Snap(Vector3.back);
                        if (id == leftId) Snap(Vector3.left);
                        if (id == rightId) Snap(Vector3.right);
                        if (id == upId) Snap(Vector3.up);
                        if (id == downId) Snap(Vector3.down);
                        break;
                    }
                    case EventType.MouseMove:
                        HandleUtility.Repaint();
                        break;
                }
            }
        }

        #endregion

        Handles.BeginGUI();

        #region Builder Menu

        var pixelRect = sceneView.camera.pixelRect;
        var builderRect = new Rect((pixelRect.width * 0.001f), 200,
            (pixelRect.width * 0.1f),
            pixelRect.height / 1.5f);
        GUILayout.BeginArea(builderRect);

        var rect = EditorGUILayout.BeginVertical();
        GUI.color = Color.yellow;
        GUI.Box(rect, GUIContent.none);

        GUI.color = Color.white;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Level End");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("End Stair"))
        {
            SpawnObject("End Stair");
        }

        if (GUILayout.Button("Goal"))
        {
            SpawnObject("Goal");
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Platform Spawner");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // GUI.color = Color.red;

        if (GUILayout.Button("Straight Path"))
        {
            SpawnObject("Straight Path");
        }


        if (GUILayout.Button("Turn Left"))
        {
            SpawnObject("Turn Left");
        }

        if (GUILayout.Button("Turn Right"))
        {
            SpawnObject("Turn Right");
        }

        if (GUILayout.Button("Rotating Platform"))
        {
            SpawnObject("Rotating Platform");
        }

        if (GUILayout.Button("Two Sided Lava"))
        {
            SpawnObject("Two Sided Lava");
        }

        if (GUILayout.Button("One Sided Lava"))
        {
            SpawnObject("One Sided Lava");
        }

        if (GUILayout.Button("Lava Pool"))
        {
            SpawnObject("Lava Pool");
        }
        
        if (GUILayout.Button("Half Path Left"))
        {
            SpawnObject("Half Path Left");
        }
        if (GUILayout.Button("Half Path Right"))
        {
            SpawnObject("Half Path Right");
        }
        if (GUILayout.Button("Straight Path Gap"))
        {
            SpawnObject("Straight Path Gap");
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Obstacle/\nInteratable Spawner");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        _selGridInt = GUILayout.SelectionGrid(_selGridInt, _selStrings, 1);
        _toggleBuildMode = GUILayout.Toggle(_toggleBuildMode, "Spawn Mode", "Button");
        if (_toggleBuildMode)
        {
            Event e = Event.current;
            //Debug.Log(Event.current.type);
            if ((e.rawType == EventType.MouseDown || e.rawType == EventType.MouseDrag) && e.button == 0)
            {
                var objectToSpawn = (GameObject) Resources.Load($"LevelEditorResources/{_selStrings[_selGridInt]}");
                var parent = _levelInteractableSet.Contains(objectToSpawn.name)
                    ? _levelInteractable.transform
                    : _levelObstacle.transform;

                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
                {
                    var spawnPos = new Vector3(hit.point.x, hit.point.y + 3.0f, hit.point.z);
                    var go = (GameObject) PrefabUtility.InstantiatePrefab(objectToSpawn);
                    go.transform.position = spawnPos;
                    go.transform.parent = parent;
                    Snap(Vector3.down, go.transform);
                    Undo.RegisterCreatedObjectUndo(go, "spawn interactable");
                }

                e.Use();
            }
        }

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();

        #endregion

        #region Utility Menu

        var utilRect = new Rect((pixelRect.width * 0.85f), 300,
            (pixelRect.width * 0.15f),
            pixelRect.height / 3);
        GUILayout.BeginArea(utilRect);
        {
            var rect2 = EditorGUILayout.BeginVertical();
            {
                GUI.color = Color.yellow;
                GUI.Box(rect2, GUIContent.none);
                GUI.color = Color.white;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Utility");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUI.backgroundColor = Color.red;

                    if (GUILayout.Button("Left 90")) Rotate(Vector3.down);
                    if (GUILayout.Button("Right 90")) Rotate(Vector3.up);
                }
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Snap height \n to previous object of \n same category")) SnapHeight();

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Align X")) AlignTransform("x");
                    if (GUILayout.Button("Align Y")) AlignTransform("y");
                    if (GUILayout.Button("Align Z")) AlignTransform("z");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Space between objects");
                if (GUILayout.Button($"{_spacing}")) _showWindow = !_showWindow;

                if (_showWindow)
                {
                    var windowRect = new Rect(pixelRect.width * 0.85f, 470, 10, 10);
                    GUILayout.Window(0, windowRect, ShowSpacingInput, "Spacing");
                }

                //TODO: change to proper playerpref setting
                GUILayout.EndHorizontal();
                debugMode = GUILayout.Toggle(debugMode, "Toggle playtest mode");
                var debugInt = debugMode ? 1 : 0;
                PlayerPrefs.SetInt("debug", debugInt);
            }
            EditorGUILayout.EndVertical();
        }
        GUILayout.EndArea();

        #endregion

        Handles.EndGUI();
    }

    void ShowSpacingInput(int windowID)
    {
        _spacing = EditorGUILayout.FloatField("Space", _spacing);
        if (GUILayout.Button("Done")) _showWindow = false;
    }

    void AlignTransform(string axis)
    {
        var list = Selection.gameObjects;
        if (list == null || list.Length == 1) return;

        var pivot = list[0].transform.position;
        Vector3 alignCoord;
        Vector3 alignDir;
        switch (axis)
        {
            case "x":
                alignCoord = new Vector3(_spacing, 0, 0);
                alignDir = Vector3.right;
                break;
            case "y":
                alignCoord = new Vector3(0, _spacing, 0);
                alignDir = Vector3.up;
                break;
            case "z":
                alignCoord = new Vector3(0, 0, _spacing);
                alignDir = Vector3.forward;
                break;
            default:
                alignCoord = Vector3.zero;
                alignDir = Vector3.zero;
                break;
        }

        for (int i = 1; i <= list.Length - 1; i++)
        {
            var bound = list[i - 1].GetComponent<MeshRenderer>().bounds.size;
            var boundTranslate = new Vector3(bound.x * alignDir.x, bound.y * alignDir.y, bound.z * alignDir.z);
            list[i].transform.position = new Vector3(pivot.x + (alignCoord.x + boundTranslate.x) * i,
                pivot.y + (alignCoord.y + boundTranslate.y) * i, pivot.z + (alignCoord.z + boundTranslate.z) * i);
            Undo.RecordObject(list[i].transform, $"align{i}");
        }
    }

    void SnapHeight()
    {
        var active = Selection.activeTransform;
        if (active == null || (active.parent != _levelGround.transform && active.parent != _levelObstacle.transform &&
                               active.parent != _levelInteractable.transform)) return;

        var target = Selection.activeTransform;
        var prev = target == target.parent.GetChild(0)
            ? _startingPlatform.transform
            : target.parent.GetChild(target.GetSiblingIndex() - 1);

        active.transform.position = new Vector3(active.transform.position.x, prev.transform.position.y,
            active.transform.position.z);
    }

    void Snap(Vector3 direction)
    {
        var active = Selection.activeTransform;
        if (active == null || (active.parent != _levelGround.transform && active.parent != _levelObstacle.transform &&
                               active.parent != _levelInteractable.transform)) return;

        if (Physics.Raycast(active.position, direction, out var hit, Mathf.Infinity))
        {
            var distance = (hit.distance -
                            Mathf.Abs(Vector3.Dot(active.gameObject.GetComponent<MeshRenderer>().bounds.extents,
                                direction)));

            Undo.RecordObject(Selection.activeTransform.transform, "snap");
            Selection.activeTransform.Translate(direction * distance, Space.World);
        }
    }

    void Snap(Vector3 direction, Transform obj)
    {
        var active = obj;
        if (active == null || (active.parent != _levelGround.transform && active.parent != _levelObstacle.transform &&
                               active.parent != _levelInteractable.transform)) return;

        if (Physics.Raycast(active.position, direction, out var hit, Mathf.Infinity))
        {
            var distance = (hit.distance -
                            Mathf.Abs(Vector3.Dot(active.gameObject.GetComponent<MeshRenderer>().bounds.extents,
                                direction)));

            active.Translate(direction * distance, Space.World);
        }
    }

    void ReorderPath()
    {
        if (Selection.activeTransform == null || (Selection.activeTransform.parent != _levelGround.transform)) return;

        var target = Selection.activeTransform;
        var pathPoints = target.GetChild(0);

        var prev = target == target.parent.GetChild(0)
            ? _startingPlatform.transform
            : target.parent.GetChild(target.GetSiblingIndex() - 1);

        var dist1 = Vector3.SqrMagnitude(pathPoints.GetChild(0).position - prev.position);
        var dist2 = Vector3.SqrMagnitude(pathPoints.GetChild(pathPoints.childCount - 1).position - prev.position);
        if (dist1 > dist2)
        {
            var temp = pathPoints.GetChild(pathPoints.childCount - 1);
            pathPoints.GetChild(0).SetAsLastSibling();
            temp.SetAsFirstSibling();
        }
    }

    void Rotate(Vector3 direction)
    {
        if (Selection.activeTransform == null) return;
        Undo.RecordObject(Selection.activeTransform.transform, "rotate");
        Selection.activeTransform.transform.Rotate(direction, 90);
        ReorderPath();
    }

    Vector3 SnapAxis(Vector3 vec)
    {
        float[] coords = {vec.x, vec.y, vec.z};
        float[] absCoords = {Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z)};
        float max = Mathf.Max(absCoords);
        int idx = absCoords.ToList().IndexOf(max);
        for (int i = 0; i < absCoords.Length; i++)
        {
            if (i == idx) coords[i] = coords[i] / absCoords[i];
            else coords[i] = 0;
        }

        return new Vector3(coords[0], coords[1], coords[2]);
    }

    void SpawnObject(string objectName)
    {
        //load prefab///////////////////////////////////////////////////
        GameObject parent = _levelManager;
        var tempName = objectName.Contains("Turn") ? "Turn" : objectName;
        var objectToSpawn = (GameObject) Resources.Load($"LevelEditorResources/{tempName}");
        ///////////////////////////////////////////////////////////////

        //set parent///////////////////////////////////////////////////
        if (_levelPlatform.Contains(tempName)) parent = _levelGround;
        else if (_levelObstacleSet.Contains(tempName)) parent = _levelObstacle;
        else if (_levelInteractableSet.Contains(tempName)) parent = _levelInteractable;
        ///////////////////////////////////////////////////////////////

        //get previous object position/////////////////////////////////
        var childCount = parent.transform.childCount;
        _previouslySpawned = childCount != 0
            ? parent.transform.GetChild(childCount - 1).gameObject
            : _startingPlatform;
        var position1 = _previouslySpawned.transform.position;
        ///////////////////////////////////////////////////////////////

        //set direction according to previous object///////////////////
        Transform endPointOfTurn = null;
        var wp = _levelManager.GetComponent<PathGenerator>().waypoints;
        var wp2 = _previouslySpawned.transform.GetChild(0).GetChild(0);
        var last = wp[wp.Count - 1];

        if (_previouslySpawned == _startingPlatform) _direction = Vector3.forward;
        else if (!_previouslySpawned.name.Contains("Turn"))
            _direction = SnapAxis(last.position - wp2.position);
        else
        {
            int childIndex = last.gameObject.name == "P (3)" ? 1 : 0;
            endPointOfTurn = _previouslySpawned.transform.GetChild(1).GetChild(childIndex);
            _direction = SnapAxis(endPointOfTurn.position - wp2.position);
        }
        ///////////////////////////////////////////////////////////////

        //set position to spawn new object/////////////////////////////
        var translate = Mathf.Abs(Vector3.Dot(_previouslySpawned.GetComponent<MeshRenderer>().bounds.extents,
            _direction));
        var spawnPos = position1 + translate * _direction;
        ///////////////////////////////////////////////////////////////


        //spawn/////////////////////////////////////////////////////////
        var newObject = (GameObject) PrefabUtility.InstantiatePrefab(objectToSpawn);
        newObject.transform.position = spawnPos;
        ///////////////////////////////////////////////////////////////


        //rotate and snap newly spawned object to the correct position/
        newObject.transform.rotation = objectName == "Turn Right"
            ? Quaternion.LookRotation(Vector3.Cross(_direction, Vector3.up), Vector3.up)
            : Quaternion.LookRotation(_direction, Vector3.up);

        float translate1 = Mathf.Abs(Vector3.Dot(newObject.GetComponent<MeshRenderer>().bounds.extents, _direction));
        newObject.transform.Translate(_direction * translate1, Space.World);

        if (!objectName.Contains("Turn") && endPointOfTurn != null)
            newObject.transform.position = _direction.z != 0
                ? new Vector3(endPointOfTurn.transform.position.x, newObject.transform.position.y,
                    newObject.transform.position.z)
                : new Vector3(newObject.transform.position.x, newObject.transform.position.y,
                    endPointOfTurn.transform.position.z);

        else if (objectName.Contains("Turn"))
        {
            var pivot = objectName.Contains("Left")
                ? newObject.transform.GetChild(1).GetChild(0)
                : newObject.transform.GetChild(1).GetChild(1);

            Vector3 diff = endPointOfTurn != null
                ? pivot.position - endPointOfTurn.position
                : pivot.position - _previouslySpawned.transform.position;

            newObject.transform.position = _direction.z != 0
                ? new Vector3(newObject.transform.position.x - diff.x, newObject.transform.position.y,
                    newObject.transform.position.z)
                : new Vector3(newObject.transform.position.x, newObject.transform.position.y,
                    newObject.transform.position.z - diff.z);
        }
        ///////////////////////////////////////////////////////////////

        Undo.RegisterCreatedObjectUndo(newObject, "spawn");
        newObject.transform.parent = parent.transform;
        Selection.activeTransform = newObject.transform;
    }
}