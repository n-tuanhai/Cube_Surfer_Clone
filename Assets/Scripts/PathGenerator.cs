using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;


[RequireComponent(typeof(PathCreator))]
[ExecuteAlways]
public class PathGenerator : MonoBehaviour
{
    public bool closedLoop = false;
    public List<Transform> waypoints;
    GlobalDisplaySettings globalEditorDisplaySettings;
    private BezierPath bezierPath;
    private bool isInitialized;

    private void UpdatePath()
    {
        waypoints.Clear();

        var wp = GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(p => p.transform.root.GetSiblingIndex())
            .ThenBy(p => p.transform.parent.transform.parent.GetSiblingIndex())
            .ThenBy(p => p.transform.parent.GetSiblingIndex()).ThenBy(p => p.transform.GetSiblingIndex()).ToList();


        foreach (var p in wp)
        {
            waypoints.Add(p.transform);
        }

        if (waypoints.Count > 0)
        {
            bezierPath = new BezierPath(waypoints, closedLoop, PathSpace.xyz) {GlobalNormalsAngle = 90};
            GetComponent<PathCreator>().bezierPath = bezierPath;
        }
    }
#if UNITY_EDITOR
    private void Start()
    {
        UpdatePath();
    }

    void Update()
    {
        if (EditorApplication.isPlaying && Application.isPlaying) return;
        UpdatePath();
    }
#endif
}