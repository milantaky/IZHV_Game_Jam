using UnityEngine;

public class Canvas : MonoBehaviour
{
    public int canvasSize = 1000; // 10 units * 100 pixels
    public int brushSize = 10;
    public Color paintColor = Color.black;

    private Texture2D canvasTexture;
    
    // Start is called before the first frame update
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
        // Left mouse button clicked -> paint
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // Returns true if hits the canvas
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("hit" + hit.textureCoord);
                Paint(hit.textureCoord);
            }
            else
            {
                Debug.Log("not hit");
            }
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
        int x = (int)(textureCoord.x * canvasSize);
        int y = (int)(textureCoord.y * canvasSize);
        
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
