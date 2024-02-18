using UnityEngine;

public class PlayerScoreController : MonoBehaviour
{
    PlayerHealthController m_playerHealth;
    PlayerMoveController m_playerMove;

    [SerializeField] private TMPro.TMP_Text m_beatSyncAnalyzerText;
    [SerializeField] private UnityEngine.UI.Slider m_accuracySlider;

    private float m_lastBeatTime;
    private float m_nextBeatTime;
    private int m_comboMeter = 0;
    private int m_scoreMultiplier = 1;

    private void Awake()
    {
        m_playerHealth = GetComponent<PlayerHealthController>();
        m_playerMove = GetComponent<PlayerMoveController>();
    }

    private void OnEnable()
    {
        BeatManager.SubscribeToBeatEvent(UpdateLastBeatAndGetNext);
    }

    private void OnDisable()
    {
        BeatManager.UnsubscribeFromBeatEvent(UpdateLastBeatAndGetNext);
    }

    bool movingRight;
    int counter = 1;
    private void UpdateLastBeatAndGetNext()
    {
        if (counter >= 1)
        {
            m_lastBeatTime = BeatManager.I.GetSourceTime();
            float beatValue = BeatManager.I.GetIntervalLength(BeatType.HalfBeat);
            m_nextBeatTime = m_lastBeatTime + beatValue;
            //AudioManager.I.PlaySound("Click");

            m_accuracySlider.minValue = m_nextBeatTime - beatValue;
            m_accuracySlider.maxValue = m_nextBeatTime + beatValue;

            if (movingRight)
            {
                m_accuracySlider.SetDirection(UnityEngine.UI.Slider.Direction.LeftToRight, false);
            }
            else
            {
                m_accuracySlider.SetDirection(UnityEngine.UI.Slider.Direction.RightToLeft, false);
            }

            movingRight = !movingRight;
            counter = 0;
        }
        else
            counter++;
    }
    private void Update()
    {
        if (!m_playerMove.GetIsMoving())
        {
            m_accuracySlider.value = BeatManager.I.GetSourceTime();
        }
    }

    float threshold; // Adjust the threshold as needed
    public void CheckTiming(float _time)
    {
        threshold = BeatManager.I.GetIntervalLength(BeatType.Beat); // Adjust the threshold as needed

        if (Mathf.Abs(m_nextBeatTime - _time) <= threshold / 4)
        {
            Perfect();
        }
        else if (Mathf.Abs(m_nextBeatTime - _time) <= threshold/2)
        {
            Good();
        }
        else
        {
            if (m_comboMeter > 0)
            {
                Poor();
            }
            else
            {
                Failed();
            }
        }
    }

    private void Perfect()
    {
        m_comboMeter += 1;

        if(m_comboMeter % 4 == 0)
        {
            SongManager.I.AddMoreMusicLayer();
            m_scoreMultiplier += 1;
        }

        m_playerHealth.AddHealth(2);
        ScoreManager.I.AddScore(5 * m_scoreMultiplier);
        UpdatePlayerText("Perfect!!", Color.white);
    }

    private void Good()
    {
        m_playerHealth.AddHealth(1);
        ScoreManager.I.AddScore(2 * m_scoreMultiplier);
        UpdatePlayerText("Good", Color.yellow);
    }

    private void Poor()
    {
        m_comboMeter = 0;
        m_scoreMultiplier = 1;

        m_playerHealth.TakeDamage(2);
        ScoreManager.I.AddScore(1 * m_scoreMultiplier);
        UpdatePlayerText("Poor",Color.magenta);
    }

    private void Failed()
    {
        m_comboMeter = 0;
        m_scoreMultiplier = 1;

        SongManager.I.RemoveMusicLayer();

        m_playerHealth.TakeDamage(10);
        ScoreManager.I.AddScore(0 * m_scoreMultiplier);
        UpdatePlayerText("Failed!",Color.red);
    }

    private void UpdatePlayerText(string _message, Color _color)
    {
        m_beatSyncAnalyzerText.text = _message;
        m_beatSyncAnalyzerText.color = _color;
        m_beatSyncAnalyzerText.gameObject.SetActive(true);
    }
}
