using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour
{
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        // Add event triggers for mouse over and mouse click
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        // Mouse over event
        EventTrigger.Entry entryOver = new EventTrigger.Entry();
        entryOver.eventID = EventTriggerType.PointerEnter;
        entryOver.callback.AddListener((data) => { OnMouseOver(); });
        trigger.triggers.Add(entryOver);

        // Mouse click event
        EventTrigger.Entry entryClick = new EventTrigger.Entry();
        entryClick.eventID = EventTriggerType.PointerClick;
        entryClick.callback.AddListener((data) => { OnMouseClick(); });
        trigger.triggers.Add(entryClick);

        // Mouse exit event
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnMouseExit(); });
        trigger.triggers.Add(entryExit);
    }

    void OnMouseOver()
    {
        // Scale up on mouse over
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.2f);
        AudioManager.I.PlaySound("UI_Hover");
    }

    void OnMouseClick()
    {
        // Add your click logic here if needed
        LeanTween.scale(gameObject, originalScale * 0.8f, 0.1f).setOnComplete(()=>
            LeanTween.scale(gameObject, originalScale, 0.1f)
        );
        AudioManager.I.PlaySound("UI_Click");
    }

    void OnMouseExit()
    {
        // Revert back to normal scale on mouse exit
        LeanTween.scale(gameObject, originalScale, 0.2f);
    }
}
