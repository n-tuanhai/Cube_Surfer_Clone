using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using PathCreation;
using PathCreation.Examples;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera myCamera;
    public float minFOV;
    public float maxFOV;
    public float minZ;
    public float fovStep;
    public float fovDuration;
    public float heightDuration;
    public float endHeightDuration;
    public float rotateSpeed;
    public float moveZStep;

    private bool startGoalSequence;
    private bool startRotate;
    private int cubeDifference;

    [Inject(InjectFrom.Anywhere)] public PlayerController _playerController;
    [Inject(InjectFrom.Anywhere)] public PathFollower pathFollower;

    private void Start()
    {
        startRotate = false;
        startGoalSequence = false;
        myCamera = GetComponent<Camera>();
        EventBroker.numberOfCubeChanged += SetCubeDifference;
    }

    private void OnDisable()
    {
        EventBroker.numberOfCubeChanged -= SetCubeDifference;
    }

    private void SetCubeDifference(int amount)
    {
        cubeDifference = amount;
    }
    
    private void Update()
    {
        if (!pathFollower.Pressed) return;
        
        if (cubeDifference != 0)
        {
            var targetFieldOfView = myCamera.fieldOfView + cubeDifference * fovStep;
            if (targetFieldOfView < minFOV) targetFieldOfView = minFOV;
            if (targetFieldOfView > maxFOV) targetFieldOfView = maxFOV;

            var targetZ = myCamera.transform.localPosition.z - cubeDifference * moveZStep;
            if (targetZ > minZ) targetZ = minZ;

            myCamera.DOFieldOfView(targetFieldOfView, fovDuration);
            myCamera.transform.DOLocalMoveZ(targetZ, fovDuration);
        }

        if (startGoalSequence)
        {
            if (cubeDifference == -1)
                transform.DOMoveY(myCamera.transform.position.y + 1, heightDuration);

            if (startRotate)
                transform.RotateAround(_playerController.Player.transform.position, Vector3.up,
                    rotateSpeed * Time.deltaTime);
        }
        cubeDifference = 0;
    }

    public void GoalSequenceCamera()
    {
        startGoalSequence = true;
    }

    public void LevelWonCamera()
    {
        startRotate = true;
        transform.DOMoveY(myCamera.transform.position.y - 4, endHeightDuration);
    }
}