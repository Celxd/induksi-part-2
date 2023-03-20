using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0; // The player's current score

    [SerializeField] private int numCollectibles = 0; // Total number of collectibles in the scene
    private int numCollected = 0; // Number of collectibles the player has collected

    [SerializeField] private TextMeshProUGUI scoreText; // Reference to the score text object

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreText(); // Update the score text when the game starts
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText(); // Update the score text when the score changes

        // Check if all collectibles have been collected
        if (numCollectibles > 0 && numCollected >= numCollectibles)
        {
            Debug.Log("All collectibles collected!");
        }
    }

    public void RegisterCollectible()
    {
        numCollectibles++;
    }

    public void Collect()
    {
        numCollected++;
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Mushroom Eaten: " + score;
    }
}
