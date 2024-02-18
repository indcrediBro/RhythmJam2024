using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Game,
    Pause,
    Gameover
}

public class GameManager : Singleton<GameManager>
{
    private GameState m_gameState;

    private bool m_isStoryMode, m_isGameStarted;
    public bool IsStoryMode() { return m_isStoryMode; }
    [SerializeField] private GameObject m_playerGO;
    [SerializeField] private GameObject m_enemyManagerGO;
    [SerializeField] private GameObject m_gridManagerGO;

    public Transform GetPlayerTransform() { return m_playerGO.transform; }

    private void Start()
    {
        m_gameState = GameState.MainMenu;
    }

    private void Update()
    {
        HandleGameState();
    }

    private void HandleGameState()
    {
        switch (m_gameState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                m_isGameStarted = false;
                break;
            case GameState.Game:
                Time.timeScale = 1f;
                if (!m_playerGO.activeInHierarchy)
                {
                    GameOver();
                }
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
            case GameState.Gameover:
                Time.timeScale = 1f;
                break;
            default:
                break;
        }
    }

    public void StartStoryMode()
    {
        m_gameState = GameState.Game;
        m_isStoryMode = true;

        UIManager.I.GameUI(true);
        UIManager.I.LeaderboardSubmissionUI(false);

        m_gridManagerGO.SetActive(true);

        m_playerGO.transform.position = new Vector3Int(Random.Range(0, 7), Random.Range(0, 7));
        m_playerGO.SetActive(true);

        m_enemyManagerGO.SetActive(true);

        m_isGameStarted = true;
    }

    public void StartLeaderboardMode()
    {
        m_gameState = GameState.Game;
        m_isStoryMode = false;

        UIManager.I.GameUI(false);
        UIManager.I.LeaderboardSubmissionUI(true);

        m_gridManagerGO.SetActive(true);

        m_playerGO.transform.position = new Vector3Int(Random.Range(0, 7), Random.Range(0, 7));
        m_playerGO.SetActive(true);

        m_enemyManagerGO.SetActive(true);

        m_isGameStarted = true;
    }

    public void UnpauseGame()
    {
        m_gameState = GameState.Game;

        UIManager.I.PauseUI(false);
    }

    public void PauseGame()
    {
        m_gameState = GameState.Pause;

        UIManager.I.PauseUI(true);
    }

    public void GameOver()
    {
        m_gameState = GameState.Gameover;

        m_gridManagerGO.SetActive(false);
        m_enemyManagerGO.SetActive(false);
        m_isGameStarted = false;

        UIManager.I.GameOverUI();
    }

    public void SubmitScore()
    {
        if (UIManager.I.IsNameInputFieldEmpty())
        {
            UIManager.I.LeaderboardUI();
            UIManager.I.LeaderboardSubmissionUI(false);
            ScoreManager.I.SubmitScore();
        }
    }

    public void RetryGame()
    {
        if (m_isStoryMode)
        {
            StartStoryMode();
        }
        else
        {
            StartLeaderboardMode();
        }
    }
}
