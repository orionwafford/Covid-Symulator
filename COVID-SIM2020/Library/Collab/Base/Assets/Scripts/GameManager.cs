using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum GameState { GameStart, RunningDay, EndOfDay}

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public GameState currentGameState;

    public GameObject roomsObject;
    private List<GameObject> rooms = new List<GameObject>();

    public GameObject peoplesObject;
    private List<GameObject> people = new List<GameObject>();

    public GameObject outsides;

    public int currentDay;

    public static int NUMBER_OF_PERIODS = 3;
    public int currentPeriod = 0;

    public UnityAction StartDay;
    public UnityAction PrepRooms;
    public UnityAction NextPeriod;
    public UnityAction GoToRooms;
    public UnityAction EndDay;

    public int notInfected;
    public int potentialInfected;
    public int asymptomaticInfected;
    public int presymptomaticInfected;
    public int fullyInfected;

    public int prev_notInfected;
    public int prev_potentialInfected;
    public int prev_asymptomaticInfected;
    public int prev_presymptomaticInfected;
    public int prev_fullyInfected;
    //public TextMeshProUGUI txtNotInf;
    //public TextMeshProUGUI txtPotentialInf;
    //public TextMeshProUGUI txtAsympInf;
    //public TextMeshProUGUI txtFullyInf;
    //public TextMeshProUGUI txtPreSymptomaticInf;
    //public TextMeshProUGUI txtPeriod;
    //public TextMeshProUGUI txtDay;

    void Awake()
    {
        _instance = this;

        Time.timeScale = 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentGameState = GameState.GameStart;
        currentDay = 0;

        foreach (Transform room in roomsObject.transform)
        {
            rooms.Add(room.gameObject);
        }
        foreach (Transform person in peoplesObject.transform)
        {
            people.Add(person.gameObject);

            ///calculate how many people are infected/not infected at beginning of game.
            if (person.GetComponent<Infection>().currentState == InfectionType.NotInfected)
            {
                notInfected++;
            }
            else if (person.GetComponent<Infection>().currentState == InfectionType.PotentialInfection)
            {
                potentialInfected++;
            }
            else if (person.GetComponent<Infection>().currentState == InfectionType.AsymptomaticInfection)
            {
                asymptomaticInfected++;
            }
            else if (person.GetComponent<Infection>().currentState == InfectionType.PreSymptomaticInfection)
            {
                presymptomaticInfected++;
            }
            else if (person.GetComponent<Infection>().currentState == InfectionType.FullyInfected)
            {
                fullyInfected++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentGameState == GameState.GameStart)
        {
            Scheduler._instance.CreateSchedule(people, rooms);

            StartTheDay();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && currentGameState == GameState.EndOfDay)
        {
            StartTheDay();
        }

        //CanvasUpdate();
    }

    private void CanvasUpdate()
    {
        //txtPreSymptomaticInf.text = "Presymptomatic Infections: " + presymptomaticInfected;
        //txtNotInf.text = "Non-Infections: " + notInfected;
        //txtPotentialInf.text = "Potential Infections: " + potentialInfected;
        //txtAsympInf.text = "Asymptomatic Infections: " + asymptomaticInfected;
        //txtFullyInf.text = "Full Infections: " + fullyInfected;
        //txtPeriod.text = "Period " + (currentPeriod + 1);
        //txtDay.text = "Day " + (currentDay);
    }
    
    void CheckFullRooms()
    {
        int fullRooms = 0;

        foreach (GameObject room in rooms)
        {
            if(room.GetComponent<Room>().roomFull == true)
            {
                fullRooms++;
            }
            if (rooms.Count == fullRooms)
            {
                print("Rooms Full");
                CancelInvoke();
                Invoke("MoveNextPeriod", 3);
            }
        }
    }

    void StartTheDay()
    {
        UIManager._instance.ShowEndOfDayScreen(false);

        prev_notInfected = notInfected;
        prev_asymptomaticInfected = asymptomaticInfected;
        prev_presymptomaticInfected = presymptomaticInfected;
        prev_fullyInfected = fullyInfected;

        currentGameState = GameState.RunningDay;
        currentDay++;
        currentPeriod = -1;
        StartDay?.Invoke();
        MoveNextPeriod();
    }

    void EndTheDay()
    {
        foreach (GameObject person in people)
        {
            var box = outsides.transform.GetChild(UnityEngine.Random.Range(0, outsides.transform.childCount)).GetComponent<BoxCollider>();
            var pos = box.transform.position + new UnityEngine.Vector3(UnityEngine.Random.Range(-box.size.x / 2, box.size.x / 2), 0, UnityEngine.Random.Range(-box.size.z / 2, box.size.z / 2));

            person.GetComponent<NavMeshAgent>().SetDestination(pos);
            print(pos);
        }
        Invoke("ACTIONCallEndDay", 10);
        
    }

    void ACTIONCallEndDay()
    {
        EndDay?.Invoke();

        UIManager._instance.ShowEndOfDayScreen(true);
    }

    void MoveNextPeriod()
    {
        if (currentPeriod < NUMBER_OF_PERIODS - 1)
        {
            currentPeriod++;
            print("Current Period: " + currentPeriod);
            CancelInvoke();
            NextPeriod?.Invoke();
            GoToRooms?.Invoke();
            InvokeRepeating("CheckFullRooms", 0, 1);
        }
        else
        {
            // End day
            currentGameState = GameState.EndOfDay;
            print("End Day");
            EndTheDay();
        } 
    }

    public void Button_StartNextDay()
    {
        if (currentGameState == GameState.EndOfDay)
        {
            StartTheDay();
        }
    }
}
