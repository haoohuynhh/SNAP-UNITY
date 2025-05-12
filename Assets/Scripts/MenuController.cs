using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel; 
    [SerializeField] private GameObject mainMenuPanel;

    
    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button exitButton;
    public Button settingButton;


    [Header("Settings Panel Buttons")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public Button applyButton;

    


   
    // Start is called before the first frame update
    void Start()
    {


        startButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        settingPanel.SetActive(false); // Hide the settings panel at the start
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        applyButton.onClick.AddListener(OnApplyButtonClicked);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Level 0"); // Replace "GameScene" with the name of your game scene
        Debug.Log("Start button clicked!");
        
        }
    public void OnSettingButtonClicked()
        {
            settingPanel.SetActive(true);
            mainMenuPanel.SetActive(false);


            
        }
    public void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked!");
        Application.Quit();

    }
    public void OnApplyButtonClicked()
    {
        settingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    
}
