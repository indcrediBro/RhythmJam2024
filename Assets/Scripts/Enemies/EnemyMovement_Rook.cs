using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement_Rook : MonoBehaviour
{
    [SerializeField] private string m_sfxName = "Rook_Move";
    [SerializeField] private Transform m_playerTF;
    public void SetPlayerTarget(Transform _target) { m_playerTF = _target; }

    private float m_moveDelay;
    private float m_moveDuration;
    private int m_tilesToMove; // Dynamically calculated

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
        m_moveDelay = BeatManager.I.GetIntervalLength(BeatType.Beat);
    }

    private void FollowPlayer()
    {
        if (!isMoving && m_playerTF.gameObject.activeInHierarchy)
        {
            if (m_playerTF != null)
            {
                Vector3 direction = CalculateDirection(m_playerTF.position);
                m_tilesToMove = CalculateTilesToMove(m_playerTF.position);

                StartCoroutine(MoveTiles(direction, m_tilesToMove));
            }
        }
    }

    Vector3 CalculateDirection(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;
        float xDiff = targetPosition.x - currentPosition.x;
        float yDiff = targetPosition.y - currentPosition.y;

        if (Mathf.Abs(xDiff) > Mathf.Abs(yDiff))
        {
            return new Vector3(Mathf.Sign(xDiff), 0f, 0f); // Move horizontally
        }
        else
        {
            return new Vector3(0f, Mathf.Sign(yDiff), 0f); // Move vertically
        }
    }

    int CalculateTilesToMove(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;
        float xDiff = Mathf.Abs(targetPosition.x - currentPosition.x);
        float yDiff = Mathf.Abs(targetPosition.y - currentPosition.y);

        // Round up to ensure at least 1 tile is moved
        return Mathf.Max(Mathf.CeilToInt(xDiff), Mathf.CeilToInt(yDiff));
    }

    IEnumerator MoveTiles(Vector3 direction, int tilesToMove)
    {
        isMoving = true;
        AudioManager.I.PlaySound(m_sfxName);

        for (int i = 0; i < tilesToMove; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + direction;

            Collider2D[] col = Physics2D.OverlapCircleAll(endPosition, .3f);

            foreach (var c in col)
            {
                if (c.CompareTag("Enemy"))
                {
                    StopCoroutine(MoveTiles(Vector3.zero,0));
                    break;
                }
            }

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
            yield return new WaitForSeconds(m_moveDelay/4);
        }

        yield return new WaitForSeconds(m_moveDelay);
        isMoving = false;
    }
}
