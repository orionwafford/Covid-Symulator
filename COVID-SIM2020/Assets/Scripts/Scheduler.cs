using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    public static Scheduler _instance;//yo

    [SerializeField]
    private List<GameObject> localPeople = new List<GameObject>();

    void Awake()
    {
        _instance = this;
    }

    public void CreateSchedule(List<GameObject> people, List<GameObject> rooms,GameObject lunchRoom = null)
    {
        int peopleCount = people.Count;
        int roomCount = rooms.Count;
        int peoplePerRoom = peoplePerRoom = Mathf.FloorToInt(peopleCount / roomCount);
        int leftOverPeople;

        localPeople = new List<GameObject>(people);

        for (int periodIndex = 0; periodIndex < (GameManager._instance.hasLunch ? GameManager.NUMBER_OF_PERIODS : GameManager.NUMBER_OF_PERIODS + 1); periodIndex++)
        {
            if (GameManager._instance.hasLunch && periodIndex == GameManager._instance.lunchPeriod)
            {
                for (int peopleIndex = 0; peopleIndex < peopleCount; peopleIndex++)
                {
                    localPeople[peopleIndex].GetComponent<Schedule>().SetScheduleRoom(lunchRoom, periodIndex);
                }
            }
            else
            {
                if (peoplePerRoom != 0)
                {
                    leftOverPeople = peopleCount % roomCount;
                }
                else
                {
                    leftOverPeople = peopleCount;
                }
                int totalCount = 0;

                ShufflePeople(localPeople);

                for (int roomIndex = 0; roomIndex < roomCount; roomIndex++)
                {

                    for (int peopleIndex = 0; peopleIndex < peoplePerRoom; peopleIndex++)
                    {
                        localPeople[totalCount].GetComponent<Schedule>().SetScheduleRoom(rooms[roomIndex], periodIndex);
                        totalCount++;
                    }

                    if (leftOverPeople > 0)
                    {
                        localPeople[totalCount].GetComponent<Schedule>().SetScheduleRoom(rooms[roomIndex], periodIndex);
                        totalCount++;
                        leftOverPeople--;
                    }
                }
            }
        }
    }

    private void ShufflePeople(List<GameObject> list)
    {
        System.Random rng = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            GameObject value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
