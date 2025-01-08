using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 0.2f;
    public Vector3 center = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (!StartMenuController.isGameRunning) 
            return;
        
        // Horizontal
        while (Input.GetKey(KeyCode.A))
        {
            transform.RotateAround(center, Vector3.up, rotationSpeed);
            break; 
        }
        
        while (Input.GetKey(KeyCode.D)) 
        {
            transform.RotateAround(center, Vector3.down, rotationSpeed);
            break;
        }
        
        // Vertical
        Vector3 cameraRotation = Camera.main.transform.rotation.eulerAngles;
        Vector3 cameraPosition = Camera.main.transform.position;
        
        while (Input.GetKey(KeyCode.W)) 
        {
            if (cameraRotation.x < 345)
            {
                Vector3 helper = cameraPosition;
                helper.y++;

                Vector3 cross = Vector3.Cross(helper, cameraPosition);
                
                transform.RotateAround(center, cross, -rotationSpeed);
            }
            break;
        }
        
        while (Input.GetKey(KeyCode.S))
        {
            if (cameraRotation.x > 15)
            {
                Vector3 helper = cameraPosition;
                helper.y++;

                Vector3 cross = Vector3.Cross(helper, cameraPosition);
                
                transform.RotateAround(center, cross, rotationSpeed);
            }
            break;
        }
        
    }
}
