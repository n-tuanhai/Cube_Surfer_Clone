using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using PathCreation.Examples;
using Unity.Collections;
using UnityEngine;

public class CharacterAnim : MonoBehaviour
{
    private Animator anim;
    private float lastY;
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Won = Animator.StringToHash("Won");
    private int cubeDifference;
    private Rigidbody mainBody;

    [Inject(InjectFrom.Anywhere)] public PlayerController _playerController;

    void Start()
    {
        EventBroker.levelWin += SetWonAnimation;
        EventBroker.numberOfCubeChanged += SetCubeDifference;
        mainBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        EventBroker.numberOfCubeChanged -= SetCubeDifference;
    }

    private void SetCubeDifference(int amount)
    {
        cubeDifference = amount;
    }

    void Update()
    {
        if (mainBody == null) mainBody = GetComponent<Rigidbody>();
        if (anim == null) anim = GetComponent<Animator>();

        if (cubeDifference > 0)
        {
            anim.SetTrigger(Jump);
            cubeDifference = 0;
        }
        // else if (cubeDifference < 0)
        // {
        //     anim.SetTrigger(Fall);
        //     cubeDifference = 0;
        // }
        //
        if (mainBody.velocity.y > 5.0f)
        {
            anim.SetTrigger(Fall);
            mainBody.velocity = new Vector3(0 ,0, 0);
        }
            
    }

    private void OnDestroy()
    {
        EventBroker.levelWin -= SetWonAnimation;
    }

    private void SetWonAnimation()
    {
        anim.SetBool(Won, true);
    }
}