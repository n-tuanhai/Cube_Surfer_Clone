using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CubeSpawner : MonoBehaviour
{
    public int numberOfCube = 1;
    public GameObject cubePrefab;

    void Start()
    {
        SpawnCube();
    }

    // private void OnEnable()
    // {
    //     EventBroker.changeCubeSkin += UpdateInteractableCube;
    // }
    //
    // private void OnDisable()
    // {
    //     EventBroker.changeCubeSkin -= UpdateInteractableCube;
    // }
    //
    // private void UpdateInteractableCube(UnlockableSkin skin)
    // {
    //     
    // }

    void Update()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying && Application.isPlaying) return;

        name = numberOfCube.ToString();

        if (transform.childCount <= 0) return;
        var list = GetComponentsInChildren<Transform>().ToList();
        list.RemoveAt(0);
        foreach (var tf in list)
        {
            DestroyImmediate(tf.gameObject);
        }
#endif
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        Gizmos.color = Color.red;
        for (int i = 0; i < numberOfCube; i++)
        {
            Gizmos.DrawWireCube(new Vector3(pos.x, pos.y + 1f * i, pos.z), GetComponent<MeshRenderer>().bounds.size);
        }
    }

    void SpawnCube()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying) return;
#endif
        Physics.Raycast(transform.position, Vector3.down, out var hit, Mathf.Infinity);
        var pos = transform.position;
        for (int i = 0; i < numberOfCube; i++)
        {
            var cub = Instantiate(cubePrefab);
            cub.transform.position = new Vector3(pos.x, pos.y + 1f * i, pos.z);
            cub.transform.rotation = hit.transform.rotation;
            cub.transform.parent = transform;
        }
    }
}