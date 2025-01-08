using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public GameObject startMenu; 

    void Start()
    {
        startMenu.SetActive(true);  
    }

    public void StartGame()
    {
        startMenu.SetActive(false);  
    }
}