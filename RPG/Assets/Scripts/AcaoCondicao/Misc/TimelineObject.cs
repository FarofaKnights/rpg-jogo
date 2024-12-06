using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Cutscene))]
public class TimelineObject : MonoBehaviour
{
    [Header("Cutscene Referenciavel")]
    public string timelineId;

    public Cutscene cutscene { get { return GetComponent<Cutscene>(); } }
}
