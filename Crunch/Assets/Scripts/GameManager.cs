using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [SerializeField] private Slider scoreSlider;
    [SerializeField] private TMP_Text debugScoreText;
    [SerializeField] private TMP_Text waveNumberText;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text timeText, scoreText, newsText;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 _minMaxCooldownPhone = new Vector2(10, 20);
    [SerializeField] private GameObject tutorial;
    public float waveDuration = 20;
    public float waveGoalScore = 20;

    [SerializeField] private int currentWave;
    private float _timeBeforePhone;
    private float waveTimeElapsed;
    private float waveScore;
    private bool _isGameDone;
    private bool _hasWon;

    private int _nbScreams, _nbBreaks, _nbTrauma, _nbCat;

    private InteractablePhoneBoss _phone;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("singleton GameManager is already instantiated");
            Destroy(gameObject);
        }
        
        _phone = FindFirstObjectByType<InteractablePhoneBoss>(FindObjectsInactive.Exclude);
        _timeBeforePhone = UnityEngine.Random.Range(_minMaxCooldownPhone.x, _minMaxCooldownPhone.y);
        waveNumberText.text = "Wave " + (currentWave + 1);

        if (currentWave == 0)
        {
            SetTimeScale(0f);
            tutorial.SetActive(true);
        }
    }

    private void Update()
    {
        if (_isGameDone)
        {
            return;
        }

        waveTimeElapsed += Time.deltaTime;

        if (waveScore >= waveGoalScore)
        {
            SetGameOver(true);
            MusicManager.Instance.ChangeMusic(1);
        }
        else if (waveTimeElapsed >= waveDuration)
        {
            SetGameOver(false);
            MusicManager.Instance.ChangeMusic(2);
        }

        if (_timeBeforePhone < 0 && _phone != null)
        {
            _phone.LaunchCoroutineRing();
           _timeBeforePhone = UnityEngine.Random.Range(_minMaxCooldownPhone.x, _minMaxCooldownPhone.y);
        }
        _timeBeforePhone -= Time.deltaTime;
    }

    private void OnGUI()
    {
        UpdateUIElements();
    }

    public void ProduceMoney(float moneyRate)
    {
        waveScore += moneyRate * Time.deltaTime;
    }
    public void SetGameOver(bool hasWon)
    {
        _isGameDone = true;
        _hasWon = hasWon;
        Time.timeScale  = 0f;
        
        if (hasWon)
        {
            Debug.Log("YOU WON (ka-ching!)");
            animator.SetTrigger("victoryTrigger");
            scoreSlider.value = 1f;
            ScoreBoard();

        }
        else
        {
            Debug.Log("YOU LOST (loser)");
            timeText.text = string.Format("{0:00}:{1:00}", 0, 0);
            animator.SetTrigger("defeatTrigger");
            timeSlider.value = 0f;
        }
    }

    private void ScoreBoard()
    {
        float valueScore = waveScore * 10f - waveTimeElapsed;
        scoreText.text = "Stats:"+ Environment.NewLine+ "Screams " + _nbScreams + Environment.NewLine + "Breaks " + _nbBreaks + Environment.NewLine + "Traumatized " + _nbTrauma + Environment.NewLine + "Profitability " + valueScore.ToString("0");
        newsText.text = "Lots of new things!!!!!";
    }

    public void RestartWave()
    {
        SceneManager.LoadSceneAsync(currentWave);
        int v = currentWave + 3;
        MusicManager.Instance.ChangeMusic(v);
    }
    
    public void SetContinue()
    {
        Debug.Log("Continue");
        currentWave++;
        waveNumberText.text = "Wave " + (currentWave + 1);
        waveScore = 0;
        waveTimeElapsed = 0;
        animator.SetTrigger("continueTrigger");
        Time.timeScale = 1f;
        _nbBreaks = 0;
        _nbScreams = 0;
        _nbTrauma = 0;
        int v = currentWave + 4;
        MusicManager.Instance.ChangeMusic(v);
        SceneManager.LoadSceneAsync(currentWave+1);
    }


    private void UpdateUIElements()
    {
        int minutes = (int)(waveDuration-waveTimeElapsed) / 60;
        int seconds = (int)(waveDuration-waveTimeElapsed) % 60;
        
        minutes = Mathf.Max(minutes, 0);
        seconds = Mathf.Max(seconds, 0);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        timeSlider.value = 1-Mathf.Clamp01(waveTimeElapsed / waveDuration);

        scoreSlider.value = Mathf.Clamp01(waveScore / waveGoalScore);

        debugScoreText.text = "Score : " + waveScore.ToString("F2", CultureInfo.InvariantCulture); ;
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }

    public void StatsScreams()
    {
        _nbScreams++;
    }

    public void StatsBreaks()
    {
        _nbBreaks++;
    }

    public void StatsTrauma()
    {
        _nbTrauma ++;
    }

    public void StatsCat()
    {
        _nbCat++;
    }
}
