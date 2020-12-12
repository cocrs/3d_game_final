using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QuantumTek.QuantumUI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameObject parkingLotUsing;
    public GameObject[] buildings;
    public GameObject[] goalTown1;
    public GameObject[] goalTown2;
    public GameObject[] goalCity;
    public GameObject startPos;
    public GameObject goal;
    public GameObject goalIcon;
    public GameObject WayPoint;
    public GameObject parkingManagers;
    public GameObject player;
    public GameObject wayPointArrow;
    public GameObject questTester;

    [Header("Timer")]
    // public GameObject timerMenu;
    public Text timerText;
    public float timer;
    [Header("Game State")]
    public static bool playing = false;
    public static bool inQuest = false;
    public static bool finishParking = false;
    public static bool questFinished = false;
    public static float limitDistance;
    public Animation lightAnime;

    [Header("Game Windows")]
    public QUI_Window failWindow;
    public QUI_Window successWindow;
    public QUI_Window questWindow;
    public QUI_Window endDayWindow;

    [Header("MainUI")]
    public TextMeshProUGUI DayTXT;
    private int curDay; 
    public GameObject EnergyNotEnoughTXT;

    [Header("QuestUI")]
    public Text successTXT;
    // public GameObject scorePanel;
    public Text scoreTXT;
    public int score;
    public GameObject timerCountMenu;
    public GameObject questUI;
    public TextMeshProUGUI goalDistanceTXT;
    private static float goalDis;
    public GameObject tooFarTXT;


    // Start is called before the first frame update
    void Start()
    {
        failWindow.SetActive(false);
        questWindow.SetActive(false);
        successWindow.SetActive(false);
        questUI.SetActive(false);
        timerCountMenu.SetActive(false);
        parkingManagers.SetActive(false);
        tooFarTXT.SetActive(false);
        wayPointArrow.SetActive(false);
        EnergyNotEnoughTXT.SetActive(false);
        endDayWindow.SetActive(false);

        limitDistance = 0;
        playing = true;

        scoreTXT.text = "Score: " + score;
        parkingLotUsing = null;
        Instantiate(goalIcon, new Vector3(0, 0, 0), Quaternion.identity);
        goalIcon.SetActive(false);

        curDay = 1;
        DayTXT.text = "Day " + curDay;
        // buildings = GameObject.FindGameObjectsWithTag("Building");
        goalTown1 = GameObject.FindGameObjectsWithTag("GoalTown1");
        goalTown2 = GameObject.FindGameObjectsWithTag("GoalTown2");
        // SetRandomGoal();
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            if (inQuest)
            {
                goalDis = (int)(player.transform.position - goal.transform.position).magnitude;
                goalDistanceTXT.text = "Goal Distance: " + goalDis.ToString("00") + " m";
                if (!questFinished && parkingLotUsing == null)
                {
                    // timer
                    timer -= Time.deltaTime;
                    int minutes = Mathf.FloorToInt(timer / 60f);
                    int seconds = Mathf.FloorToInt(timer % 60f);
                    // int milliseconds = Mathf.FloorToInt((timer * 100f) % 100f);
                    if (minutes == 0 && seconds <= 10)
                    {
                        timerText.color = Color.red;
                    }
                    if (minutes != 0 || seconds != 0)
                    {
                        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
                    }
                    else
                    {
                        timerText.text = "00:00";
                        questFinished = true;
                    }
                }
                else if(questFinished)
                {
                    // turn off some UI
                    questUI.SetActive(false);
                    parkingManagers.SetActive(false);
                    wayPointArrow.SetActive(false);
                    PlayerControll(false);

                    if (finishParking)
                    {
                        // show success menu
                        successWindow.SetActive(true);
                        successTXT.text = "Score: " + score + "\nTime Left: " + timerText.text + "\nDistacne: " + goalDis.ToString();
                        
                    }
                    else
                    {
                        failWindow.SetActive(true); 
                    }
                    inQuest = false;
                }
            }
        }
    }

    public void acceptQuest(string args){
        string[] subs = args.Split(',');
        int amount = System.Int32.Parse(subs[0]);
        string townName = subs[1];
        if(questTester.GetComponent<HealthTester>().consumeEnergy(amount)){
            questWindow.SetActive(false);
            setGoal(townName);
        }
        else{
            EnergyNotEnoughTXT.SetActive(true);
            EnergyNotEnoughTXT.GetComponent<Animation>().Stop();
            EnergyNotEnoughTXT.GetComponent<Animation>().Play();
        }
    }
    public void toNextDay(){
        StartCoroutine(addDate());
    }
    IEnumerator addDate()
    {
        PlayerControll(false);
        lightAnime.Play();
        yield return new WaitForSeconds(3);
        curDay += 1;
        DayTXT.text = "Day " + curDay;
        PlayerControll(true);
    }

    public void adjustScore(int change)
    {
        if (score + change >= 0)
        {
            score += change;
            scoreTXT.text = "Score: " + score;
        }
    }

    public void changeInQuestState(){
        inQuest = !inQuest;

    }
    void SetRandomGoal()
    {
        // random
        // goal = buildings[Random.Range(0, buildings.Length)];

        // test
        goal = buildings[1];

        Vector3 center = goal.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.center;
        Instantiate(goalIcon, new Vector3(center.x, 30f, center.z), Quaternion.identity);
        WayPoint.transform.position = center;
    }
    public void setGoal(string townName)
    {
        if (townName == "town1")
        {
            goal = goalTown1[Random.Range(0, goalTown1.Length)];
        }
        else if (townName == "town2")
        {
            goal = goalTown2[Random.Range(0, goalTown2.Length)];
        }
        // else if(townName == "city"){
        //     goal = goalTown1[Random.Range(0, goalTown1.Length)];
        // }

        Vector3 center;
        if (goal.transform.childCount != 0)
        {
            center = goal.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.center;
        }
        else
        {
            center = goal.transform.GetComponent<MeshRenderer>().bounds.center;
        }
        goalIcon.SetActive(true);
        goalIcon.transform.position = new Vector3(center.x, 30f, center.z);
        WayPoint.transform.position = center;

        limitDistance = 100;
        inQuest = true;
        questFinished = false;
        finishParking = false;
        wayPointArrow.SetActive(true);
        PlayerControll(true);
        questUI.SetActive(true);
        parkingManagers.SetActive(true);

        // timer reset
        timerText.color = Color.black;
        timer = 20;
        MParkingManager.endTime = Time.time + 4;
    }
    // void ColorChangerr()
    // {
    //     float t;
    //     Color ori = scoreTXT.color;
    //     scoreTXT.color = Color.Lerp(Color.red, Color.red, t);

    //     if (t < 1){ 
    //         t += Time.deltaTime/duration;
    //     }
    // }
    public static bool isTooFarFromGoal(){
        return goalDis > limitDistance;
    }

    public void PlayerControll(bool state)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = !state;
    }
}
