using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using PathCreation;
using PathCreation.Examples;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    private int _numberOfCube;
    private int _lastNumberOfCube;
    private int _cubeDifference;
    private List<Transform> _cubeList;

    private GameObject _cubePool;
    [Inject(InjectFrom.Anywhere)] public Player _player;
    [Inject(InjectFrom.Anywhere)] public CharacterModel charModel;
    private TrailRenderer _Trail;
    public Player Player => _player;
    public int NumberOfCube => _numberOfCube;

    private PathFollower _pathFollower;
    public GameObject cubeInTowerPrefab;
    public GameObject playerPrefab;
    public GameObject trailPrefab;
    public GameObject cubeOnMapPrefab;
    public GameObject winEffects;

    public PathFollower pathFollower => _pathFollower;

    private void OnEnable()
    {
        EventBroker.upgradeCubeTower += UpgradeCubeTower;
        EventBroker.changePlayerSkin += ChangePlayerSkin;
        EventBroker.changeCubeSkin += ChangeCubeSkin;
        EventBroker.levelWin += EnableWinEffects;
    }

    private void OnDisable()
    {
        EventBroker.upgradeCubeTower -= UpgradeCubeTower;
        EventBroker.changePlayerSkin -= ChangePlayerSkin;
        EventBroker.changeCubeSkin -= ChangeCubeSkin;
        EventBroker.levelWin -= EnableWinEffects;
    }

    private void Start()
    {
        winEffects.SetActive(false);
        _Trail = FindObjectOfType<TrailRenderer>();
        _cubeList = new List<Transform>();
        _cubePool = GameObject.FindGameObjectWithTag("Pool");
        _pathFollower = GetComponent<PathFollower>();
    }

    private void Update()
    {
        if (GetCubeDifference() != 0)
        {
            EventBroker.CallNumberOfCubeChanged(GetCubeDifference());
            //Debug.Log($"{_numberOfCube}, {_lastNumberOfCube}, {GetCubeDifference()}");
            _lastNumberOfCube = _numberOfCube;
        }

        if (GameManager.Instance.GameState != GameManager.State.Running) return;
        if (_pathFollower.Pressed) GetComponent<InputController>().enabled = true;
        if (_numberOfCube == 0)
        {
            StopRunning();
            SendEndLevel();
        }
    }

    public void SendLevelLose()
    {
        GameManager.Instance.LevelLose();
    }

    public void SendEndLevel()
    {
        GameManager.Instance.EndLevel();
    }

    public void SendLevelWin()
    {
        GameManager.Instance.LevelWin();
    }

    public void SendGoalSequence()
    {
        GameManager.Instance.StartGoalSequence();
    }

    public int GetCubeDifference()
    {
        return _numberOfCube - _lastNumberOfCube;
    }

    public void StopRunning()
    {
        _pathFollower.endOfPathInstruction = EndOfPathInstruction.Stop;
        GetComponent<InputController>().enabled = false;
    }

    public void SpeedBoost()
    {
        _pathFollower.IsBoosted = true;
    }

    public void RemoveCubeFromTower(Transform cube)
    {
        cube.SetParent(_cubePool.transform, true);
        _cubeList.Remove(cube);
        _numberOfCube = _cubeList.Count;
    }

    public void GemCollected()
    {
        GameManager.Instance.IncreaseGem();
    }

    public void AddCubeToTower(int amount)
    {
        _lastNumberOfCube = _numberOfCube;
        if (amount > 0)
        {
            Vector3 playerPos = _player.transform.position;
            _player.transform.position = new Vector3(playerPos.x, playerPos.y + amount * 1.0f, playerPos.z);
            var position = _player.transform.position;
            for (int i = 1; i <= amount; i++)
            {
                float offset = 1.0f;
                var temp = Instantiate(cubeInTowerPrefab,
                    new Vector3(position.x, position.y + 0.5f - i * offset, playerPos.z),
                    Quaternion.identity);
                temp.transform.SetParent(_player.transform.parent.transform, true);
                temp.transform.GetChild(0).gameObject.SetActive(true);
                _cubeList.Add(temp.transform);
                
                EventBroker.CallCollectCubeFX(temp.transform.position);
            }

            _numberOfCube = _cubeList.Count;
        }
    }

    public void InitStartingPos(int startAmount)
    {
        _cubeList.Clear();
        AddCubeToTower(startAmount);
        _numberOfCube = _cubeList.Count;
        _lastNumberOfCube = _numberOfCube;
        winEffects.SetActive(false);

        _pathFollower.pathCreator = null;
        _pathFollower.Pressed = false;
        _pathFollower.endOfPathInstruction = EndOfPathInstruction.Loop;

        GetComponent<InputController>().enabled = false;
    }

    private void UpgradeCubeTower()
    {
        AddCubeToTower(1);
        _lastNumberOfCube = _cubeList.Count;
    }

    private void EnableWinEffects()
    {
        winEffects.SetActive(true);
    }

    private void ChangePlayerSkin(UnlockableSkin charSkin)
    {
        var skinnedMeshRenderer = playerPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        var newMeshRenderer = charSkin.skinModel.GetComponentInChildren<SkinnedMeshRenderer>();
        var currentSkinnedMeshRender = charModel.GetComponent<SkinnedMeshRenderer>();
        // update mesh
        skinnedMeshRenderer.sharedMesh = newMeshRenderer.sharedMesh;
        currentSkinnedMeshRender.sharedMesh = newMeshRenderer.sharedMesh;
        //Transform[] children = transform.GetComponentsInChildren<Transform> (true);
        // sort bones.
        // Transform[] bones = new Transform[newMeshRenderer.bones.Length];
        // for (int boneOrder = 0; boneOrder < newMeshRenderer.bones.Length; boneOrder++) {
        //     bones [boneOrder] = Array.Find<Transform> (children, c => c.name.Equals(newMeshRenderer.bones [boneOrder].name));
        // }
        // skinnedMeshRenderer.bones = bones;
    }

    private void ChangeCubeSkin(UnlockableSkin cubeSkin)
    {
        cubeInTowerPrefab.GetComponent<MeshRenderer>().material = cubeSkin.skinMat;
        foreach (var cube in _cubeList)
        {
            cube.gameObject.GetComponent<MeshRenderer>().material = cubeSkin.skinMat;
        }

        trailPrefab.GetComponent<TrailRenderer>().colorGradient = cubeSkin.trailColor;
        _Trail.gameObject.GetComponent<TrailRenderer>().colorGradient = cubeSkin.trailColor;

        cubeOnMapPrefab.GetComponent<MeshRenderer>().material = cubeSkin.skinMat;
        var tmp = FindObjectsOfType<CubeInteractable>().ToList();
        foreach (var ci in tmp)
        {
            ci.GetComponent<MeshRenderer>().material = cubeSkin.skinMat;
        }
    }
}