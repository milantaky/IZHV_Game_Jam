using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 0.2f;
    public Vector3 center = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        
        while (Input.GetKey("a"))
        {
            transform.RotateAround(center, Vector3.up, rotationSpeed);
            break; 
        }
        
        while (Input.GetKey("d")) 
        {
            transform.RotateAround(center, Vector3.up, -rotationSpeed);
            break;
        }
    }
}
