using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
        Debug.Log("Back to Main Menu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        MusicManager.Instance.ChangeMusic(3);
        Debug.Log("Launching main Scene");
    }
}
