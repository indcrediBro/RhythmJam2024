using System.Collections;
using UnityEngine;

public class EnemyMovement_Pawn : MonoBehaviour
{
    [SerializeField] private string m_sfxName = "Pawn_Move";
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
        m_moveDuration = BeatManager.I.GetIntervalLength(BeatType.Beat);
        m_moveDelay = BeatManager.I.GetIntervalLength(BeatType.Beat);
    }

    private void FollowPlayer()
    {
        if (!isMoving && m_playerTF.gameObject.activeInHierarchy)
        {
            if (m_playerTF != null)
            {
                Vector3 direction = CalculateDirection(m_playerTF.position);

                StartCoroutine(MoveOneTile(direction));
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
    IEnumerator MoveOneTile(Vector3 direction)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + direction;

        Collider2D[] col = Physics2D.OverlapCircleAll(endPosition, .3f);

        foreach (var c in col)
        {
            if (c.CompareTag("Enemy"))
            {
                StopCoroutine(MoveOneTile(Vector3.zero));
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
        yield return new WaitForSeconds(m_moveDelay);
        isMoving = false;
    }
}
