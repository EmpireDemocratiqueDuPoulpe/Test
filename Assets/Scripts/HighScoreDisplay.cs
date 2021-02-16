using System;
using TMPro;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    private TMP_Text _scoreText;
    private TMP_Text _durationText;

    private void Start()
    {
        _scoreText = transform.Find("Score").GetComponent<TMP_Text>();
        _durationText = transform.Find("Duration").GetComponent<TMP_Text>();

        ShowHighScore();
    }
    
    /********************************************************************
     * High score display
     ********************************************************************/

    public void ShowHighScore()
    {
        if (!DoesHighScoreExist()) return;
        
        // Set text
        GetHighScore();
        
        // Display the game object
        _scoreText.enabled = true;
        _durationText.enabled = true;
    }
    
    public void HideHighScore()
    {
        // Hide the game object
        _scoreText.enabled = false;
        _durationText.enabled = false;
    }

    private bool DoesHighScoreExist()
    {
        return PlayerPrefs.HasKey("high_score_points");
    }

    private void GetHighScore()
    {
        // Show score
        var score = PlayerPrefs.GetInt("high_score_points");
        _scoreText.text = $"High score: {score} points";
        
        // Show duration
        if (PlayerPrefs.HasKey("high_score_duration"))
        {
            var duration = PlayerPrefs.GetFloat("high_score_duration");
            _durationText.text = $"in {(int) duration / 60}:{duration % 60:00.000}s";
        }
    }
}
