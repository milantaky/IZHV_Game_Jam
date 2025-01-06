using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 0.2f;
    public Vector3 center = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        // Horizontal
        while (Input.GetKey("a"))
        {
            transform.RotateAround(center, Vector3.up, rotationSpeed);
            break; 
        }
        
        while (Input.GetKey("d")) 
        {
            transform.RotateAround(center, Vector3.down, rotationSpeed);
            break;
        }
        
        // Vertical
        // Vector3 cameraRotation = Camera.main.transform.rotation.eulerAngles;
        //
        // while (Input.GetKey("w")) 
        // {
        //     if (cameraRotation.x < 345)
        //     {
        //         transform.RotateAround(center, new Vector3(-1,0,0), rotationSpeed);
        //     }
        //     break;
        // }
        //
        // while (Input.GetKey("s"))
        // {
        //     if (cameraRotation.x > 15)
        //     {
        //         transform.RotateAround(center, new Vector3(1,0,0), rotationSpeed);
        //     }
        //     break;
        // }
        
    }
}
