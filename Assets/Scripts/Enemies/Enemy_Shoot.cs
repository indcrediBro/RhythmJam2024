using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shoot : MonoBehaviour
{
    [SerializeField] private string m_sfxName = "King_Shoot";
    [SerializeField] private Transform m_playerTF;
    public void SetPlayerTarget(Transform _target) { m_playerTF = _target; }

    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private Transform m_shootPosition;

    private void OnEnable()
    {
        m_playerTF = GameManager.I.GetPlayerTransform();
        BeatManager.SubscribeToBeatEvent(ShootPlayer);
    }

    private void OnDisable()
    {
        BeatManager.UnsubscribeFromBeatEvent(ShootPlayer);
    }

    private void ShootPlayer()
    {
        //float angleZ = CalculateAngle(m_shootPosition.position, m_playerTF.position);
        Instantiate(m_bulletPrefab, m_shootPosition.position, Quaternion.identity);
        AudioManager.I.PlaySound(m_sfxName);
    }

    
}
