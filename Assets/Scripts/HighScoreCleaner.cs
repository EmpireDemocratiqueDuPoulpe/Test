using UnityEngine;

public class HighScoreCleaner : MonoBehaviour
{
    public HighScoreDisplay highScoreDisplay;

    public void DeleteHighScore()
    {
        PlayerPrefs.DeleteKey("high_score_points");
        PlayerPrefs.DeleteKey("high_score_duration");
        PlayerPrefs.Save();

        if (highScoreDisplay != null)
        {
            highScoreDisplay.HideHighScore();
        }
    }
}
