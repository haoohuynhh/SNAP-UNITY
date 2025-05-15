using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLevel : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Exit"))
        {
            Debug.Log("Level Completed");
            LoadNextLevel();

        }

        
    }
     public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        
        // Kiểm tra xem có phải scene cuối cùng không
        if (currentSceneIndex >= totalScenes - 1)
        {
            // Đã hoàn thành tất cả level, trở về màn hình chính (scene 0)
            Debug.Log("All levels completed! Returning to main menu.");
            SceneManager.LoadScene(0);
        }
        else
        {
            // Chưa phải scene cuối, tiếp tục chuyển đến scene tiếp theo
            SceneManager.LoadScene(currentSceneIndex + 1);
            Debug.Log("Loading next level: " + (currentSceneIndex + 1));
        }
    }
}
    

