using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Start()
    {
        Dialogues.DialoguesManager.dialogueIsActive = false;
        InteractionController.lootIsOpen = false;
        GameManager.UIElementIsOpen = false;
    }
    public static bool loadGame;
    public void ExitGame() => Application.Quit();

    public void LoadIntermediateScene() => SceneManager.LoadScene(1);

    public void LoadGame() => loadGame = true;
}
