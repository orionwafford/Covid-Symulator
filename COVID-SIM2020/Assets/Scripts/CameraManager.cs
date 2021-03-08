using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    LayerMask myMask;

    Vector3 basePosOffest = new Vector3(0, 116, -6);
    Vector3 baseRotOffest = new Vector3(90, 0, 0);

    Vector3 zoomCap = new Vector2(50, 116);

    Vector3 personPosOffest = new Vector3(3, 25, 0);
    Vector3 personRotOffest = new Vector3(90, 0, 0);

    bool isFocused;

    void Start()
    {
        myMask = LayerMask.GetMask("People");
        isFocused = false;
    }

    void Update()
    {
        if (!isFocused)
        {
            Vector3 pos = Camera.main.transform.position;
            pos.y -= Input.GetAxis("Mouse ScrollWheel") * 45;
            pos.y = Mathf.Clamp(pos.y, zoomCap.x, zoomCap.y);

            pos.x = Input.GetAxis("Horizontal") * 15;
            pos.z = Input.GetAxis("Vertical") * 15;

            Camera.main.transform.position = pos;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, myMask))
            {
                FocusOn(hit.collider.gameObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetFocus();
        }
    }

    void FocusOn(GameObject target)
    {
        Camera.main.transform.parent = target.transform;
        Camera.main.transform.localPosition = personPosOffest;
        Camera.main.transform.localRotation = Quaternion.Euler(personRotOffest);

        isFocused = true;
    }

    void ResetFocus()
    {
        Camera.main.transform.parent = null;
        Camera.main.transform.localPosition = basePosOffest;
        Camera.main.transform.eulerAngles = baseRotOffest;

        isFocused = false;
    }
}
