using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    private void OnEnable()
    {
        LeanTween.alphaCanvas(m_canvasGroup, 1f, 0f);
        LeanTween.alphaCanvas(m_canvasGroup, 0f, BeatManager.I.GetIntervalLength(BeatType.Beat)*2).setEaseInExpo().setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
