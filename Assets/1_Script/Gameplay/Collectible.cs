using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Timeline;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1; // The amount of score to add when collected
    [SerializeField] private Text scoreText; // The Text component that displays the score
    [SerializeField] private AudioClip collectSound; // The sound effect to play when the collectible is collected
    [SerializeField] private float volume = 1f; // The volume of the sound effect

    public PlayableDirector director; // Reference to the PlayableDirector component
    public int trackIndex; // The index of the track you want to target
    public Camera mainCamera; // Reference to the main camera

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

            // Get the specified track from the Timeline and mute all other tracks
            var timelineAsset = (TimelineAsset)director.playableAsset;
            for (int i = 0; i < timelineAsset.outputTrackCount; i++)
            {
                var track = timelineAsset.GetOutputTrack(i);
                if (track != null)
                {
                    if (i == trackIndex)
                    {
                        // Unmute the target track
                        track.muted = false;
                        // Set the track's output to the main camera
                        var cam = Camera.main;
                        if (cam != null)
                        {
                            //track.SetGenericBinding(cam.gameObject, typeof(Camera));
                        }
                    }
                    else
                    {
                        // Mute all other tracks
                        track.muted = true;
                    }
                }
            }

            // Play the Timeline
            director.Play();

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
