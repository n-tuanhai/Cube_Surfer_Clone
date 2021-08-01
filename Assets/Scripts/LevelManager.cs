using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    static LevelManager instance;
    public List<GameObject> levelPlaylist;

    void Awake()
    {
        if (instance == null)
        {
            instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
    }

    public void LoadLevel(int idx)
    {
        var level = levelPlaylist[idx];
        var levelData = GameObject.FindGameObjectWithTag("Level Map");
        //destroy existing level
        if (levelData != null) DestroyImmediate(levelData);

        //load & assign level prefab
        var lvl = Instantiate(level, level.transform.position, level.transform.rotation);
        lvl.name = lvl.name.Replace("(Clone)", "");
        //Debug.Log(lvl.name);
    }
    
#if UNITY_EDITOR 
    [ContextMenu("Auto populate playlist")]
    public void AutoPopulate()
    {
        DirectoryInfo dirInfo = new DirectoryInfo("Assets/Prefabs/Levels");
        FileInfo[] fileInf = dirInfo.GetFiles("*.prefab");
        levelPlaylist.Clear();

        foreach (FileInfo fileInfo in fileInf)
        {
            string fullPath = fileInfo.FullName.Replace(@"\", "/");
            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;

            if (prefab != null) levelPlaylist.Add(prefab);
        }
    }
#endif
}