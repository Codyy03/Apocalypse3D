using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static bool menuLoadedAtStart = false;

    public static SceneController Instance { get; private set; }

    [SerializeField] List<GameObject> multiSceneObjects;

    private void Start()
    {
        if (!menuLoadedAtStart)
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
            menuLoadedAtStart = true;
        }

        foreach (GameObject go in multiSceneObjects)
        {
            if (go !=null)
                DontDestroyOnLoad(go);
        }
    }

}
