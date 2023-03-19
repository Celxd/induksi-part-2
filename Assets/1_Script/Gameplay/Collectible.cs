using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1; // The amount of score to add when collected

    private void Start()
    {
        GameManager.Instance.RegisterCollectible(); // Register this collectible with the GameManager
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(scoreValue); // Add score to the player
            GameManager.Instance.Collect(); // Register that a collectible has been collected
            Destroy(gameObject); // Destroy this collectible
        }
    }
}
