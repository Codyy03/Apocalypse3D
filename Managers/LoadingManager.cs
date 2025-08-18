using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    [SerializeField] TextMeshProUGUI progressText;
    

    void Start() => StartCoroutine(LoadSceneAsync());

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(2);
        while (!operation.isDone)
        {
            if (progressBar)
                progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressText)
                progressText.text = Mathf.RoundToInt(operation.progress * 100f) + "%";
            yield return null;
        }
    }
}