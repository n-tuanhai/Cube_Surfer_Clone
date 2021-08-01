using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailSetting : MonoBehaviour
{
    public Player player;
   
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    
    void Update()
    {
        var position = player.transform.position;

        transform.position = new Vector3(position.x, transform.position.y,
            position.z);
    }
}
