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
    private GameObject goalIconObj;
    public GameObject wayPointGoal;
    public GameObject waypoints;
    public GameObject parkingManagers;
    public GameObject player;
    // private GameObject wayPointArrow;
    public GameObject questTester;
    public GameObject homeParkingLot;
    public Transform gateFrontPose;
    public GameObject checkMark;
    public Cinemachine.CinemachineVirtualCamera cinemachine;

    [Header("Timer")]
    // public GameObject timerMenu;
    public Text timerText;
    public float timer;
    [Header("Game State")]
    public static bool playing = true;
    public static bool inQuest = false;
    public static bool finishParking = false;
    public static bool questFinished = false;
    public static bool gameFinished = false;
    public static float limitDistance;
    public Animation lightAnime;
    public Animation stretch1;
    public Animation stretch2;
    public GameObject getMoney;

    [Header("Game Windows")]
    public QUI_Window failWindow;
    public QUI_Window successWindow;
    public QUI_Window questWindow;
    public QUI_Window pauseWindow;
    public QUI_Window endDayWindow;
    public QUI_Window questConfirmWindow;
    public QUI_Window questDetailWindow;

    [Header("MainUI")]
    public TextMeshProUGUI DayTXT;
    private int curDay;
    public GameObject EnergyNotEnoughTXT;
    public TextMeshProUGUI moneyTXT;
    public GameObject questBtn;
    public GameObject pauseBtn;

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

    [Header("Quest")]
    private int chosedTown;
    public Dictionary<string, dynamic>[] goalList;

    [Header("Shop")]
    public Image[] itemPos;
    public Sprite[] itemSprites;
    private int curShowItemIndex;
    public int playerDollars;
    public Dictionary<string, dynamic>[] items;


    void Awake()
    {
        items = new Dictionary<string, dynamic>[]{
            new Dictionary<string, dynamic>(){
                {"name", "Map"},
                {"discription", "You can see the whole view using this map."},
                {"price", 2000},
                {"belong", "shop"}
            },
            new Dictionary<string, dynamic>(){
                {"name", "Bomb"},
                {"discription", "Attack the cars that block in front of you!"},
                {"price", 500},
                {"belong", "shop"}
            },
            new Dictionary<string, dynamic>(){
                {"name", "Hourglass"},
                {"discription", "Looks like something can affect #?!*&"},
                {"price", 1000},
                {"belong", "shop"}
            },
            new Dictionary<string, dynamic>(){
                {"name", "Magical Cake"},
                {"discription", "A magical cake. You don't know what will happen if you eat it."},
                {"price", 1000},
                {"belong", "shop"}
            },
        };

        playerDollars = 10000;
        curShowItemIndex = 0;
    }
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
        EnergyNotEnoughTXT.SetActive(false);
        endDayWindow.SetActive(false);
        getMoney.SetActive(false);

        // waypoints.SetActive(false);
        RandomDisactiveWaypoints();

        limitDistance = 0;
        playing = true;

        moneyTXT.text = "$ " + playerDollars;
        scoreTXT.text = "Score: " + score;
        parkingLotUsing = null;
        Instantiate(goalIcon, new Vector3(0, 0, 0), Quaternion.identity);
        goalIconObj = GameObject.FindWithTag("GoalIcon");
        goalIconObj.SetActive(false);

        curDay = 1;
        DayTXT.text = "Day " + curDay;
        // buildings = GameObject.FindGameObjectsWithTag("Building");
        goalTown1 = GameObject.FindGameObjectsWithTag("GoalTown1");
        goalTown2 = GameObject.FindGameObjectsWithTag("GoalTown2");
        setRandomGoalThisRound();
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
                if (goalDis <= limitDistance)
                {
                    checkMark.SetActive(true);
                }
                else
                {
                    checkMark.SetActive(false);
                }
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
                else if (questFinished)
                {
                    // turn off some UI
                    parkingManagers.SetActive(false);
                    waypoints.SetActive(true);
                    RandomDisactiveWaypoints();
                    // PlayerControll(false);
                    homeParkingLot.GetComponent<ParkingTrigger>().ResetTireTrigger();
                    homeParkingLot.SetActive(true);
                    goalIconObj.SetActive(false);

                    setRandomGoalThisRound();

                    if (finishParking)
                    {
                        // show success menu
                        // successWindow.SetActive(true);
                        // successTXT.text = "Reward: " + goalList[chosedTown]["reward"] + "\nTime Left: " + timerText.text + "\nDistacne: " + goalDis.ToString();
                        StartCoroutine(addMoney());
                        parkingLotUsing.GetComponent<MParkingManager>().ResetTireTrigger();
                        parkingLotUsing = null;
                    }
                    else {
                        questUI.SetActive(false);
                        questDetailWindow.SetActive(false);
                    }
                    inQuest = false;
                }
            }
        }
    }
    IEnumerator addMoney()
    {
        stretch1.Play();
        yield return new WaitForSeconds(1);
        getMoney.SetActive(true);
        getMoney.GetComponent<TextMeshProUGUI>().text = "+" + goalList[chosedTown]["reward"];
        getMoney.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(1);
        getMoney.SetActive(false);
        playerDollars += goalList[chosedTown]["reward"];
        updatePlayerDollars();
        if ((int)(goalDis / 5) > 0)
        {
            stretch2.Play();
            if ((int)(goalDis / 5) > questTester.GetComponent<HealthTester>().curHealth)
            {
                questTester.GetComponent<HealthTester>().consumeEnergy(questTester.GetComponent<HealthTester>().curHealth);
            }
            else
            {
                questTester.GetComponent<HealthTester>().consumeEnergy((int)(goalDis / 5));
            }
            yield return new WaitForSeconds(1);
        }
        questUI.SetActive(false);
        questDetailWindow.SetActive(false);
    }
    public void SetPlayingState(bool state)
    {
        playing = state;
    }
    public void SetGameFinishedState(bool state)
    {
        gameFinished = state;
        cinemachine.Follow = null;
    }
    public void playerGetItem(int itemIndex)
    {
        itemPos[curShowItemIndex].sprite = itemSprites[itemIndex];
        itemPos[curShowItemIndex].color = new Color(255, 255, 255, 255);
        curShowItemIndex++;
    }
    public void updatePlayerDollars()
    {
        moneyTXT.text = "$ " + playerDollars;
    }
    public void acceptQuest()
    {
        chosedTown = Random.Range(0, 2);
        // if (questTester.GetComponent<HealthTester>().consumeEnergy(goalList[chosedTown]["consume"])) {
        questWindow.SetActive(false);
        questConfirmWindow.SetActive(false);
        waypoints.SetActive(false);
        setGoal();
        // } else {
        //     EnergyNotEnoughTXT.SetActive(true);
        //     EnergyNotEnoughTXT.GetComponent<Animation>().Stop();
        //     EnergyNotEnoughTXT.GetComponent<Animation>().Play();
        // }
    }
    public void RandomDisactiveWaypoints()
    {
        int count = 0;
        foreach (Transform child in waypoints.transform)
        {
            int state = Random.Range(1, 11);
            if (state <= 2)
            {
                count++;
                child.gameObject.SetActive(false);
            }
            if (count == 3)
            {
                break;
            }
        }
    }
    public void toNextDay()
    {
        HealthTester.recoverEnergy = true;
        StartCoroutine(addDate());
    }
    IEnumerator addDate()
    {
        PlayerControll(false);
        lightAnime.Play();
        yield return new WaitForSeconds(3);
        curDay += 1;
        playerDollars -= 500;
        updatePlayerDollars();
        DayTXT.text = "Day " + curDay;
        PlayerControll(true);
        OpenMainUIButton();
    }
    public void setChosedTown(int choice)
    {
        chosedTown = choice;
    }
    public void openConfirmWindow()
    {
        TextMeshProUGUI headerText = questConfirmWindow.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI infoText = questConfirmWindow.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
        headerText.text = goalList[chosedTown]["name"];
        infoText.text = "Reward: " + goalList[chosedTown]["reward"] + "\n";
        infoText.text += "Time limit: " + goalList[chosedTown]["time"] + " secs" + "\n";
        infoText.text += "Distance limit: " + goalList[chosedTown]["distance"] + " m" + "\n";
        infoText.text += "Consume energy: " + goalList[chosedTown]["consume"];
        questConfirmWindow.SetActive(true);
    }
    public void adjustScore(int change)
    {
        if (score + change >= 0)
        {
            score += change;
            scoreTXT.text = "Score: " + score;
        }
    }

    public void changeInQuestState()
    {
        inQuest = !inQuest;

    }
    void setRandomGoalThisRound()
    {
        GameObject goal1 = goalTown1[Random.Range(0, goalTown1.Length)];
        GameObject goal2 = goalTown2[Random.Range(0, goalTown2.Length)];
        float goal1Dis = Vector3.Distance(player.transform.position, goal1.transform.position);
        float goal2Dis = Vector3.Distance(player.transform.position, goal2.transform.position);
        int reward = Random.Range(100, 501);
        int distance = Random.Range(100, 151);

        goalList = new Dictionary<string, dynamic>[]{
            new Dictionary<string, dynamic>(){
                {"name", "town1"},
                {"goal", goal1},
                {"reward", reward},
                {"time", Mathf.Max((int)goal1Dis / 3, 30)},
                {"consume", 50},
                {"distance", distance}
            },
            new Dictionary<string, dynamic>(){
                {"name", "town2"},
                {"goal", goal2},
                {"reward", reward},
                {"time", Mathf.Max((int)goal2Dis / 3, 30)},
                {"consume", 80},
                {"distance", distance}
            }
        };
    }
    public void setGoal()
    {
        goal = goalList[chosedTown]["goal"];
        timer = goalList[chosedTown]["time"];
        limitDistance = goalList[chosedTown]["distance"];

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
        goalIconObj.SetActive(true);
        goalIconObj.transform.position = new Vector3(center.x, 30f, center.z);
        wayPointGoal.transform.position = center;


        inQuest = true;
        questFinished = false;
        finishParking = false;
        parkingLotUsing = null;
        // wayPointArrow.SetActive(true);
        PlayerControll(true);
        questUI.SetActive(true);
        questDetailWindow.SetActive(true);
        questDetailWindow.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Limit Distance: " + limitDistance;
        questDetailWindow.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Reward: " + goalList[chosedTown]["reward"];
        parkingManagers.SetActive(true);
        homeParkingLot.SetActive(false);

        // timer reset
        timerText.color = Color.black;
        MParkingManager.endTime = Time.time + 4;
    }

    public void CloseMainUI()
    {
        questBtn.SetActive(false);
        pauseBtn.SetActive(false);
        questWindow.SetActive(false);
        pauseWindow.SetActive(false);
        questConfirmWindow.SetActive(false);
    }
    public void OpenMainUIButton()
    {
        questBtn.SetActive(true);
        pauseBtn.SetActive(true);
    }
    public static bool isTooFarFromGoal()
    {
        return goalDis > limitDistance;
    }
    public void SetPlayerPosition()
    {
        player.transform.position = gateFrontPose.transform.position;
        player.transform.rotation = gateFrontPose.transform.rotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    public void PlayerControll(bool state)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = !state;
    }
}
