using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Game Panels")]
    [SerializeField] private GameObject m_mainMenuPanel;
    [SerializeField] private GameObject m_gamePanel;
    [SerializeField] private GameObject m_pausePanel;
    [SerializeField] private GameObject m_settingsPanel;
    [SerializeField] private GameObject m_gameOverPanel;


    [Header("Leaderboard UI")]
    [SerializeField] private GameObject m_leaderboardPanel;
    [SerializeField] private GameObject m_leaderboardSubmissionUI;
    [SerializeField] private TMP_InputField m_leaderboardNameInputUI;
    public TMP_InputField GetUsernameIF() { return m_leaderboardNameInputUI; }

    [Header("Story Mode UI")]
    [SerializeField] private GameObject m_storyModeLevelCountUI;
    [SerializeField] private TMP_Text m_currentLevelText;

    [Header("Score Mode UI")]
    [SerializeField] private GameObject m_currentScoreUI;
    [SerializeField] private GameObject m_highScoreUI;
    [SerializeField] private GameObject m_gameoverScoreUI;
    [SerializeField] private TMP_Text m_currentScoreText;
    [SerializeField] private TMP_Text m_highScoreText, m_gameoverScoreText;

    public void UpdateLevelCount(int _count)
    {
        m_currentLevelText.text = (_count + 1).ToString();
    }
    public void UpdateScoreCount(int _count)
    {
        m_currentScoreText.text = (_count).ToString();
        m_gameoverScoreText.text = (_count).ToString();
    }
    public void UpdateHighScoreCount(int _count)
    {
        m_highScoreText.text = _count.ToString();
    }

    public void MainMenuUI()
    {
        if (!m_mainMenuPanel.activeInHierarchy)
        {
            m_mainMenuPanel.SetActive(true);
        }
    }

    public void GameUI(bool _isStoryMode)
    {
        m_gamePanel.SetActive(true);

        m_mainMenuPanel.SetActive(false);
        m_gameOverPanel.SetActive(false);

        m_storyModeLevelCountUI.SetActive(_isStoryMode);
        m_currentScoreUI.SetActive(!_isStoryMode);
        m_highScoreUI.SetActive(!_isStoryMode);
        m_gameoverScoreUI.SetActive(!_isStoryMode);
    }

    public void PauseUI(bool _value)
    {
        m_pausePanel.SetActive(_value);
    }

    public void SettingsUI()
    {
        m_settingsPanel.SetActive(true);
    }

    public void GameOverUI()
    {
        m_gameOverPanel.SetActive(true);
    }

    public void LeaderboardUI()
    {
        m_leaderboardPanel.SetActive(true);
    }

    public void LeaderboardSubmissionUI(bool _value)
    {
        m_leaderboardSubmissionUI.SetActive(_value);
    }

    public bool IsNameInputFieldEmpty()
    {
        return m_leaderboardNameInputUI.text != string.Empty;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartApp()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
