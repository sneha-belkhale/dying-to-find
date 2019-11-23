using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

//Static Class to hand grab state
public class CCSceneUtils : MonoBehaviour {
    public static CCSceneUtils instance;
    public Transform instantiatedObjectRoot;
    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(instance);
            instance = this;
        }
        #if !UNITY_EDITOR
            SceneManager.LoadScene("CrowdScene", LoadSceneMode.Additive);
        #endif
    }
    private void OnDestroy()
    {
        instance = null;
    }
    public static IEnumerator DoFadeSceneLoadCoroutine (string sceneName, string currentSceneName) {
        foreach (Transform child in instance.instantiatedObjectRoot) {
            GameObject.Destroy(child.gameObject);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        yield return null;
        SceneManager.UnloadSceneAsync(currentSceneName);
    }
}