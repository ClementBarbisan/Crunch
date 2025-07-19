using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [SerializeField] private Slider scoreSlider;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private float waveDuration = 20;
    [SerializeField] private float waveGoalScore = 20;
    private float waveTimeElapsed;
    private float waveScore;
    private bool _isGameDone;
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
    }

    private void OnGUI()
    {
        if (_isGameDone)
        {
            return;
        }
        UpdateUIElements();
    }

    public void ProduceMoney(float moneyRate)
    {
        waveScore += moneyRate * Time.deltaTime;
    }
    public void SetGameOver(bool hasWon)
    {
        _isGameDone = true;
        if (hasWon)
        {
            Debug.Log("YOU WON (ka-ching!)");
            scoreSlider.value = 1f;
        }
        else
        {
            Debug.Log("YOU LOST (loser)");

            int minutes = (int)waveDuration / 60;
            int seconds = (int)waveDuration % 60;
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            timeSlider.value = 1f;
        }
        UpdateUIElements();
    }


    private void UpdateUIElements()
    {
        int minutes = (int)waveTimeElapsed / 60;
        int seconds = (int)waveTimeElapsed % 60;
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        timeSlider.value = Mathf.Clamp01(waveTimeElapsed / waveDuration);

        scoreSlider.value = Mathf.Clamp01(waveScore / waveGoalScore);
    }
}
