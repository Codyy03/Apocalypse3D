using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar; 

    void Start() => StartCoroutine(LoadSceneAsync());

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(2);
        while (!operation.isDone)
        {
            if (progressBar)
                progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }
}