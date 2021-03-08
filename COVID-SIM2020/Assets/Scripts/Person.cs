using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour
{
    private NavMeshAgent myAgent;
    private Schedule mySchedule;

    public bool onSchoolPremesis = false;
    int prio;

    void Awake()
    {
        myAgent = GetComponent<NavMeshAgent>();
        mySchedule = GetComponent<Schedule>();
    }

    void Start()
    {
        GameManager._instance.NextPeriod += NextPeriod;

        prio = GetComponent<NavMeshAgent>().avoidancePriority;
    }

    void Update()
    {
        if (GetComponent<NavMeshAgent>().remainingDistance <= 0.25)
            GetComponent<NavMeshAgent>().avoidancePriority = 100;
        else
            GetComponent<NavMeshAgent>().avoidancePriority = prio;
    }

    void NextPeriod()
    {
        mySchedule.mySchedule[GameManager._instance.currentPeriod].GetComponent<Room>().peopleInThisRoom.Add(gameObject);
    }

    private void OnDestroy()
    {
        GameManager._instance.NextPeriod -= NextPeriod;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.name == "SCHOOLPREMESIS")
        {
            onSchoolPremesis = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "SCHOOLPREMESIS")
        {
            onSchoolPremesis = true;
        }
    }
}
