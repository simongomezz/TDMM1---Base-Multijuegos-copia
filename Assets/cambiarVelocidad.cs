using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cambiarVelocidad : MonoBehaviour
{
    public Player playerScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)){
            playerScript.speed=+10;
        }
    }
}
