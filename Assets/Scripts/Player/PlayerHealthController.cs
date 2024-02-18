using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : Health
{
    [SerializeField] private UnityEngine.UI.Slider m_healthUISlider;

    private void LateUpdate()
    {
        if (m_healthUISlider) { DisplayHealth(); }
    }

    void DisplayHealth()
    {
        m_healthUISlider.value = GetCurrentHealthValue();
    }

    protected override void Die()
    {
        base.Die();
    }
}
