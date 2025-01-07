using UnityEngine;
using UnityEngine.UI;

public enum PaintTool
{
    Brush,
    Spray
}

public class Canvas : MonoBehaviour
{
    public int canvasSize = 512; // 10 units * 100 pixels
    public int brushSize = 5;
    public ColorPicker colorPicker;
    public Color paintColor;
    public PaintTool currentTool = PaintTool.Brush;
    public Slider brushSizeSlider;

    private Texture2D canvasTexture;
    private Vector2 lastMousePosition;
    private bool hasLastPosition = false;
    
    void Start()
    {
        // Canvas texture
        canvasTexture = new Texture2D(canvasSize, canvasSize);
        GetComponent<Renderer>().material.mainTexture = canvasTexture;
        
        ClearCanvas();
        
        brushSizeSlider.value = brushSize;
        brushSizeSlider.onValueChanged.AddListener(OnBrushSizeChanged);
    }

    // Update is called once per frame
    void Update()
    {
        paintColor = colorPicker.GetCurrentColor();
        
        // Switching between tools
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentTool = PaintTool.Brush;
            Debug.Log("Selected tool: Brush");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentTool = PaintTool.Spray;
            Debug.Log("Selected tool: Spray");
        }
        
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
    
    void OnBrushSizeChanged(float newSize)
    {
        brushSize = Mathf.RoundToInt(newSize);
        Debug.Log("Brush size: " + brushSize);
    }

    void Paint(Vector2 textureCoord)
    {
        // Coords in texture space
        int x = Mathf.FloorToInt(textureCoord.x);
        int y = Mathf.FloorToInt(textureCoord.y);

        switch (currentTool)
        {
            case PaintTool.Brush:
                PaintBrush(x, y);
                break;
            case PaintTool.Spray:
                PaintSpray(x, y);
                break;
        }
        
        canvasTexture.Apply();
    }

    void PaintBrush(int x, int y)
    {
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
    }
    
    void PaintSpray(int x, int y)
    {
        int sprayDensity = 50; 
        float sprayRadius = brushSize;

        for (int i = 0; i < sprayDensity; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2); 
            float radius = Random.Range(0f, sprayRadius);

            int sprayX = Mathf.FloorToInt(x + Mathf.Cos(angle) * radius);
            int sprayY = Mathf.FloorToInt(y + Mathf.Sin(angle) * radius);

            if (sprayX >= 0 && sprayX < canvasSize && sprayY >= 0 && sprayY < canvasSize)
            {
                canvasTexture.SetPixel(sprayX, sprayY, paintColor);
            }
        }
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
