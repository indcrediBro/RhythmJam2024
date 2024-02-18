using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement_Bishop : MonoBehaviour
{
    [SerializeField] private string m_sfxName = "Bishop_Move";
    [SerializeField] private Transform m_playerTF;
    public void SetPlayerTarget(Transform _target) { m_playerTF = _target; }

    private float m_moveDelay;
    private float m_moveDuration;

    private bool isMoving = false;

    private void OnEnable()
    {
        BeatManager.SubscribeToBeatEvent(FollowPlayer);
    }

    private void OnDisable()
    {
        BeatManager.UnsubscribeFromBeatEvent(FollowPlayer);
    }

    void Start()
    {
        m_moveDuration = BeatManager.I.GetIntervalLength(BeatType.Beat) / 2;
        m_moveDelay = BeatManager.I.GetIntervalLength(BeatType.Beat) * 4;
    }

    private void FollowPlayer()
    {
        if (!isMoving)
        {
            if (m_playerTF != null)
            {
                Vector3 direction = CalculateDirection(m_playerTF.position);
                int tilesToMove = CalculateTilesToMove(m_playerTF.position);

                if (tilesToMove > 0)
                {
                    StartCoroutine(MoveDiagonallyTiles(direction, tilesToMove));
                }
                else
                {
                    // If not possible to reach player diagonally, find a random diagonal target
                    Vector3 randomDiagonalTarget = FindRandomDiagonalTarget();
                    StartCoroutine(MoveDiagonallyTiles(randomDiagonalTarget.normalized, tilesToMove));
                }
            }
        }
    }

    Vector3 CalculateDirection(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;
        return (targetPosition - currentPosition).normalized;
    }

    int CalculateTilesToMove(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;
        float xDiff = Mathf.Abs(targetPosition.x - currentPosition.x);
        float yDiff = Mathf.Abs(targetPosition.y - currentPosition.y);

        // Round up to ensure at least 1 tile is moved
        return Mathf.Max(Mathf.CeilToInt(xDiff), Mathf.CeilToInt(yDiff));
    }

    Vector3 FindRandomDiagonalTarget()
    {
        // Randomly choose a direction (diagonal)
        int randomX = Random.Range(-1, 2);
        int randomY = Random.Range(-1, 2);

        // Avoid moving directly horizontally or vertically
        if (randomX != 0 && randomY != 0)
        {
            return new Vector3(randomX, randomY, 0f);
        }
        else
        {
            return FindRandomDiagonalTarget(); // Try again if horizontal or vertical
        }
    }

    IEnumerator MoveDiagonallyTiles(Vector3 direction, int tilesToMove)
    {
        isMoving = true;
        AudioManager.I.PlaySound(m_sfxName);

        for (int i = 0; i < tilesToMove; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + direction;

            // Ensure the next position stays within the grid boundaries
            endPosition.x = Mathf.Clamp(endPosition.x, 0f, 7f);
            endPosition.y = Mathf.Clamp(endPosition.y, 0f, 7f);

            float elapsedTime = 0;

            while (elapsedTime <= m_moveDuration)
            {
                elapsedTime += Time.deltaTime;
                float percent = elapsedTime / m_moveDuration;
                transform.position = Vector3.Lerp(startPosition, endPosition, percent);
                yield return null;
            }

            transform.position = new Vector3(Mathf.FloorToInt(endPosition.x), Mathf.FloorToInt(endPosition.y), 0f);
            AudioManager.I.PlaySound(m_sfxName);
            yield return new WaitForSeconds(m_moveDelay/2);
        }

        yield return new WaitForSeconds(m_moveDelay * 2);
        isMoving = false;
    }
}
