using UnityEngine;
using UnityEngine.UI;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1; // The amount of score to add when collected
    [SerializeField] private Text scoreText; // The Text component that displays the score
    [SerializeField] private AudioClip collectSound; // The sound effect to play when the collectible is collected
    [SerializeField] private float volume = 1f; // The volume of the sound effect

    private AudioSource audioSource; // The AudioSource component that plays the sound effect

    private void Start()
    {
        GameManager.Instance.RegisterCollectible(); // Register this collectible with the GameManager
        UpdateScoreText(); // Update the score text when the game starts
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(scoreValue); // Add score to the player
            GameManager.Instance.Collect(); // Register that a collectible has been collected
            Destroy(gameObject); // Destroy this collectible
            UpdateScoreText(); // Update the score text after collecting
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound, volume); // Play the sound effect
            }
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + GameManager.Instance.score.ToString(); // Update the score text
        }
    }
}
