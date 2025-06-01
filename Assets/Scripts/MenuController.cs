using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel; 
    [SerializeField] private GameObject mainMenuPanel;
    
    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button exitButton;
    public Button settingButton;

    [Header("Settings Panel Buttons")]
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    [SerializeField] public AudioMixer audioMixer;
    public Button applyButton;

    public bool isFullscreen = true;
    
    [Header("Audio Settings")]
    [SerializeField] private string volumeParameter = "volume"; // Parameter name in AudioMixer
    [SerializeField] private float defaultVolume = 0.75f; // Default volume (75%)

    [Header("PlayerPrefs Keys")]
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string VOLUME_KEY = "MasterVolume";
    
    // Start is called before the first frame update
    void Start()
    {
        isFullscreen = true; // Mặc định là fullscreen
        startButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        settingPanel.SetActive(false); // Hide the settings panel at the start
        
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        applyButton.onClick.AddListener(OnApplyButtonClicked);
        
        // Đăng ký sự kiện thay đổi volume slider
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetAudioLevel);
        }
        
        // Tải cài đặt từ PlayerPrefs khi khởi động
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Tải cài đặt fullscreen
        isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
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
        
        Debug.Log($"Settings loaded: Fullscreen={isFullscreen}, Volume={savedVolume}");
    }

    private void SaveSetting()
    {
        // Lưu cài đặt fullscreen
        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
        
        // Lưu cài đặt âm lượng
        if (volumeSlider != null)
        {
            PlayerPrefs.SetFloat(VOLUME_KEY, volumeSlider.value);
        } 
        
        // Đảm bảo lưu ngay lập tức
        PlayerPrefs.Save();
        Debug.Log("Settings saved to PlayerPrefs");
    }
    
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Level 1");
        Debug.Log("Start button clicked!");
    }
    
    public void OnSettingButtonClicked()
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
        
        settingPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
    
    public void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked!");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    public void OnApplyButtonClicked()
    {
        // Lưu cài đặt trước khi đóng panel
        SaveSetting();
        
        settingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
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