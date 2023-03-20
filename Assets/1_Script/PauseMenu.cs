using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public void resume()
    {
        Time.timeScale = 1f; // Set the time scale back to normal
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Make the cursor invisible
        gameObject.SetActive(false); // Deactivate the pause menu canvas
    }

    public void OnBackToMainMenuButtonClick()
    {
        SceneManager.LoadScene("Menu");
    }
}
