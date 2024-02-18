using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_bulletRB;
    [SerializeField] private Transform m_bulletTF;

    [SerializeField] private int m_bulletDamage;
    [SerializeField] private float m_bulletSpeed;
    [SerializeField] private float m_bulletLifeTime;
    [SerializeField] private bool m_lifeTimeDestroy;
    private float m_despawnTimer;

    [SerializeField] private string m_impactSFX = "Bullet_Impact";
    [SerializeField] private GameObject m_bulletImpactVFX;

    private void Start()
    {
        //transform.LookAt(GameManager.I.GetPlayerTransform(), Vector3.forward);
        transform.rotation = Quaternion.Euler(0f, 0f, CalculateAngle(transform.position, GameManager.I.GetPlayerTransform().position));
        LaunchBullet();
    }

    private float CalculateAngle(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = toPosition - fromPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void LaunchBullet()
    {
        m_bulletRB.velocity = (transform.position-GameManager.I.GetPlayerTransform().position) * -m_bulletSpeed;
    }
    private void Update()
    {
        if (m_lifeTimeDestroy)
        {
            CountBulletLifetimeDestroy();
        }
    }

    private void CountBulletLifetimeDestroy()
    {
        m_despawnTimer += Time.deltaTime;
        if (m_despawnTimer >= m_bulletLifeTime)
        {
            m_despawnTimer = 0;
            Destroy(gameObject);
        }
    }

    private void Impact(Collider2D _otherCollider)
    {
        Health health = _otherCollider.GetComponent<PlayerHealthController>();

        if (health != null)
        {
            health.TakeDamage(m_bulletDamage);
        }

        if (m_bulletImpactVFX) PlayImpactVFX();

        AudioManager.I.PlaySound(m_impactSFX);

        Destroy(gameObject);
    }

    private void PlayImpactVFX()
    {
        if (!m_bulletImpactVFX) return;

        float vfxOffsetFromWall = 0.1f;

        var obj = Instantiate(m_bulletImpactVFX, m_bulletTF.position, m_bulletTF.rotation);
        Transform objTF = obj.transform;

        Vector3 playerDir = GameManager.I.GetPlayerTransform().position - objTF.position;
        objTF.position += playerDir * vfxOffsetFromWall;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Enemy"))
            Impact(collision.collider);
    }
}
