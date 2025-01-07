using UnityEngine;
using UnityEngine.UI;

public enum PaintTool
{
    Brush,
    Spray, 
    Eraser,
    Ball
}

public class Canvas : MonoBehaviour
{
    public int canvasSize = 512; // 10 units * 100 pixels
    public int brushSize = 5;
    public ColorPicker colorPicker;
    public Color paintColor;
    public PaintTool currentTool = PaintTool.Brush;
    public Slider brushSizeSlider;
    public GameObject colorBallPrefab;

    private Texture2D canvasTexture;
    private Vector2 lastMousePosition;
    private bool hasLastPosition = false;
    private bool canThrowBall = true;
    
    void Start()
    {
        // Canvas texture
        canvasTexture = new Texture2D(canvasSize, canvasSize);
        GetComponent<Renderer>().material.mainTexture = canvasTexture;
        
        ClearCanvas();
        
        brushSizeSlider.value = brushSize;
        brushSizeSlider.onValueChanged.AddListener(OnBrushSizeChanged);
    }

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
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentTool = PaintTool.Eraser;
            Debug.Log("Selected tool: Eraser");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentTool = PaintTool.Ball;
            Debug.Log("Selected tool: Ball");
        }
        
        if (currentTool == PaintTool.Ball && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (canThrowBall)
                {
                    ThrowBall(hit);
                    canThrowBall = false;
                }
            }
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
                    
                    if (!canThrowBall)  // Pokud nebyla koule vystřelena
                    {
                        canThrowBall = true; // Umožníme vystřelit kouli až při příštím kliknutí
                    }
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
            case PaintTool.Eraser:
                PaintEraser(x, y);
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
    
    void PaintEraser(int x, int y)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (x + i >= 0 && x + i < canvasSize && y + j >= 0 && y + j < canvasSize)
                {
                    canvasTexture.SetPixel(x + i, y + j, Color.white);
                }
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
    
    void ThrowBall(RaycastHit hit)
    {
        // Creating a ball
        GameObject ball = Instantiate(colorBallPrefab, Camera.main.transform.position, Quaternion.identity);
        Vector3 targetPosition = hit.point;
        Vector3 direction = (targetPosition - ball.transform.position).normalized;
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(direction * 2000f);
        }
    }

    public void ExplodeBall(Vector3 position)
    {
        Debug.Log("YUP");
        
        for (int i = 0; i < 100; i++)
        {
            float x = Random.Range(0f, canvasSize);
            float y = Random.Range(0f, canvasSize);
            // Paint(new Vector2(x, y)); 
            PaintBrush((int)x, (int)y);
        }
        
        canvasTexture.Apply();
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ColorBall"))
        {
            // Where the ball hit
            Vector3 impactPosition = collision.contacts[0].point;
            
            Vector2 canvasCoord = new Vector2(impactPosition.x, impactPosition.z); 
            Vector2 textureCoord = canvasCoord * canvasSize;
            
            // Effect
            ExplodeBall(textureCoord);
            
            // Destroy ball after collision
            Destroy(collision.gameObject);
        }
    }

}
