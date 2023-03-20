using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;

    void Update()
    {
        // Pause using ESC
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseCanvas.activeSelf)
        {
            GameManager.Instance.Pause(pauseCanvas);
            UnlockCursor();
        }
        // Resume using ESC
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseCanvas.activeSelf)
        {
            GameManager.Instance.Resume(pauseCanvas);
            LockCursor();
        }
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
