using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public enum InfectionType
{
    NotInfected,
    PotentialInfection,
    AsymptomaticInfection,
    PreSymptomaticInfection,
    FullyInfected,
    Sick
}

public class Infection : MonoBehaviour
{
    public GameManager manager;

    // Start is called before the first frame update
    public InfectionType currentState = InfectionType.NotInfected;

    private IEnumerator myRoutine;

    public Material mat_NotINF;
    public Material mat_potentialINF;
    public Material mat_asymptomaticINF;
    public Material mat_presymptomaticINF;
    public Material mat_fullyINF;
    public Material mat_sick;

    public float timeUntilInfection = 2.0f;

    SphereCollider obj_distancing;

    // Whenever player walks outside of screen, they can no longer infect people they are next to.
    public bool canInfect = false;
    private int daySinceInfection;
    private int daysUntilFullInfection;
    private int daysUntilCured;
    public bool maskOn = false;

    void Start()
    {
        manager = GameManager._instance;

        obj_distancing = GetComponent<SphereCollider>();

        daySinceInfection = 0;
        daysUntilFullInfection = 0;

        GameManager._instance.EndDay += EndDayResult;

        if (currentState == InfectionType.AsymptomaticInfection)
        {
            daysUntilCured = Random.Range(1, 7);
        }
        else if(currentState == InfectionType.PreSymptomaticInfection)
        {
            daysUntilFullInfection = Random.Range(1, 12);
        }

        CheckInfection();
    }

    void CheckInfection()
    {
        if (currentState== InfectionType.NotInfected)
        {
            this.GetComponent<Renderer>().material = mat_NotINF;
            manager.notInfected++;
        }
        else if(currentState == InfectionType.PotentialInfection)
        {
            this.GetComponent<Renderer>().material = mat_potentialINF;
            manager.potentialInfected++;
        }
        else if (currentState == InfectionType.AsymptomaticInfection)
        {
            this.GetComponent<Renderer>().material = mat_asymptomaticINF;
            manager.asymptomaticInfected++;
        }
        else if (currentState == InfectionType.PreSymptomaticInfection)
        {
            this.GetComponent<Renderer>().material = mat_presymptomaticINF;
            manager.presymptomaticInfected++;
        }
        else if (currentState == InfectionType.FullyInfected)
        {
            this.GetComponent<Renderer>().material = mat_fullyINF;
            manager.fullyInfected++;
        }
    }

    void GetInfected(GameObject other)
    {
        Infection otherInfection = other.gameObject.GetComponent<Infection>();

        if (otherInfection != null && other.GetType() != typeof(SphereCollider) && otherInfection.canInfect == true && currentState == InfectionType.NotInfected)
        {
            //If other person is infected AND you are not infected
            if (otherInfection.currentState != InfectionType.NotInfected)
            {
                currentState = InfectionType.PotentialInfection;
                //manager.potentialInfected++;
                manager.notInfected--;

                CheckInfection();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "SCHOOLPREMESIS")
        {
            canInfect = true;
        }
        else if (other.gameObject.GetComponent<Infection>() != null && other.gameObject.GetComponent<Infection>().currentState != InfectionType.NotInfected)
        {
            myRoutine = CheckForInfectionWait(other.gameObject);
            StartCoroutine(myRoutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.name == "SCHOOLPREMESIS")
        {
            canInfect = false;
        }
    }

    IEnumerator CheckForInfectionWait(GameObject otherObject)
    {
        yield return new WaitForSeconds(timeUntilInfection);

        if (Vector3.Distance(this.transform.position, otherObject.transform.position) < 1)
        {
            GetInfected(otherObject.gameObject);
        }
    }

    public void CalculatePotentials()
    {
        if(currentState == InfectionType.PotentialInfection)
        {
            int RNG = Random.Range(0, 100);
            
            //50% chance they turn infected, 50% they are not infected.
            int ceilingRNG = 50;

            //35% chance to spread with mask.
            if (maskOn == true)
            {
                ceilingRNG = 35;
            }
            if (RNG < ceilingRNG)
            {
                int RNG2 = Random.Range(0, 100);
                //40% chance they turn asymptomatic
                ceilingRNG = 40;

                if(RNG2 < ceilingRNG)
                {
                    daysUntilCured = Random.Range(1, 7);
                    currentState = InfectionType.AsymptomaticInfection;
                    manager.potentialInfected--;
                    manager.asymptomaticInfected++;
                }
                //60% to be fully infected within 7 days.
                else
                {
                    daysUntilFullInfection = Random.Range(1, 12);
                    currentState = InfectionType.PreSymptomaticInfection;
                    manager.potentialInfected--;
                    manager.presymptomaticInfected++;
                }
            }
            else
            {
                currentState = InfectionType.NotInfected;
                manager.potentialInfected--;
                manager.notInfected++;
            }
        }
    }

    public void EndDayResult()
    {
        //canInfect = false;
        CalculatePotentials();

        if (currentState == InfectionType.PreSymptomaticInfection)
        {
            daySinceInfection++;
            if(daySinceInfection >= daysUntilFullInfection)
            {
                currentState = InfectionType.FullyInfected;
                manager.fullyInfected++;
                manager.presymptomaticInfected--;
            }
        }
        else if(currentState == InfectionType.AsymptomaticInfection)
        {
            daySinceInfection++;
            if (daySinceInfection >= daysUntilCured)
            {
                currentState = InfectionType.NotInfected;
                manager.notInfected++;
                manager.asymptomaticInfected--;
            }
        }
        else if(currentState == InfectionType.FullyInfected)
        {
            currentState = InfectionType.Sick;

            manager.sick++;
            manager.fullyInfected--;
            GameManager._instance.people.Remove(this.gameObject);
            
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        GameManager._instance.EndDay -= EndDayResult;
    }
}
