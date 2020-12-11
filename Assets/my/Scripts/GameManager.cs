using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QuantumTek.QuantumUI;

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

    [Header("Timer")]
    public GameObject timerMenu;
    public Text timerText;
    public float timer;
    [Header("Game State")]
    public static bool playing = false;
    public static bool inQuest = false;
    public static bool finishParking = false;

    [Header("Game Text")]
    public GameObject failTXT;
    public QUI_Window successWindow;
    public QUI_Window questWindow;
    public Text successTXT;
    public GameObject scorePanel;
    public Text scoreTXT;
    public int score;


    // Start is called before the first frame update
    void Start()
    {
        questWindow.SetActive(false);
        successWindow.SetActive(false);
        timerMenu.SetActive(false);
        scorePanel.SetActive(false);

        playing = true;

        scoreTXT.text = "Score: " + score;
        parkingLotUsing = null;
        // buildings = GameObject.FindGameObjectsWithTag("Building");
        goalTown1 = GameObject.FindGameObjectsWithTag("GoalTown1");
        goalTown2 = GameObject.FindGameObjectsWithTag("GoalTown2");
        // SetRandomGoal();
    }

    // Update is called once per frame
    void Update()
    {
        if (playing && inQuest == true)
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
                inQuest = false;
                failTXT.SetActive(true);
            }
        }
        else if(playing){
            
        }
        else
        {
            PlayerControll(false);
            if (finishParking)
            {
                // turn off some UI
                timerMenu.SetActive(false);
                scorePanel.SetActive(false);

                // show seccess menu
                successWindow.SetActive(true);
                int disTmp = (int)(GameObject.FindWithTag("Player").transform.position - goal.transform.position).magnitude;
                successTXT.text = "Score: " + score + "\nTime Left: " + timerText.text + "\nDistacne: " + disTmp.ToString();
            }
            // else
            // {
            //     failTXT.SetActive(true);
            // }
        }
    }

    public void adjustScore(int change)
    {
        if (score + change >= 0)
        {
            score += change;
            scoreTXT.text = "Score: " + score;
        }
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
    public void setGoal(string townName){
        if(townName == "town1"){
            goal = goalTown1[Random.Range(0, goalTown1.Length)];
        }
        else if(townName == "town2"){
            goal = goalTown2[Random.Range(0, goalTown2.Length)];
        }
        // else if(townName == "city"){
        //     goal = goalTown1[Random.Range(0, goalTown1.Length)];
        // }

        Vector3 center;
        if(goal.transform.childCount != 0){
            center = goal.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.center;
        }
        else{
            center = goal.transform.GetComponent<MeshRenderer>().bounds.center;
        }
        Instantiate(goalIcon, new Vector3(center.x, 30f, center.z), Quaternion.identity);
        WayPoint.transform.position = center;

        inQuest = true;
        PlayerControll(true);
        timerMenu.SetActive(true);
        scorePanel.SetActive(true);
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

    public void PlayerControll(bool state)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = !state;
    }
}
