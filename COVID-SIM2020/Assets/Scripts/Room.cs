using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    private BoxCollider roomCollider;
    
    public List<GameObject> peopleInThisRoom = new List<GameObject>();

    public bool roomFull = false;

    public int currentPeopleInRoom = 0;

    LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        roomCollider = GetComponent<BoxCollider>();

        mask = LayerMask.GetMask("People");

        GameManager._instance.GoToRooms += CalculatePositions;
    }

    public void CalculatePositions()
    {
        Vector3 positionsStartPos = transform.position + (roomCollider.center) - new Vector3(roomCollider.size.x / 2, 0, -roomCollider.size.z / 2);

        int rowsAndColumns = Mathf.CeilToInt(Mathf.Sqrt(peopleInThisRoom.Count));

        float xDist = roomCollider.size.x / (rowsAndColumns - 1);
        float zDist = roomCollider.size.z / (rowsAndColumns - 1);

        int total = 0;
        for (int rowIndex = 0; rowIndex < rowsAndColumns; rowIndex++)
        {
            for (int colIndex = 0; colIndex < rowsAndColumns; colIndex++)
            {
                if (total >= peopleInThisRoom.Count)
                    break;

                NavMeshPath newPath = new NavMeshPath();

                bool ret;

                if (rowsAndColumns == 1)
                {
                    ret = peopleInThisRoom[total].GetComponent<NavMeshAgent>().CalculatePath(transform.position, newPath);
                    peopleInThisRoom[total].GetComponent<NavMeshAgent>().SetPath(newPath);

                    if (!ret) 
                        peopleInThisRoom[total].GetComponent<NavMeshAgent>().SetDestination(transform.position);
                }
                else
                {
                    ret = peopleInThisRoom[total].GetComponent<NavMeshAgent>().CalculatePath(positionsStartPos + new Vector3(colIndex * xDist, 0, rowIndex * -zDist), newPath);
                    peopleInThisRoom[total].GetComponent<NavMeshAgent>().SetPath(newPath);

                    if (!ret)
                        peopleInThisRoom[total].GetComponent<NavMeshAgent>().SetDestination(positionsStartPos + new Vector3(colIndex * xDist, 0, rowIndex * -zDist));
                }

                total++;
            }
        }

        roomFull = false;
        InvokeRepeating("CheckRoomFull", 0, 1);
    }

    private void CheckRoomFull()
    {
        currentPeopleInRoom = 0;

        Collider[] col = Physics.OverlapBox(transform.position, (roomCollider.size / 1.5f), Quaternion.identity, mask);

        foreach (Collider collider in col)
        {
            if (peopleInThisRoom.Contains(collider.gameObject) && collider is CapsuleCollider)
            {
                currentPeopleInRoom++;
            }
        }

        if (currentPeopleInRoom >= peopleInThisRoom.Count)
        {
            roomFull = true;
            peopleInThisRoom.Clear();
            CancelInvoke();
        }
    }
}
