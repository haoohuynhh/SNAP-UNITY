using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingMenu;
    public Button resumeButton;
    public Button settingButton;
    public Button exitButton;

    private PlayerInput playerInput;
    private InputAction pauseAction;
    public bool isPaused = false;

    void Awake()
    {

        playerInput = GetComponent<PlayerInput>();
        pauseAction = playerInput.actions["Pause"]; // Tên action trong InputActions

        pauseAction.performed += ctx => OnPauseButtonClicked();
    }

    void OnEnable()
    {
        pauseAction.Enable();
    }

    void OnDisable()
    {
        pauseAction.Disable();
    }

    void Start()
    {

        pauseMenu.SetActive(false);
        settingMenu.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        settingButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        settingMenu.SetActive(false);
        isPaused = false;
    }

    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Open Settings");
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Scene UI"); // Replace "MainMenu" with the name of your main menu scene
        Debug.Log("Exit Game");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        settingMenu.SetActive(false);
        
    }

    public void OnPauseButtonClicked()
    {
        if (pauseMenu.activeSelf)
            ResumeGame();
        else
            PauseGame();
    }
}
