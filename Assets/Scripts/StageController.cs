using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    private static bool isLoading;
    public void ClearStage()
    {
        if (isLoading) return;
        isLoading = true;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);

        isLoading = false;
    }

    public void RestartStage()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
    }
}
