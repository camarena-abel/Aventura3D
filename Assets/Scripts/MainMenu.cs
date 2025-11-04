using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Continue()
    {
        if (GameState.Load())
        {
            SceneManager.LoadScene("SampleScene");
        }        
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
