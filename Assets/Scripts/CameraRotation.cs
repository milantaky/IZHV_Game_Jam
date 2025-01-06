using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 0.2f;

    // Update is called once per frame
    void Update()
    {
        
        while (Input.GetKey("a"))
        {
            transform.Rotate(0, -rotationSpeed, 0);  
            break; 
        }
        
        while (Input.GetKey("d")) 
        {
            transform.Rotate(0, rotationSpeed, 0); 
            break;
        }
    }
}
