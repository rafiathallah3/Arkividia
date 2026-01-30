using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EventSituasi
{
    [Min(0f)]
    public float delay;

    public UnityEvent onEventTriggered;
}
