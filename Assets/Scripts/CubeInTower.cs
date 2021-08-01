using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation.Examples;
using UnityEngine;

public class CubeInTower : MonoBehaviour
{
    public PlayerController _playerController;
    public Player player;
    private Rigidbody _rBody;
    public ParticleSystem lavaSplashPrefab;
    public ParticleSystem gemSplashPrefab;
    
    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        player = FindObjectOfType<Player>();
        _rBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //remove vertical velocity
        var currentVelocity = _rBody.velocity;
        if (currentVelocity.y <= 0f) return;
        currentVelocity.y = 0f;
        _rBody.velocity = currentVelocity;
    }

    private void Update()
    {
        if (!transform.parent.CompareTag("Cube Tower")) return;

        if (_rBody.IsSleeping()) _rBody.WakeUp();

        //self adjust
        var position = player.transform.position;

        transform.position = new Vector3(position.x, transform.position.y,
            position.z);
        transform.rotation = player.transform.rotation;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            _playerController.RemoveCubeFromTower(transform);
        }

        if (other.gameObject.CompareTag("Goal"))
        {
            _playerController.SendGoalSequence();
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Level Win"))
        {
            _playerController.SendLevelWin();
            _playerController.StopRunning();
            _playerController.SendEndLevel();
        }

        if (other.gameObject.CompareTag("Gem"))
        {
            if (!transform.parent.CompareTag("Cube Tower")) return;
            
            other.gameObject.GetComponent<MeshCollider>().enabled = false;
            ParticleSystem effect = Instantiate(gemSplashPrefab, other.transform.position, gemSplashPrefab.transform.rotation);
            Destroy(effect.gameObject,effect.main.duration);
            Destroy(other.gameObject);
            EventBroker.CallCollectGemFX(other.gameObject.transform.position);
            _playerController.GemCollected();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            _playerController.AddCubeToTower(other.GetComponent<CubeSpawner>().numberOfCube);
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Lava"))
        {
            ParticleSystem effect = Instantiate(lavaSplashPrefab, this.transform.position, lavaSplashPrefab.transform.rotation);
            Destroy(effect.gameObject,effect.main.duration);
            gameObject.SetActive(false);
            _playerController.RemoveCubeFromTower(transform);
        }

        if (other.gameObject.CompareTag("Speed Zone"))
        {
            _playerController.SpeedBoost();
        }
    }
}