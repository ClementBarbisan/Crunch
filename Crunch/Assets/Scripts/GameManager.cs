using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [SerializeField] private Slider scoreSlider;
    [SerializeField] private TMP_Text debugScoreText;
    [SerializeField] private TMP_Text waveNumberText;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Animator animator;

    public float waveDuration = 20;
    public float waveGoalScore = 20;

    [SerializeField] private int currentWave;
    private float _timeBeforePhone;
    private float waveTimeElapsed;
    private float waveScore;
    private bool _isGameDone;
    private bool _hasWon;
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

        _phone = FindFirstObjectByType<InteractablePhoneBoss>(FindObjectsInactive.Include);
        _timeBeforePhone = Random.Range(10, 20);
        waveNumberText.text = "Wave " + (currentWave + 1);
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
        }
        else if (waveTimeElapsed >= waveDuration)
        {
            SetGameOver(false);
            
        }

        if (_timeBeforePhone < 0)
        {
           _phone.LaunchCoroutineRing();
            _timeBeforePhone = Random.Range(10, 20);
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
            
        }
        else
        {
            Debug.Log("YOU LOST (loser)");

            timeText.text = string.Format("{0:00}:{1:00}", 0, 0);
            
            animator.SetTrigger("defeatTrigger");

            timeSlider.value = 0f;
        }
    }

    public void SetContinue()
    {
        Debug.Log("Continue");
        currentWave++;
        waveNumberText.text = "Wave " + (currentWave + 1);
        waveScore = 0;
        waveTimeElapsed = 0;
        SceneManager.LoadSceneAsync(currentWave);
        animator.SetTrigger("continueTrigger");
        Time.timeScale = 1f;
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

        debugScoreText.text = "Score : "+waveScore.ToString("F2", CultureInfo.InvariantCulture); ;
    }
}
