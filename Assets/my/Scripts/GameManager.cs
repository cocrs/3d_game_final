using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameObject parkingLotUsing;
    public GameObject[] buildings;
    public GameObject startPos;
    public GameObject goal;
    public GameObject goalIcon;
    public GameObject WayPoint;

    [Header("Timer")]
    public GameObject timerMenu;
    public Text timerText;
    public float timer;
    [Header("Game State")]
    public static bool playing = true;
    public static bool finishParking = false;

    [Header("Game Text")]
    public GameObject failTXT;
    public GameObject successMenu;
    public Text successTXT;
    public GameObject scorePanel;
    public Text scoreTXT;
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        scoreTXT.text = "Score: " + score;
        parkingLotUsing = null;
        buildings = GameObject.FindGameObjectsWithTag("Building");
        SetRandomGoal();
    }

    // Update is called once per frame
    void Update()
    {
        if (playing == true)
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
                playing = false;
                StopPlyerControll();
            }
        }
        else
        {
            if (finishParking)
            {
                // turn off some UI
                timerMenu.SetActive(false);
                scorePanel.SetActive(false);

                // show seccess menu
                successMenu.SetActive(true);
                int disTmp = (int)(GameObject.Find("Car").transform.position - goal.transform.position).magnitude;
                successTXT.text = "Score: " + score + "\nTime Left: " + timerText.text + "\nDistacne: " + disTmp.ToString();
            }
            else
            {
                failTXT.SetActive(true);
            }
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

    // void ColorChangerr()
    // {
    //     float t;
    //     Color ori = scoreTXT.color;
    //     scoreTXT.color = Color.Lerp(Color.red, Color.red, t);

    //     if (t < 1){ 
    //         t += Time.deltaTime/duration;
    //     }
    // }

    public static void StopPlyerControll()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = true;
    }
}
