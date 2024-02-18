using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private Leaderboard m_leaderboard;

    private const string m_HIGHSCORE_PREFKEY = "HighScore";
    private const string m_PLAYCOUNT_PREFKEY = "PlayCount";

    private int m_currentScore = 0;

    private int m_highScore;
    private int m_gamePlayCount;

    private void Start()
    {
        m_highScore = PlayerPrefs.GetInt(m_HIGHSCORE_PREFKEY);
        m_gamePlayCount = PlayerPrefs.GetInt(m_PLAYCOUNT_PREFKEY);

        ResetScore();
    }

    public void ResetScore()
    {
        m_currentScore = 0;
        UIManager.I.UpdateScoreCount(m_currentScore);
        UIManager.I.UpdateHighScoreCount(m_highScore);
    }

    public void AddScore(int _points)
    {
        m_currentScore += _points;
        UIManager.I.UpdateScoreCount(m_currentScore);
    }

    public void SubmitScore()
    {
        m_leaderboard.SetEntry(UIManager.I.GetUsernameIF().text, m_currentScore);

        if (IsHighScore())
        {
            UpdateHighScore();
        }

        UpdatePlayCount();
    }

    private void UpdatePlayCount()
    {
        m_gamePlayCount += 1;
        PlayerPrefs.SetInt(m_PLAYCOUNT_PREFKEY, m_gamePlayCount);
    }

    private bool IsHighScore()
    {
        return m_currentScore > m_highScore;
    }

    private void UpdateHighScore()
    {
        m_highScore = m_currentScore;
        PlayerPrefs.SetInt(m_HIGHSCORE_PREFKEY, m_highScore);
    }
}
