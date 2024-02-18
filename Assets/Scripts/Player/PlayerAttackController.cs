using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private GameObject m_attackObject;
    private PlayerMoveController m_playerMove;
    private PlayerScoreController m_playerScore;

    private bool m_isAttacking;
    private float m_duration;
    [SerializeField] private LayerMask m_enemyLayer;

    void Start()
    {
        m_playerMove = GetComponent<PlayerMoveController>();
        m_playerScore = GetComponent<PlayerScoreController>();

        m_duration = BeatManager.I.GetIntervalLength(BeatType.Beat);

        InputManager.I.PlayerAttackInput().performed += ctx => HandleAttack(ctx.ReadValue<Vector2>());
        m_attackObject.SetActive(false);
    }
    public bool GetIsAttacking() { return m_isAttacking; }

    private void HandleAttack(Vector2 direction)
    {
        if (!m_isAttacking && gameObject.activeInHierarchy)
        {
            if (direction.y > 0)
            {
                StartCoroutine(Attack(Vector2.up));
            }
            else if (direction.y < 0)
            {
                StartCoroutine(Attack(Vector2.down));
            }
            else if (direction.x < 0)
            {
                StartCoroutine(Attack(Vector2.left));
            }
            else if (direction.x > 0)
            {
                StartCoroutine(Attack(Vector2.right));
            }

            m_playerScore.CheckTiming(BeatManager.I.GetSourceTime());
        }
    }

    private IEnumerator Attack(Vector2 direction)
    {
        m_isAttacking = true;

        AudioManager.I.PlaySound("Player_Attack");

        Vector2 attackPosition = (Vector2)transform.position + (direction);

        m_attackObject.transform.position = attackPosition;
        m_attackObject.SetActive(true);

        Collider2D[] collisions = Physics2D.OverlapCircleAll(m_attackObject.transform.position, 0.4f,m_enemyLayer);

        if(collisions.Length > 0)
        {
            foreach (var col in collisions)
            {
                if (col.CompareTag("Enemy"))
                {
                    ScoreManager.I.AddScore(col.GetComponent<Enemy>().GetKillPoint());
                    Destroy(col.gameObject);
                }
            }
        }

        yield return new WaitForSeconds(m_duration);

        m_attackObject.SetActive(false);

        m_isAttacking = false;
        AudioManager.I.PlaySound("Player_Stop");
    }
}
