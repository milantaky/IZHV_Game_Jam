using UnityEngine;

public class Canvas : MonoBehaviour
{
    public int canvasSize = 512; // 10 units * 100 pixels
    public int brushSize = 5;
    public ColorPicker colorPicker;
    public Color paintColor;

    private Texture2D canvasTexture;
    private Vector2 lastMousePosition;
    private bool hasLastPosition = false;
    
    void Start()
    {
        // Canvas texture
        canvasTexture = new Texture2D(canvasSize, canvasSize);
        GetComponent<Renderer>().material.mainTexture = canvasTexture;
        
        ClearCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        paintColor = colorPicker.GetCurrentColor();
        
        // Left mouse button clicked -> paint
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // Returns true if hits the canvas
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 currentMousePosition = hit.textureCoord * canvasSize;
                
                if (hasLastPosition)
                {
                    // Interpolation so there are not spaces between faster draws
                    float distance = Vector2.Distance(lastMousePosition, currentMousePosition);
                    
                    // Only interpolate if the distance is significant
                    if (distance > 1f) 
                    {
                        int steps = Mathf.CeilToInt(distance / 10);

                        for (int i = 0; i <= steps; i++)
                        {
                            Vector2 interpolatedPosition = Vector2.Lerp(lastMousePosition, currentMousePosition, i / (float)steps);
                        
                            // Check for NaN values
                            if (!float.IsNaN(interpolatedPosition.x) && !float.IsNaN(interpolatedPosition.y))
                            {
                                Paint(interpolatedPosition);
                            }
                        }
                    }
                }
                else
                {
                    Paint(currentMousePosition);
                    hasLastPosition = true;
                }

                lastMousePosition = currentMousePosition;
            }
        }
        else
        {
            // Reset after unpressing button
            hasLastPosition = false;
        }
        
        // "r" clicked -> reset canvas
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearCanvas();
            Debug.Log("Canvas cleared");
        }   
    }

    void Paint(Vector2 textureCoord)
    {
        // Coords in texture space
        int x = Mathf.FloorToInt(textureCoord.x);
        int y = Mathf.FloorToInt(textureCoord.y);

        // Debug.Log("Coords: " + y + " " + x);
        
        canvasTexture.SetPixel(x, y, paintColor);

        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (x + i >= 0 && x + i < canvasSize && y + j >= 0 && y + j < canvasSize)
                {
                    canvasTexture.SetPixel(x + i, y + j, paintColor);
                }
            }
        }
        
        canvasTexture.Apply();
    }

    void ClearCanvas()
    {
        for (int x = 0; x < canvasSize; x++)
        {
            for (int y = 0; y < canvasSize; y++)
            {
                canvasTexture.SetPixel(x, y, Color.white); 
            }
        }
        canvasTexture.Apply();
    }
}
