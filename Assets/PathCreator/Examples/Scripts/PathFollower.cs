using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed;
        private float distanceTravelled;
        public float speedBoostDuration;
        private bool isBoosted;
        private float timer;
        private bool pressed = false;
        [SerializeField] private float speedIncreasedAmount = 0.5f;

        public GameObject boostEffect;
        
        public bool IsBoosted
        {
            get => isBoosted;
            set => isBoosted = value;
        }

        public float DistanceTravelled
        {
            get => distanceTravelled;
            set => distanceTravelled = value;
        }

        public bool Pressed
        {
            get => pressed;
            set => pressed = value;
        }

        void Start()
        {
            distanceTravelled = 0;
            pressed = false;
            isBoosted = false;
            boostEffect.SetActive(false);
            timer = 0;
            GetPath();
        }

        private void Update()
        {
            if (!pressed) return;

            if (pathCreator != null && endOfPathInstruction != EndOfPathInstruction.Stop)
            {
                distanceTravelled += speed * Time.deltaTime;
                var curPos = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.position = new Vector3(curPos.x, transform.position.y, curPos.z);
                var curRot = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = Quaternion.Euler(curRot.eulerAngles.x, curRot.eulerAngles.y, curRot.eulerAngles.z);
            }

            SpeedBoostCheck();
        }

        public void GetPath()
        {
            pathCreator = FindObjectOfType<PathCreator>();
        }

        private void SpeedBoostCheck()
        {
            if (isBoosted)
            {
                speed += speedIncreasedAmount;
                timer += Time.deltaTime;
                boostEffect.SetActive(true);
                isBoosted = false;
            }

            if (timer > 0 && timer < speedBoostDuration)
            {
                timer += Time.deltaTime;
            }

            if (timer >= speedBoostDuration)
            {
                boostEffect.SetActive(false);
                speed -= speedIncreasedAmount;
                timer = 0;
            }
        }

        // IEnumerator DelayAndRun(float time)
        // {
        //     yield return new WaitForSecondsRealtime(time);
        //
        //     while (endOfPathInstruction != EndOfPathInstruction.Stop)
        //     {
        //         distanceTravelled += speed * Time.deltaTime;
        //         transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        //         transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        //         yield return null;
        //     }
        // }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}