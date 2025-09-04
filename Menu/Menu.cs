using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static bool loadGame;
    private void Start()
    {
        Dialogues.DialoguesManager.dialogueIsActive = false;
        InteractionController.lootIsOpen = false;
        GameManager.UIElementIsOpen = false;
        loadGame = false;

        Cursor.lockState = CursorLockMode.None;
    }
    public void ExitGame() => Application.Quit();

    public void LoadIntermediateScene()
    {
        SceneManager.LoadScene(2);
        SceneLoadData.sceneToLoadIndex = 3;
    }

    public void LoadGame() => loadGame = true;
}
