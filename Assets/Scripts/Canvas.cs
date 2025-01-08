using TMPro;
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
    public int canvasSize = 512;
    public int brushSize = 5;
    public ColorPicker colorPicker;
    public Color paintColor;
    public PaintTool currentTool = PaintTool.Brush;
    public Slider brushSizeSlider;
    public GameObject colorBallPrefab;
    public GameObject paintParticlesPrefab;
    public Texture2D splashTemplate;
    public TextMeshProUGUI selectedToolText;
    
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

    void Update()
    {
        paintColor = colorPicker.GetCurrentColor();
        
        // Switching between tools
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentTool = PaintTool.Brush;
            selectedToolText.text = "Brush";
            Debug.Log("Selected tool: Brush");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentTool = PaintTool.Spray;
            selectedToolText.text = "Spray";
            Debug.Log("Selected tool: Spray");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentTool = PaintTool.Eraser;
            selectedToolText.text = "Eraser";
            Debug.Log("Selected tool: Eraser");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentTool = PaintTool.Ball;
            selectedToolText.text = "Ball";
            Debug.Log("Selected tool: Ball");
        }
        
        // Exploding ball
        if (currentTool == PaintTool.Ball && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 currentMousePosition = hit.textureCoord * canvasSize;
                ThrowBall(hit);
            }
        }
        
        // Left mouse button clicked -> paint
        if (Input.GetKey(KeyCode.Mouse0) && currentTool != PaintTool.Ball)
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
            case PaintTool.Ball:
                PaintBall(x, y);
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

    void PaintBall(int x, int y)
    {
        int splashSize = splashTemplate.width;
        int startX = Mathf.FloorToInt(x) - splashSize / 2;
        int startY = Mathf.FloorToInt(y) - splashSize / 2;

        for (int i = 0; i < splashSize; i++)
        {
            for (int j = 0; j < splashSize; j++)
            {
                Color splashPixel = splashTemplate.GetPixel(i, j);

                if (splashPixel == Color.clear)
                {
                    continue;
                }
                
                splashPixel = paintColor;

                int canvasX = startX + i;
                int canvasY = startY + j;

                if (canvasX >= 0 && canvasX < canvasSize && canvasY >= 0 && canvasY < canvasSize)
                {
                    canvasTexture.SetPixel(canvasX, canvasY, splashPixel);
                }
            }
        }

        
        // int splashRadius = 20;
        //
        // for (int i = -splashRadius; i <= splashRadius; i++)
        // {
        //     for (int j = -splashRadius; j <= splashRadius; j++)
        //     {
        //         float distance = Mathf.Sqrt(i * i + j * j);
        //         if (distance <= splashRadius)
        //         {
        //             int pixelX = x + i;
        //             int pixelY = y + j;
        //
        //             if (pixelX >= 0 && pixelX < canvasSize && pixelY >= 0 && pixelY < canvasSize)
        //             {
        //                 canvasTexture.SetPixel(pixelX, pixelY, paintColor);
        //             }
        //         }
        //     }
        // }
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
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ColorBall"))
        {
            // Where the ball hit
            Vector3 impactPosition = collision.contacts[0].point;
            
            GameObject particles = Instantiate(paintParticlesPrefab, impactPosition, Quaternion.identity);
            Debug.Log(impactPosition);
            float normX = 512 - ((impactPosition.x + 5) / 10 * 512);
            float normZ = 512 - ((impactPosition.z + 5) / 10 * 512);
            
            Paint(new Vector2(normX, normZ));
            
            // Effect
            Destroy(particles, 2f);
            
            // Destroy ball after collision
            Destroy(collision.gameObject);
        }
    }
    
    

}
