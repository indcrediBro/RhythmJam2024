using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int m_levelNumber;
    public EnemyType[] m_initialEnemies;
    public EnemyType[] m_remainingEnemies;
    public int m_maxEnemiesAtTime;
}

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private LevelData[] m_levelData;
    [SerializeField] private GameObject[] m_allEnemyPrefabs;

    private Transform m_playerTF;

    private int m_currentLevel = 0;
    private int m_killCount = 0;

    private int m_maxEnemiesAtTime = 1;
    private int m_maxEnemyModifier;
    [SerializeField] bool m_isStoryMode = true;

    private List<GameObject> m_spawnedEnemies;
    private List<GameObject> m_remainingEnemies;

    private void OnEnable()
    {

        m_playerTF = GameManager.I.GetPlayerTransform();
        m_spawnedEnemies = new List<GameObject>();
        m_remainingEnemies = new List<GameObject>();
        m_isStoryMode = GameManager.I.IsStoryMode();

        if (m_isStoryMode)
        {
            InitializeStoryMode();
        }
        else
        {
            InitializeRandomMode();
        }
    }

    private void InitializeStoryMode()
    {
        m_currentLevel = 0;
        SpawnLevelEnemies();
    }

    private void InitializeRandomMode()
    {
        m_killCount = 0;
        m_maxEnemiesAtTime = 1;
        m_maxEnemyModifier = 3;
        int initialSpawnCount = 3;

        foreach (GameObject _obj in m_allEnemyPrefabs)
        {
            for (int i = 0; i < initialSpawnCount; i++)
            {
                GameObject newObj = Instantiate(_obj, this.transform);
                newObj.GetComponent<Enemy>().SetPlayerTransform(m_playerTF);
                newObj.SetActive(false);

                m_remainingEnemies.Add(newObj);
            }
        }

        GameObject obj = Instantiate(m_allEnemyPrefabs[0], this.transform);
        obj.GetComponent<Enemy>().SetPlayerTransform(m_playerTF);
        m_spawnedEnemies.Add(obj);
        while (Mathf.RoundToInt(obj.transform.position.x) == -1f || Mathf.RoundToInt(obj.transform.position.y) == -1f)
        {
            obj.transform.position = GetRandomFreeTile();
        }
        obj.SetActive(true);
    }

    private void SpawnLevelEnemies()
    {
        foreach (var enemy in m_levelData[m_currentLevel].m_initialEnemies)
        {
            GameObject obj = Instantiate(GetPrefab(enemy), this.transform);
            obj.transform.position = GetRandomFreeTile();

            while(Mathf.RoundToInt( obj.transform.position.x) == -1f || Mathf.RoundToInt(obj.transform.position.y) == -1f)
            {
                obj.transform.position = GetRandomFreeTile();
            }

            obj.GetComponent<Enemy>().SetPlayerTransform(m_playerTF);

            obj.SetActive(true);

            m_spawnedEnemies.Add(obj);
        }

        foreach (var enemy in m_levelData[m_currentLevel].m_remainingEnemies)
        {
            GameObject obj = Instantiate(GetPrefab(enemy), this.transform);
            obj.GetComponent<Enemy>().SetPlayerTransform(m_playerTF);

            obj.SetActive(false);

            m_remainingEnemies.Add(obj);
        }
    }

    private Vector2 GetRandomFreeTile()
    {
        int x = Random.Range(0, 7);
        int y = Random.Range(0, 7);
        Vector3 randomPosition = new Vector2(x, y);
        while (randomPosition != m_playerTF.position)
        {
            if (Vector2.Distance(m_playerTF.position, randomPosition) > 4)
            {
                return randomPosition;
            }
            else
            {
                x = Random.Range(0, 7);
                y = Random.Range(0, 7);
                randomPosition = new Vector2(x, y);
            }
        }
        return  Vector2.one * -1;
    }

    private GameObject GetPrefab(EnemyType _type)
    {
        foreach (var obj in m_allEnemyPrefabs)
        {
            if(obj.GetComponent<Enemy>().GetEnemyType() == _type)
            {
                return obj;
            }
        }

        return null;
    }

    private void Update()
    {
        TrackSpawnedEnemies();

        if (m_isStoryMode)
        {
            if(m_spawnedEnemies.Count < m_levelData[m_currentLevel].m_maxEnemiesAtTime
                && m_remainingEnemies.Count > 0)
            {
                SpawnNextRemainingEnemy();
            }

            if (m_spawnedEnemies.Count == 0 && m_remainingEnemies.Count == 0)
            {
                loadNextLevelEnemies();
            }

            if(m_currentLevel > m_levelData.Length)
            {
                GameManager.I.GameOver();
            }

            UIManager.I.UpdateLevelCount(m_currentLevel);
        }
        else
        {
            //Todo: Implement Random Spawning Logic
            if (m_spawnedEnemies.Count < m_maxEnemiesAtTime)
            {
                m_killCount++;

                SpawnNextRandomEnemy();
            }
        }
    }

    private void TrackSpawnedEnemies()
    {
        if (m_spawnedEnemies.Count > 0)
        {
            for (int i = 0; i < m_spawnedEnemies.Count; i++)
            {
                if (m_spawnedEnemies[i] == null)
                {
                    m_spawnedEnemies.RemoveAt(i);
                }
            }
        }
    }

    private void SpawnNextRemainingEnemy()
    {
        GameObject nextEnemy = m_remainingEnemies[0];

        m_spawnedEnemies.Add(nextEnemy);
        m_remainingEnemies.Remove(nextEnemy);

        nextEnemy.transform.position = GetRandomFreeTile();
        while (Mathf.RoundToInt(nextEnemy.transform.position.x) == -1f || Mathf.RoundToInt(nextEnemy.transform.position.y) == -1f)
        {
            nextEnemy.transform.position = GetRandomFreeTile();
        }
        nextEnemy.SetActive(true);
    }

    private void loadNextLevelEnemies()
    {
        m_currentLevel++;

        SpawnLevelEnemies();
    }

    private void SpawnNextRandomEnemy()
    {
        GameObject nextEnemy = m_remainingEnemies[Random.Range(0,m_remainingEnemies.Count)];

        m_spawnedEnemies.Add(nextEnemy);
        m_remainingEnemies.Remove(nextEnemy);

        nextEnemy.transform.position = GetRandomFreeTile();
        while (Mathf.RoundToInt(nextEnemy.transform.position.x) == -1f || Mathf.RoundToInt(nextEnemy.transform.position.y) == -1f)
        {
            nextEnemy.transform.position = GetRandomFreeTile();
        }
        nextEnemy.SetActive(true);

        if(m_remainingEnemies.Count < 1)
        {
            GameObject newObj = Instantiate(m_allEnemyPrefabs[Random.Range(0,m_allEnemyPrefabs.Length)], this.transform);
            newObj.GetComponent<Enemy>().SetPlayerTransform(m_playerTF);
            newObj.SetActive(false);

            m_remainingEnemies.Add(newObj);
        }

        if (m_killCount % m_maxEnemyModifier == 0)
        {
            m_maxEnemiesAtTime++;
            m_maxEnemyModifier *= 2;
        }
    }

    private void ResetEnemyManager()
    {
        m_currentLevel = 0;
        if (m_spawnedEnemies?.Count > 0)
        {
            for (int i = 0; i < m_spawnedEnemies.Count; i++)
            {
                GameObject obj = m_spawnedEnemies[i];
                m_spawnedEnemies.Remove(obj);
                Destroy(obj);
            }
        }
        if (m_remainingEnemies?.Count > 0)
        {
            for (int i = 0; i < m_spawnedEnemies.Count; i++)
            {
                GameObject obj = m_spawnedEnemies[i];
                m_remainingEnemies.Remove(obj);
                Destroy(obj);
            }
        }
    }

    private void OnDisable()
    {
        ResetEnemyManager();
    }
}
