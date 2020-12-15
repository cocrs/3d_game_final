using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QuantumTek.QuantumUI;
using TMPro;

public class GameManager : MonoBehaviour {
    public static GameObject parkingLotUsing;
    public GameObject[] buildings;
    public GameObject[] goalTown1;
    public GameObject[] goalTown2;
    public GameObject[] goalCity;
    public GameObject startPos;
    public GameObject goal;
    public GameObject goalIcon;
    private GameObject goalIconObj;
    public GameObject WayPoint;
    public GameObject parkingManagers;
    public GameObject player;
    // private GameObject wayPointArrow;
    public GameObject questTester;
    public GameObject homeParkingLot;

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
    public QUI_Window pauseWindow;
    public QUI_Window endDayWindow;
    public QUI_Window questConfirmWindow;

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
    

    [Header("Shop")]
    public static Dictionary<string, dynamic>[] shopItems;
    public Dictionary<string, dynamic>[] items;
    public Image[] itemPos;
    public Sprite[] itemSprites;
    private int curShowItemIndex;
    public int playerDollars;


    public Dictionary<string, dynamic>[] goalList;
    private int chosedTown;


    void Awake() {
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

        playerDollars = -1000;
        curShowItemIndex = 0;
    }
    // Start is called before the first frame update
    void Start() {
        // wayPointArrow = GameObject.Find("Waypoint Arrow");
        failWindow.SetActive(false);
        questWindow.SetActive(false);
        successWindow.SetActive(false);
        questUI.SetActive(false);
        timerCountMenu.SetActive(false);
        parkingManagers.SetActive(false);
        tooFarTXT.SetActive(false);
        // wayPointArrow.SetActive(false);
        EnergyNotEnoughTXT.SetActive(false);
        endDayWindow.SetActive(false);

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
    void Update() {
        if (playing) {
            if (inQuest) {
                goalDis = (int)(player.transform.position - goal.transform.position).magnitude;
                goalDistanceTXT.text = "Goal Distance: " + goalDis.ToString("00") + " m";
                if (!questFinished && parkingLotUsing == null) {
                    // timer
                    timer -= Time.deltaTime;
                    int minutes = Mathf.FloorToInt(timer / 60f);
                    int seconds = Mathf.FloorToInt(timer % 60f);
                    // int milliseconds = Mathf.FloorToInt((timer * 100f) % 100f);
                    if (minutes == 0 && seconds <= 10) {
                        timerText.color = Color.red;
                    }
                    if (minutes != 0 || seconds != 0) {
                        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
                    } else {
                        timerText.text = "00:00";
                        questFinished = true;
                    }
                } else if (questFinished) {
                    // turn off some UI
                    questUI.SetActive(false);
                    parkingManagers.SetActive(false);
                    // wayPointArrow.SetActive(false);
                    PlayerControll(false);
                    homeParkingLot.GetComponent<ParkingTrigger>().ResetTireTrigger();
                    homeParkingLot.SetActive(true);
                    setRandomGoalThisRound();

                    if (finishParking) {
                        // show success menu
                        successWindow.SetActive(true);
                        successTXT.text = "Reward: " + goalList[chosedTown]["reward"] + "\nTime Left: " + timerText.text + "\nDistacne: " + goalDis.ToString();
                        playerDollars += goalList[chosedTown]["reward"];
                        updatePlayerDollars();
                        parkingLotUsing.GetComponent<MParkingManager>().ResetTireTrigger();
                        parkingLotUsing = null;
                    } else {
                        failWindow.SetActive(true);
                    }
                    inQuest = false;
                }
            }
        }
    }

    public void playerGetItem(int itemIndex) {
        itemPos[curShowItemIndex].sprite = itemSprites[itemIndex];
        itemPos[curShowItemIndex].color = new Color(255, 255, 255, 255);
        curShowItemIndex++;
    }
    public void updatePlayerDollars() {
        moneyTXT.text = "$ " + playerDollars;
    }
    public void acceptQuest() {
        if (questTester.GetComponent<HealthTester>().consumeEnergy(goalList[chosedTown]["consume"])) {
            questWindow.SetActive(false);
            questConfirmWindow.SetActive(false);
            setGoal();
        } else {
            EnergyNotEnoughTXT.SetActive(true);
            EnergyNotEnoughTXT.GetComponent<Animation>().Stop();
            EnergyNotEnoughTXT.GetComponent<Animation>().Play();
        }
    }
    public void toNextDay() {
        HealthTester.recoverEnergy = true;
        StartCoroutine(addDate());
    }
    IEnumerator addDate() {
        PlayerControll(false);
        lightAnime.Play();
        yield return new WaitForSeconds(3);
        curDay += 1;
        DayTXT.text = "Day " + curDay;
        PlayerControll(true);
        questBtn.SetActive(true);
        pauseBtn.SetActive(true);
    }
    public void setChosedTown(int choice) {
        chosedTown = choice;
    }
    public void openConfirmWindow() {
        TextMeshProUGUI headerText = questConfirmWindow.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI infoText = questConfirmWindow.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
        headerText.text = goalList[chosedTown]["name"];
        infoText.text = "Reward: " + goalList[chosedTown]["reward"] + "\n";
        infoText.text += "Time limit: " + goalList[chosedTown]["time"] + " secs" + "\n";
        infoText.text += "Distance limit: " + goalList[chosedTown]["distance"] + " m" + "\n";
        infoText.text += "Consume energy: " + goalList[chosedTown]["consume"];
        questConfirmWindow.SetActive(true);
    }
    public void adjustScore(int change) {
        if (score + change >= 0) {
            score += change;
            scoreTXT.text = "Score: " + score;
        }
    }

    public void changeInQuestState() {
        inQuest = !inQuest;

    }
    void setRandomGoalThisRound() {
        GameObject goal1 = goalTown1[Random.Range(0, goalTown1.Length)];
        GameObject goal2 = goalTown2[Random.Range(0, goalTown2.Length)];

        goalList = new Dictionary<string, dynamic>[]{
            new Dictionary<string, dynamic>(){
                {"name", "town1"},
                {"goal", goal1},
                {"reward", 1000},
                {"time", 300},
                {"consume", 50},
                {"distance", 150}
            },
            new Dictionary<string, dynamic>(){
                {"name", "town2"},
                {"goal", goal2},
                {"reward", 1500},
                {"time", 200},
                {"consume", 80},
                {"distance", 100}
            }
        };
    }
    public void setGoal() {
        goal = goalList[chosedTown]["goal"];
        timer = goalList[chosedTown]["time"];
        limitDistance = goalList[chosedTown]["distance"];

        // else if(townName == "city"){
        //     goal = goalTown1[Random.Range(0, goalTown1.Length)];
        // }

        Vector3 center;
        if (goal.transform.childCount != 0) {
            center = goal.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.center;
        } else {
            center = goal.transform.GetComponent<MeshRenderer>().bounds.center;
        }
        goalIconObj.SetActive(true);
        goalIconObj.transform.position = new Vector3(center.x, 30f, center.z);
        WayPoint.transform.position = center;


        inQuest = true;
        questFinished = false;
        finishParking = false;
        parkingLotUsing = null;
        // wayPointArrow.SetActive(true);
        PlayerControll(true);
        questUI.SetActive(true);
        parkingManagers.SetActive(true);
        homeParkingLot.SetActive(false);

        // timer reset
        timerText.color = Color.black;
        MParkingManager.endTime = Time.time + 4;
    }

    public void CloseMainUI(){
        questBtn.SetActive(false);
        pauseBtn.SetActive(false);
        questWindow.SetActive(false);
        pauseWindow.SetActive(false);
        questConfirmWindow.SetActive(false);
    }
    public static bool isTooFarFromGoal() {
        return goalDis > limitDistance;
    }

    public void PlayerControll(bool state) {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = !state;
    }
}
