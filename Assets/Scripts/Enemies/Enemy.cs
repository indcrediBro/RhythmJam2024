using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Pawn, Rook, Knight, Bishop, King, Queen
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyType m_enemyType;
    public EnemyType GetEnemyType() { return m_enemyType; }

    private Transform m_playerTF;
    public void SetPlayerTransform(Transform _transform)
    {
        m_playerTF = _transform;
    }

    private void Awake()
    {
        if (m_playerTF == null)
        {
            m_playerTF = FindObjectOfType<PlayerMoveController>().transform;
        }
        InitializeAIMovement();
    }

    private void InitializeAIMovement()
    {
        switch (m_enemyType)
        {
            case EnemyType.Pawn:
                gameObject.AddComponent<EnemyMovement_Pawn>()
                    .SetPlayerTarget(m_playerTF);
                break;
            case EnemyType.Rook:
                gameObject.AddComponent<EnemyMovement_Rook>()
                    .SetPlayerTarget(m_playerTF);
                break;
            case EnemyType.Knight:
                break;
            case EnemyType.Bishop:
                gameObject.AddComponent<EnemyMovement_Bishop>()
                    .SetPlayerTarget(m_playerTF);
                break;
            case EnemyType.King:
                gameObject.AddComponent<EnemyMovement_King>()
                    .SetPlayerTarget(m_playerTF);
                break;
            case EnemyType.Queen:
                break;
        }
    }

    public int GetKillPoint()
    {
        switch (m_enemyType)
        {
            case EnemyType.Pawn:
                return 50;
            case EnemyType.Rook:
                return 100;
            case EnemyType.Knight:
                return 100;
            case EnemyType.Bishop:
                return 100;
            case EnemyType.King:
                return 1000;
            case EnemyType.Queen:
                return 500;
            default:
                return 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            m_playerTF.GetComponent<PlayerHealthController>().TakeDamage(100);
        }
    }
}
