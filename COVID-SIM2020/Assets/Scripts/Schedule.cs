using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schedule : MonoBehaviour
{
    [SerializeField]
    public GameObject[] mySchedule;

    public void Awake()
    {
        mySchedule = new GameObject[GameManager.NUMBER_OF_PERIODS];
    }

    public void SetScheduleRoom(GameObject room, int periodIndex)
    {
        mySchedule[periodIndex] = room;
    }
}
