using System.Collections.Generic;
using UnityEngine;

public class LevelConfig : MonoBehaviour
{
    [Header("Intro Dialogue Settings")]
    public bool hasIntroDialogue = true;
    [TextArea(3, 10)]
    public string introDialogueText = "";

    [Header("Intro Events")]
    public List<EventSituasi> introEvents;
}
