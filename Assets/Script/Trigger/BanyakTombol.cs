using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanyakTombol : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("List of buttons that must be pressed to trigger the events.")]
    public List<TombolLevel> buttons;
    
    [Tooltip("List of events to execute when all buttons are pressed.")]
    public List<EventSituasi> events;

    private HashSet<TombolLevel> pressedButtons = new HashSet<TombolLevel>();
    private bool areEventsTriggered = false;

    void Start()
    {
        if (buttons == null)
        {
            buttons = new List<TombolLevel>();
        }

        // Subscribe to each button's OnPressed event
        foreach (var button in buttons)
        {
            if (button != null)
            {
                button.OnPressed.AddListener(() => OnButtonActivated(button));
            }
        }
    }

    private void OnButtonActivated(TombolLevel button)
    {
        if (areEventsTriggered) return;

        if (!pressedButtons.Contains(button))
        {
            pressedButtons.Add(button);
            CheckAllButtonsPressed();
        }
    }

    private void CheckAllButtonsPressed()
    {
        // Filter out any potential nulls if buttons were destroyed (unlikely but good safety)
        buttons.RemoveAll(item => item == null);

        if (pressedButtons.Count >= buttons.Count)
        {
            // Verify all specific unique buttons in the list are in the set
            // (HashSet logic already ensures distinct counts, so Count check should suffice if setup is correct)
            // But to be safe, let's verify exact matches if duplicates were allowed in 'buttons' list (Unity Inspector allows duplicates)
            bool allPressed = true;
            foreach (var btn in buttons)
            {
                if (!pressedButtons.Contains(btn))
                {
                    allPressed = false;
                    break;
                }
            }

            if (allPressed)
            {
                StartCoroutine(ExecuteEvents());
            }
        }
    }

    private IEnumerator ExecuteEvents()
    {
        areEventsTriggered = true;

        foreach (var eventSituasi in events)
        {
            if (eventSituasi.delay > 0)
            {
                yield return new WaitForSeconds(eventSituasi.delay);
            }
            
            eventSituasi.onEventTriggered?.Invoke();
        }
    }
}
