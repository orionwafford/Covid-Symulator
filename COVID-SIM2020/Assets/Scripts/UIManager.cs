using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    public TextMeshProUGUI txtNotInf;
    public TextMeshProUGUI txtPotentialInf;
    public TextMeshProUGUI txtAsympInf;
    public TextMeshProUGUI txtFullyInf;
    public TextMeshProUGUI txtPreSymptomaticInf;
    public TextMeshProUGUI txtSick;

    public TextMeshProUGUI txtPeriod;
    public TextMeshProUGUI txtDay;

    public TextMeshProUGUI txtPrevNotInf;
    public TextMeshProUGUI txtPrevPotentialInf;
    public TextMeshProUGUI txtPrevAsympInf;
    public TextMeshProUGUI txtPrevPreSymptomaticInf;
    public TextMeshProUGUI txtPrevFullyInf;
    public TextMeshProUGUI txPrevSick;

    public Material blurMaterial;

    public GameObject runningDayUI;

    public GameObject endOfDayScreen;

    public GameObject StartScreen;

    public TMP_InputField input_Periods;
    public TMP_InputField input_TotalPeople;
    public TMP_InputField input_AsympPeople;
    public TMP_InputField input_PresympPeople;
    public TMP_InputField input_SympPeople;

    public Toggle input_StartupWearMasks;
    public Toggle input_StartupSocialDistance;

    public Toggle input_NextDayWearMasks;
    public Toggle input_NextDaySocialDistance;


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
        txtPreSymptomaticInf.text = GameManager._instance.presymptomaticInfected.ToString();
        txtNotInf.text = GameManager._instance.notInfected.ToString();
        txtPotentialInf.text = GameManager._instance.potentialInfected.ToString();
        txtAsympInf.text = GameManager._instance.asymptomaticInfected.ToString();
        txtFullyInf.text = GameManager._instance.fullyInfected.ToString();
        txtSick.text = GameManager._instance.sick.ToString();

        txtPeriod.text = "Period " + (GameManager._instance.currentPeriod + 1);
        txtDay.text = "Day " + GameManager._instance.currentDay;

        string symbol = "";
        int infectionDif = (GameManager._instance.start_notInfected - GameManager._instance.notInfected);
        if (GameManager._instance.start_notInfected > GameManager._instance.notInfected)
            symbol = "-";
        txtPrevNotInf.text = symbol + infectionDif;

        infectionDif = (GameManager._instance.asymptomaticInfected - GameManager._instance.start_asymptomaticInfected);
        if (GameManager._instance.start_asymptomaticInfected < GameManager._instance.asymptomaticInfected)
            symbol = "+";
        txtPrevAsympInf.text = symbol + infectionDif;

        infectionDif = (GameManager._instance.presymptomaticInfected - GameManager._instance.start_presymptomaticInfected);
        if (GameManager._instance.start_presymptomaticInfected > GameManager._instance.presymptomaticInfected)
            symbol = "+";
        txtPrevPreSymptomaticInf.text = symbol + infectionDif;

        infectionDif = (GameManager._instance.fullyInfected - GameManager._instance.start_fullyInfected);
        if (GameManager._instance.start_fullyInfected < GameManager._instance.fullyInfected)
        {
            symbol = "+";
            Mathf.Abs(infectionDif);
        }
        txtPrevFullyInf.text = symbol + infectionDif;

        infectionDif = (GameManager._instance.sick - GameManager._instance.start_sick);
        txPrevSick.text = symbol + infectionDif;
    }

    public void ShowEndOfDayScreen(bool value)
    {
        endOfDayScreen.SetActive(value);
        //runningDayUI.SetActive(!value);
    }
    public bool ShowStartSimScreen()
    {
        StartScreen.SetActive(true);

        return false;
    }
}
