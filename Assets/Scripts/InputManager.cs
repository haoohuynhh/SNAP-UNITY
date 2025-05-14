using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingMenu;
    public Button resumeButton;
    public Button settingButton;
    public Button exitButton;
    public Button applyButton;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    
    public bool isPaused = false;
    
    [Header("Death Menu")]
    [SerializeField] GameObject deathMenu;
    
    [Header("Settings Panel")]
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public bool isFullscreen = true;
    [SerializeField] public AudioMixer audioMixer;
    
    [Header("Audio Settings")]
    [SerializeField] private string volumeParameter = "volume"; // Parameter name in AudioMixer
    [SerializeField] private float defaultVolume = 0.75f; // Default volume (75%)
    
    [Header("PlayerPrefs Keys")]
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string VOLUME_KEY = "MasterVolume";
    
    void Start()
    {
        isFullscreen = true; // Mặc định là fullscreen
        // Thiết lập các listeners cho buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (settingButton != null)
            settingButton.onClick.AddListener(OpenSettings);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitToMainMenu);
            
        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySettings);
        
        // Đăng ký sự kiện thay đổi volume slider
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetAudioLevel);
        }
        
        // Tải cài đặt từ PlayerPrefs khi khởi động
        LoadSettings();
    }
    
    void Update()
    {
        // Xử lý phím Escape
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
        
        Time.timeScale = 0f;
        isPaused = true;
    }
    
    public void ResumeGame()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
            
        if (settingMenu != null)
            settingMenu.SetActive(false);
            
        Time.timeScale = 1f;
        isPaused = false;
    }
    
    public void OpenSettings()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
            
        if (settingMenu != null)
        {
            // Tải lại cài đặt hiện tại vào UI trước khi hiển thị panel
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
            }
            
            if (volumeSlider != null)
            {
                float currentVolume = 0;
                if (audioMixer.GetFloat(volumeParameter, out currentVolume))
                {
                    // Chuyển đổi từ decibel sang giá trị tuyến tính (0-1)
                    float linearVolume = Mathf.Pow(10, currentVolume / 20);
                    volumeSlider.value = linearVolume;
                }
                else
                {
                    // Nếu không lấy được giá trị từ mixer, sử dụng giá trị đã lưu
                    volumeSlider.value = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
                }
            }
            
            settingMenu.SetActive(true);
        }
            
        isPaused = true;
        Time.timeScale = 0f;
    }
    
    public void ApplySettings()
    {
        // Lưu cài đặt
        SaveSettings();
        
        // Đóng menu settings và mở lại menu pause
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
    
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene UI");
    }
    
    public void ShowDeathMenu()
    {
        if (deathMenu != null)
            deathMenu.SetActive(true);
            
        isPaused = true;
    }
    
    private void LoadSettings()
    {
        // Tải cài đặt fullscreen
        bool isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = isFullscreen;
        }
        
        // Tải cài đặt âm lượng
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
        
        // Áp dụng cài đặt âm lượng vào AudioMixer
        SetAudioLevel(savedVolume);        
        // Áp dụng cài đặt fullscreen
        Screen.fullScreen = isFullscreen;
        
        Debug.Log($"Settings loaded in InputManager: Fullscreen={isFullscreen}, Volume={savedVolume}");
    }
    
    private void SaveSettings()
    {
        // Lưu cài đặt fullscreen
        if (fullscreenToggle != null)
        {
            PlayerPrefs.SetInt(FULLSCREEN_KEY, fullscreenToggle.isOn ? 1 : 0);
        }
        
        // Lưu cài đặt âm lượng
        if (volumeSlider != null)
        {
            PlayerPrefs.SetFloat(VOLUME_KEY, volumeSlider.value);
        }
        
        // Đảm bảo lưu ngay lập tức
        PlayerPrefs.Save();
        Debug.Log("Settings saved from InputManager");
    }
    
    public void OnFullscreenToggleChanged()
    {
        isFullscreen = fullscreenToggle.isOn;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, isFullscreen);
        Debug.Log("Fullscreen changed to: " + isFullscreen);
    }
    
    // Phương thức được gọi khi thay đổi giá trị slider
    public void SetAudioLevel(float volumeLevel)
    {
       
            // Chuyển đổi từ tuyến tính sang decibel
            audioMixer.SetFloat(volumeParameter, Mathf.Log10(volumeLevel) * 20);
        
        
        Debug.Log($"Volume set to: {volumeLevel} ({Mathf.Log10(Mathf.Max(0.0001f, volumeLevel)) * 20}dB)");
    }
}
    
    // Phương thức để đặt âm lượng mà không cần thay đổi slider
   