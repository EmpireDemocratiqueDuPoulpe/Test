using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game settings")]
    public int gameDuration = 180;
    public int defaultScore = 10_000;
    public int scoreLoosePerSeconds = 55;
    public bool canHighScoreOnLose = false;
    public bool enableCheat = false;
    
    private bool _isGameFinished = false;
    private bool _hasPlayerWon = false;
    
    [Header("Game loop")]
    public float endingDuration = 3f;
    
    private WaitForSeconds _endingWait;
    
    [Header("Info display")]
    public TMPro.TMP_Text scoreText;
    public TMPro.TMP_Text remainingTimeText;
    public GameObject winScreen;
    public TMPro.TMP_Text winScoreText;
    public GameObject loseScreen;
    public TMPro.TMP_Text loseScoreText;
    public GameObject newHighScoreText;
    
    private float _score;
    private NumberFormatInfo _scoreFormat = new NumberFormatInfo {CurrencyGroupSeparator = " ", CurrencySymbol = string.Empty, CurrencyDecimalDigits = 0};
    private float _remainingTime;

    [Header("Sound settings")]
    public bool playSounds = true;
    public AudioClip winSound;
    public AudioClip loseSound;

    private AudioSource _audioSource;

    private void Start()
    {
        _endingWait = new WaitForSeconds(endingDuration);

        _audioSource = GetComponent<AudioSource>();
        
        StartGame();
    }

    private void Update()
    {
        if (_isGameFinished) return;
        
        // Update score and remaining time
        _score -= Time.deltaTime * scoreLoosePerSeconds;
        _remainingTime -= Time.deltaTime;
        
        // Update display
        UpdateTimerDisplay();
        UpdateScoreDisplay();
    }

    /********************************************************************
     * Game cycles
     ********************************************************************/

    public void StartGame()
    {
        ShowWinScreen(false);
        ShowLoseScreen(false);
        ShowNewHighScoreText(false);
        SetScore(defaultScore);
        SetRemainingTime(gameDuration);
        
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        // Start game's states
        yield return StartCoroutine(GamePlaying());
        yield return StartCoroutine(GameEnding());
        
        // The game is finished
        if (!_isGameFinished) yield break;

        var isAHighScore = CompareScoreToHighScore();
        
        if (_hasPlayerWon)
        {
            PlaySound(winSound);
            ShowWinScreen();
            ShowNewHighScoreText(isAHighScore);
        }
        else
        {
            PlaySound(loseSound);
            ShowLoseScreen();

            if (canHighScoreOnLose)
            {
                ShowNewHighScoreText(isAHighScore);
            }
        }
    }
    
    private IEnumerator GamePlaying()
    {
        while (!IsTimeElapsed() && !_isGameFinished && !_hasPlayerWon)
        {
            ProcessCheat();
            yield return null;
        }
    }

    private IEnumerator GameEnding()
    {
        // The game is finished
        _isGameFinished = true;

        yield return _endingWait;
    }

    public void SetGameWon(bool won = true)
    {
        _hasPlayerWon = won;
    }

    public bool IsGameFinished => _isGameFinished;

    /********************************************************************
     * Score
     ********************************************************************/
    
    public void AddScore(int scoreToAdd) { _score += scoreToAdd; }
    public void RemoveScore(int scoreToRemove) { _score -= scoreToRemove; }
    
    public void SetScore(int score)
    {
        _score = score;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = GetFormattedScore();
    }

    public string GetFormattedScore()
    {
        return _score.ToString("C", _scoreFormat);
    }

    private bool CompareScoreToHighScore()
    {
        // Check if there's already a high score
        if (PlayerPrefs.HasKey("high_score_points"))
        {
            var points = PlayerPrefs.GetInt("high_score_points");

            if (points < _score)
            {
                AddScoreToHighScore();
            }
            else return false;
        }
        else
        {
            AddScoreToHighScore();
        }

        return true;
    }
    
    private void AddScoreToHighScore()
    {
        PlayerPrefs.SetInt("high_score_points", (int) _score);
        PlayerPrefs.SetFloat("high_score_duration", gameDuration - _remainingTime);
        PlayerPrefs.Save();
    }

    /********************************************************************
     * Timer
     ********************************************************************/

    public void SetRemainingTime(int remainingTime)
    {
        _remainingTime = remainingTime;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (IsTimeElapsed())
        {
            remainingTimeText.text = "00:00.000";
            return;
        }
        
        remainingTimeText.text = $"{(int) _remainingTime / 60}:{_remainingTime % 60:00.000}";
    }
    
    public bool IsTimeElapsed()
    {
        return (_remainingTime <= 0f);
    }
    
    /********************************************************************
     * Screen display
     ********************************************************************/

    public void ShowWinScreen(bool show = true)
    {
        if (show)
        {
            winScoreText.text = $"You made {GetFormattedScore()} points.";
        }
        
        winScreen.SetActive(show);
    }
    
    public void ShowLoseScreen(bool show = true)
    {
        if (show)
        {
            loseScoreText.text = $"You made {GetFormattedScore()} points.";
        }
        
        loseScreen.SetActive(show);
    }
    
    public void ShowNewHighScoreText(bool show = true)
    {
        newHighScoreText.SetActive(show);
    }
    
    /********************************************************************
     * Game states sounds
     ********************************************************************/
    
    private void PlaySound(AudioClip clip)
    {
        if (!CanPlaySound(clip)) return;
        
        if (_audioSource.isPlaying) _audioSource.Stop();
        _audioSource.PlayOneShot(clip);
    }

    public bool CanPlaySound()
    {
        return playSounds;
    }
    
    public bool CanPlaySound(AudioClip clip)
    {
        return playSounds && (clip != null);
    }
    
    /********************************************************************
     * Cheat
     ********************************************************************/

    private void ProcessCheat()
    {
        if (!enableCheat) return;
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            _isGameFinished = true;
            SetGameWon(false);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _isGameFinished = true;
            SetGameWon(true);
        }
    }
}
