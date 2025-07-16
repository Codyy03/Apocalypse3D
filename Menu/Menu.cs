using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void ExitGame() => Application.Quit();

    public void LoadIntermediateScene() => SceneManager.LoadScene(1);
}
