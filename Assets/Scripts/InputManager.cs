using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingMenu;
    public Button resumeButton;
    public Button settingButton;
    public Button exitButton;
    public Button applyButton;

    // Phím dùng để tạm dừng game (mặc định là Escape)
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    
    public bool isPaused = false;

    [Header("Death Menu")]
    
    MenuController menuController;
    [SerializeField] GameObject deathMenu;

    // Không cần Awake, OnEnable, OnDisable nữa vì không sử dụng Input System

    void Start()
    {
        
        isPaused = false;

        // Đảm bảo menu không hiển thị khi bắt đầu
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
            
        if (settingMenu != null)
            settingMenu.SetActive(false);

        // Gán các listener cho các nút
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (settingButton != null)
            settingButton.onClick.AddListener(OpenSettings);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
            
        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySettings);
    }

    // Thêm phương thức Update để kiểm tra phím
    void Update()
    {
        // Kiểm tra nếu người chơi nhấn phím Escape (hoặc phím được cấu hình)
        if (Input.GetKeyDown(pauseKey))
        {
            OnPauseButtonClicked();
        }
    }

    public void ResumeGame()
    {
        // Kiểm tra null trước khi truy cập
        Time.timeScale = 1f;
        
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
            
        if (settingMenu != null)
            settingMenu.SetActive(false);
            
        isPaused = false;
    }

    public void OpenSettings()
    {
        
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
            
        if (settingMenu != null)
            settingMenu.SetActive(true);
            
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Open Settings");
    }

    public void ExitGame()
    {
        ResetGame();
        SceneManager.LoadScene("Scene UI");
        Debug.Log("Exit Game");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
            
        if (settingMenu != null)
            settingMenu.SetActive(false);
            
        isPaused = true;
    }

    public void OnPauseButtonClicked()
    {
        // Kiểm tra null trước khi truy cập
        if (pauseMenu == null)
        {
            Debug.LogWarning("Pause menu is null!");
            return;
        }

        if (pauseMenu.activeSelf)
            ResumeGame();
        else
            PauseGame();
    }

    public void ApplySettings()
    {
        if (settingMenu != null)
            settingMenu.SetActive(false);
            
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
            
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Reset Game");
    }

    public void ShowDeathMenu()
    {
        if (deathMenu != null )
            deathMenu.SetActive(true);
            
        
        isPaused = true;
    }

    public void OnFullscreenToggleChanged()
    {
        
    }



}