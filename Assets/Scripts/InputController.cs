using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float speed = 0.3f;
    private float? _lastMousePoint;
    
    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePoint = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _lastMousePoint = null;
        }

        if (_lastMousePoint != null)
        {
            float difference = Input.mousePosition.x - _lastMousePoint.Value;

            _player.transform.Translate(difference * speed * Time.deltaTime, 0,0 );
            
            var position = _player.transform.localPosition;
            position = new Vector3(Mathf.Clamp(position.x, -1.94f, 1.94f), position.y,
                position.z);
            _player.transform.localPosition = position;
            _lastMousePoint = Input.mousePosition.x;
        }

        // if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        // {
        //     Vector2 touchPosition = Input.GetTouch(0).deltaPosition;
        //     transform.Translate(touchPosition.x * speed * Time.deltaTime, 0, 0);
        // }
    }
}
