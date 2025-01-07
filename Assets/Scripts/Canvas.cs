using UnityEngine;

public class Canvas : MonoBehaviour
{
    public int canvasSize = 1000; // 10 units * 100 pixels

    private Texture2D canvasTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        // Canvas texture
        canvasTexture = new Texture2D(canvasSize, canvasSize);
        GetComponent<Renderer>().material.mainTexture = canvasTexture;
        
        // Filling it blue to see if it works
        for (int x = 0; x < canvasSize; x++)
        {
            for (int y = 0; y < canvasSize; y++)
            {
                canvasTexture.SetPixel(x, y, Color.blue); 
            }
        }
        canvasTexture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
