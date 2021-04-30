using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { GameStart, RunningDay, EndOfDay}

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public GameState currentGameState;

    public GameObject personPrefab;

    public GameObject roomsObject;
    private List<GameObject> rooms = new List<GameObject>();

    public GameObject peoplesObject;
    public List<GameObject> people = new List<GameObject>();

    public bool hasLunch;
    public int lunchPeriod;
    public GameObject lunchRoom;

    public GameObject outsides;

    [HideInInspector]
    public int currentDay;
    [HideInInspector]
    public int currentPeriod = 0;
    [HideInInspector]
    public static int NUMBER_OF_PERIODS = 3;

    public UnityAction StartDay;
    public UnityAction PrepRooms;
    public UnityAction NextPeriod;
    public UnityAction GoToRooms;
    public UnityAction EndDay;

    [HideInInspector]
    public int notInfected;
    [HideInInspector]
    public int potentialInfected;
    [HideInInspector]
    public int asymptomaticInfected;
    [HideInInspector]
    public int presymptomaticInfected;
    [HideInInspector]
    public int fullyInfected;
    [HideInInspector]
    public int sick;

    [HideInInspector]
    public int start_notInfected;
    [HideInInspector]
    public int start_potentialInfected;
    [HideInInspector]
    public int start_asymptomaticInfected;
    [HideInInspector]
    public int start_presymptomaticInfected;
    [HideInInspector]
    public int start_fullyInfected;
    [HideInInspector]
    public int start_sick;
    [HideInInspector]
    public float applicationSpeed = 1;
    [HideInInspector]
    public float currentSpeed = 1;


    private bool dayEnding = false;
    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        Physics.SyncTransforms();

        UIManager._instance.ShowStartSimScreen();       
    }

    private void Initialize()
    {
        currentGameState = GameState.GameStart;
        currentDay = 0;

        if (hasLunch)
        {
            NUMBER_OF_PERIODS++;
            lunchPeriod = NUMBER_OF_PERIODS / 2;
        }

        PopulatePeople(int.Parse(UIManager._instance.input_TotalPeople.text));

        foreach (Transform room in roomsObject.transform)
        {
            rooms.Add(room.gameObject);
        }
        foreach (Transform person in peoplesObject.transform)
        {
            people.Add(person.gameObject);

            // TODO: Change infection class stuff on the person

            person.GetComponent<NavMeshAgent>().radius = (UIManager._instance.input_StartupSocialDistance.isOn ? 1f : 0.5f);
            int stats = UnityEngine.Random.Range(0, 4);
            person.GetComponent<NavMeshAgent>().avoidancePriority += stats;
            person.GetComponent<NavMeshAgent>().speed += stats;

            //calculate how many people are infected/not infected at beginning of game.
            //if (person.GetComponent<Infection>().currentState == InfectionType.NotInfected)
            //{
            //    notInfected++;
            //}
            //else if (person.GetComponent<Infection>().currentState == InfectionType.PotentialInfection)
            //{
            //    potentialInfected++;
            //}
            //else if (person.GetComponent<Infection>().currentState == InfectionType.AsymptomaticInfection)
            //{
            //    asymptomaticInfected++;
            //}
            //else if (person.GetComponent<Infection>().currentState == InfectionType.PreSymptomaticInfection)
            //{
            //    presymptomaticInfected++;
            //}
            //else if (person.GetComponent<Infection>().currentState == InfectionType.FullyInfected)
            //{
            //    fullyInfected++;
            //}
            //else if (person.GetComponent<Infection>().currentState == InfectionType.Sick)
            //{
            //    sick++;
            //}
        }

        for (int i = 0; i < int.Parse(UIManager._instance.input_AsympPeople.text); )
        {
            GameObject person = people[UnityEngine.Random.Range(0, people.Count)];
            if(person.gameObject.GetComponent<Infection>().currentState == InfectionType.NotInfected)
            {
                person.gameObject.GetComponent<Infection>().currentState = InfectionType.AsymptomaticInfection;
                i++;
            }
        }
        for (int i = 0; i < int.Parse(UIManager._instance.input_PresympPeople.text); )
        {
            GameObject person = people[UnityEngine.Random.Range(0, people.Count)];
            if (person.gameObject.GetComponent<Infection>().currentState == InfectionType.NotInfected)
            {
                person.gameObject.GetComponent<Infection>().currentState = InfectionType.PreSymptomaticInfection;
                i++;
                //presymptomaticInfected++;
            }
        }
        for (int i = 0; i < int.Parse(UIManager._instance.input_SympPeople.text); )
        {
            GameObject person = people[UnityEngine.Random.Range(0, people.Count)];
            if (person.gameObject.GetComponent<Infection>().currentState == InfectionType.NotInfected)
            {
                person.gameObject.GetComponent<Infection>().currentState = InfectionType.FullyInfected;
                i++;
                //fullyInfected++;
            }
        }

        Scheduler._instance.CreateSchedule(people, rooms, lunchRoom);

        if (hasLunch)
        {
            rooms.Add(lunchRoom);
        }

        Invoke("StartTheDay", 0.5f);
    }

    void StartTheDay()
    {
        bool masksOn = UIManager._instance.input_NextDayWearMasks.isOn;
        float radius = UIManager._instance.input_NextDaySocialDistance.isOn ? 1f : 0.5f;
        foreach (GameObject person in people)
        {
            // TODO: Change infection class stuff on the person
            person.GetComponent<Infection>().maskOn = masksOn;
            person.GetComponent<NavMeshAgent>().radius = radius;
        }

        UIManager._instance.ShowEndOfDayScreen(false);

        start_notInfected = notInfected;
        start_asymptomaticInfected = asymptomaticInfected;
        start_presymptomaticInfected = presymptomaticInfected;
        start_fullyInfected = fullyInfected;
        start_sick = sick;
        //currentGameState = GameState.RunningDay;
        currentDay++;
        currentPeriod = -1;
        StartDay?.Invoke();
        MoveNextPeriod();
    }

    void EndTheDay()
    {
        foreach (GameObject person in people)
        {
            NavMeshPath newPath = new NavMeshPath();

            var box = outsides.transform.GetChild(UnityEngine.Random.Range(0, outsides.transform.childCount)).GetComponent<BoxCollider>();
            var pos = box.transform.position + new UnityEngine.Vector3(UnityEngine.Random.Range(-box.size.x / 2, box.size.x / 2), 0, UnityEngine.Random.Range(-box.size.z / 2, box.size.z / 2));

            person.GetComponent<NavMeshAgent>().SetDestination(pos);

            bool ret = person.GetComponent<NavMeshAgent>().CalculatePath(pos, newPath);
            person.GetComponent<NavMeshAgent>().SetPath(newPath);

            if (!ret)
                person.GetComponent<NavMeshAgent>().SetDestination(pos);
        }
        dayEnding = true;
        InvokeRepeating("CallEndDay", 0,2);
    }

    void CallEndDay()
    {
        if (people.Any(o => o.GetComponent<Person>().onSchoolPremesis != false))
            return;

        if (dayEnding == true)
        {
            EndDay?.Invoke();
            UIManager._instance.ShowEndOfDayScreen(true);
            dayEnding = false;
        }
    }

    void MoveNextPeriod()
    {
        if (currentPeriod < NUMBER_OF_PERIODS - 1)
        {
            currentPeriod++;
            CancelInvoke();
            NextPeriod?.Invoke();
            GoToRooms?.Invoke();
            InvokeRepeating("CheckFullRooms", 0, 1);
        }
        else
        {
            // End day
            currentGameState = GameState.EndOfDay;
            EndTheDay();
        } 
    }

    private void PopulatePeople(int amount)
    {

        int i = 0;
        int peoplePerBox = Mathf.CeilToInt(((float)amount / 4f));
        int total = 0;
        BoxCollider[] colliders = outsides.transform.GetComponentsInChildren<BoxCollider>();
        
        for (int j = 0; j < colliders.Length; j++)
        {
            UnityEngine.Vector3 currentBoxPos = colliders[j].transform.position;
            
            while (i < peoplePerBox && total < amount)
            {
                if(j % 2 == 0)
                {
                    Instantiate(personPrefab, currentBoxPos + new UnityEngine.Vector3((i) - 100f, 0, 0), UnityEngine.Quaternion.identity, peoplesObject.transform);
                    i++;
                    total++;
                }
                else
                {
                    Instantiate(personPrefab, currentBoxPos + new UnityEngine.Vector3(0, 0, (i) - 100f), UnityEngine.Quaternion.identity, peoplesObject.transform);
                    i++;
                    total++;
                }
                
            }
            i = 0;
        }
        
    }

    void CheckFullRooms()
    {
        int fullRooms = 0;

        foreach (GameObject room in rooms)
        {
            if (room.GetComponent<Room>().roomFull == true)
            {
                fullRooms++;
            }

            if (rooms.Count == fullRooms && lunchRoom.GetComponent<Room>().roomFull == true)
            {
                CancelInvoke();
                Invoke("MoveNextPeriod", 3);
            }
        }
    }

    #region Button Functions
    public void Button_StartNextDay()
    {
        if (currentGameState == GameState.EndOfDay)
        {
            StartTheDay();
        }
    }

    public void Button_ChangeTimeScale(float speed)
    {
        //Send in 0 if speed is is normal
        //Send in 1 if speed wants to be increased.
        //x1, x1.5, x2 x3

        if(speed == -1){
            speed = 0;
        }
        else if(speed == 0){
            speed = 1;
        }
        else if(speed >= 1){
            speed = (float)Math.Ceiling(Time.timeScale + 1); 
        }
        
        if(currentSpeed == 2){
            speed = 4;
        }


        switch(speed){
            case 0:
                currentSpeed = 0;
                break;
            case 1:
                currentSpeed = 1;
                break;
            case 2:
                currentSpeed = 1.5f;
                break;
            case 3:
                currentSpeed = 2;
                break;
            case 4:
                currentSpeed = 3;
                break;
            default:
              currentSpeed = 1;
              break;
        }
        
        Time.timeScale = currentSpeed;
    }

    public void Button_StartSim()
    {
        try
        {
            NUMBER_OF_PERIODS = Convert.ToInt32(UIManager._instance.input_Periods.text.Replace("\u200B", ""));
            UIManager._instance.StartScreen.SetActive(false);

            UIManager._instance.input_NextDayWearMasks.isOn = UIManager._instance.input_StartupWearMasks.isOn;
            UIManager._instance.input_NextDaySocialDistance.isOn = UIManager._instance.input_StartupSocialDistance.isOn;

            Initialize();
        }
        catch (Exception ex)
        {

        }
    }
    public void Button_RestartSim()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}
