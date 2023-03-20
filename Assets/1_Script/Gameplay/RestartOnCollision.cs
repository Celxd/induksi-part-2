using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnCollision : MonoBehaviour
{
    public string playerTag = "Player";
    public string waterTag = "Water";
    public string sceneName = ""; // leave blank to restart current scene

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag) && collision.collider.CompareTag(waterTag))
        {
            // Restart the game
            SceneManager.LoadScene(string.IsNullOrEmpty(sceneName) ? SceneManager.GetActiveScene().name : sceneName);
        }
    }
}