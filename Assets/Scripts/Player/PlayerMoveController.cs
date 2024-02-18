using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private PlayerScoreController m_playerScore;
    private PlayerAttackController m_playerAttack;
    // Time in seconds to move between one grid position and the next.
    [SerializeField] private float m_moveDuration = 0.1f;

    // The size of the grid
    private float m_gridSize = 1f;
    private bool m_isMoving = false;
    public bool GetIsMoving() { return m_isMoving; }
    private void Awake()
    {
        m_moveDuration = BeatManager.I.GetIntervalLength(BeatType.Beat);
    }

    private void Start()
    {
        m_playerAttack = GetComponent<PlayerAttackController>();
        m_playerScore = GetComponent<PlayerScoreController>();

        InputManager.I.PlayerMoveInput().performed += ctx => HandleMovement(ctx.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        m_isMoving = false;
    }

    private void HandleMovement(Vector2 direction)
    {
        if (!m_isMoving && gameObject.activeInHierarchy && !m_playerAttack.GetIsAttacking())
        {

            if (direction.y > 0)
            {
                if (transform.position.y >= 0 && transform.position.y < 7)
                {
                    StartCoroutine(Move(Vector2.up));
                }
            }
            else if (direction.y < 0)
            {
                if (transform.position.y > 0 && transform.position.y <= 7)
                {
                    StartCoroutine(Move(Vector2.down));
                }
            }
            else if (direction.x < 0)
            {
                if (transform.position.x <= 7 && transform.position.x > 0)
                {
                    StartCoroutine(Move(Vector2.left));
                }
            }
            else if (direction.x > 0)
            {
                if (transform.position.x >= 0 && transform.position.x < 7)
                {
                    StartCoroutine(Move(Vector2.right));
                }
            }

            m_playerScore.CheckTiming(BeatManager.I.GetSourceTime());
        }
    }

    private IEnumerator Move(Vector2 direction)
    {
        m_isMoving = true;
        AudioManager.I.PlaySound("Player_Move");

        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + (direction * m_gridSize);

        float elapsedTime = 0;
        while (elapsedTime <= m_moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / m_moveDuration;
            transform.position = Vector2.Lerp(startPosition, endPosition, percent);
            yield return null;
        }

        transform.position = new Vector3(Mathf.FloorToInt(endPosition.x), Mathf.FloorToInt(endPosition.y), 0f);
        m_isMoving = false;
        AudioManager.I.PlaySound("Player_Stop");
    }
}
