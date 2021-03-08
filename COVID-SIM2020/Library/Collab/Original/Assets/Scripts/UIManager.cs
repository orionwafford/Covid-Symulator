using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    public TextMeshProUGUI txtNotInf;
    public TextMeshProUGUI txtPotentialInf;
    public TextMeshProUGUI txtAsympInf;
    public TextMeshProUGUI txtFullyInf;
    public TextMeshProUGUI txtPreSymptomaticInf;
    public TextMeshProUGUI txtPeriod;
    public TextMeshProUGUI txtDay;

    public TextMeshProUGUI txtPrevNotInf;
    public TextMeshProUGUI txtPrevPotentialInf;
    public TextMeshProUGUI txtPrevAsympInf;
    public TextMeshProUGUI txtPrevPreSymptomaticInf;
    public TextMeshProUGUI txtPrevFullyInf;

    public TextMeshProUGUI txtTodayNotInf;
    public TextMeshProUGUI txtTodayPotentialInf;
    public TextMeshProUGUI txtTodayAsympInf;
    public TextMeshProUGUI txtTodayPreSymptomaticInf;
    public TextMeshProUGUI txtTodayFullyInf;

    public Material blurMaterial;

    public GameObject runningDayUI;

    public GameObject endOfDayScreen;

    public GameObject blur;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        ShowEndOfDayScreen(false);
    }

    void OnGUI()
    {
        txtPreSymptomaticInf.text = "Presymptomatic Infections: " + GameManager._instance.presymptomaticInfected;
        txtNotInf.text = "Non-Infections: " + GameManager._instance.notInfected;
        txtPotentialInf.text = "Potential Infections: " + GameManager._instance.potentialInfected;
        txtAsympInf.text = "Asymptomatic Infections: " + GameManager._instance.asymptomaticInfected;
        txtFullyInf.text = "Full Infections: " + GameManager._instance.fullyInfected;
        txtPeriod.text = "Period " + (GameManager._instance.currentPeriod + 1);
        txtDay.text = "Day " + GameManager._instance.currentDay;

        txtPrevNotInf.text = "Non-Infected: " + GameManager._instance.prev_notInfected;
        //txtPrevPotentialInf.text = "Potential Infected:" + GameManager._instance.prev_potentialInfected;
        txtPrevAsympInf.text = "Asymptomatic Infections: " + GameManager._instance.prev_asymptomaticInfected;
        txtPrevPreSymptomaticInf.text = "Presymptomatic Infections: " + GameManager._instance.prev_presymptomaticInfected;
        txtPrevFullyInf.text = "Full Infections: " + GameManager._instance.prev_fullyInfected;

        txtTodayNotInf.text = "Non-Infected: " + GameManager._instance.notInfected;
        //txtTodayPotentialInf.text = "Potential Infected:" + GameManager._instance.potentialInfected;
        txtTodayAsympInf.text = "Asymptomatic Infections: " + GameManager._instance.asymptomaticInfected;
        txtTodayPreSymptomaticInf.text = "Presymptomatic Infections: " + GameManager._instance.presymptomaticInfected;
        txtTodayFullyInf.text = "Full Infections: " + GameManager._instance.fullyInfected;
    }

    public void ShowEndOfDayScreen(bool value)
    {
        endOfDayScreen.SetActive(value);
        runningDayUI.SetActive(!value);
        if (value)
        {
            blurMaterial.SetFloat("_Size", 1);
        }
        else
        {
            blurMaterial.SetFloat("_Size", 0);
        }
    }
    public void ShowStartSimScreen()
    {

    }
}
