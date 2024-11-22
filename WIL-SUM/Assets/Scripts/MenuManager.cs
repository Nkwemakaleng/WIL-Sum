using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    //public GameObject mainMenuPanel;  // Reference to the main menu panel
    public GameObject pauseMenuPanel; // Reference to the pause menu panel
    public GameObject optionsMenuPanel; // Reference to the options menu panel

    private bool isPaused = false;
    public Slider volumeSlider;  // Volume slider
    public Button muteButton;    // Mute button
    private bool isMuted = false;
    private float previousVolume = 1f; // Store previous volume when muted


    void Start()
    {
        // Ensure that only the main menu is active on start if on the main menu scene
       // if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Initialize mute button
        if (muteButton != null)
        {
            muteButton.onClick.AddListener(ToggleMute);
            UpdateMuteButtonText();
        }

        // Unlock the cursor on the main menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Toggle pause menu when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // Load the main game scene
    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Quit the application
    public void QuitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }

    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze the game
        pauseMenuPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Resume the game
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Unfreeze the game
        pauseMenuPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Open the options menu
    public void OpenOptionsMenu()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(true);
    }

    // Return to the pause menu from the options menu
    public void ReturnToPauseMenu()
    {
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    // Return to the main menu
    public void ReturnToMainMenu(string mainMenuSceneName)
    {
        Time.timeScale = 1f; // Ensure time scale is reset
        SceneManager.LoadScene(mainMenuSceneName);
    }
    // Method to restart the level
    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene(); // Get the current scene
        SceneManager.LoadScene(currentScene.name); // Reload the scene
    }
    // Set the audio volume
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        isMuted = volume == 0;
        UpdateMuteButtonText();
    }

    // Toggle mute on or off
    public void ToggleMute()
    {
        if (isMuted)
        {
            // Unmute by restoring the previous volume
            AudioListener.volume = previousVolume;
            isMuted = false;
        }
        else
        {
            // Mute by setting volume to 0 and saving the current volume
            previousVolume = AudioListener.volume;
            AudioListener.volume = 0;
            isMuted = true;
        }

        // Update the button's text to reflect the mute state
        UpdateMuteButtonText();

        if (volumeSlider != null)
            volumeSlider.value = AudioListener.volume;
    }

    // Update mute button text to indicate the current state
    private void UpdateMuteButtonText()
    {
        if (muteButton != null)
        {
            Text buttonText = muteButton.GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = isMuted ? "Unmute SFX" : "Mute SFX";
        }
    }
    public void ResetUI()
    {
        /* Reset Main Menu UI
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);*/

        // Reset Pause Menu UI
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Reset Options Menu UI
        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(false);

        // Reset Volume Slider
        if (volumeSlider != null)
            volumeSlider.value = 1.0f; // Default volume value

        // Reset Mute Button
        if (muteButton != null)
        {
            isMuted = false;
            UpdateMuteButtonText();
        }

        // Ensure cursor is visible on main menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
