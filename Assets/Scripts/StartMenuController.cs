using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public GameObject startMenu;
    public static bool isGameRunning = false; // Stav hry (false na začátku)

    void Start()
    {
        startMenu.SetActive(true);  
        isGameRunning = false;      // Hra je zablokovaná na začátku
    }

    public void StartGame()
    {
        startMenu.SetActive(false);  
        isGameRunning = true;       // Hra se odemkne po kliknutí na Start
    }
}