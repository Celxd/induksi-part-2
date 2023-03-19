using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0; // The player's current score

    [SerializeField] private int numCollectibles = 0; // Total number of collectibles in the scene
    private int numCollected = 0; // Number of collectibles the player has collected

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

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);

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
}
