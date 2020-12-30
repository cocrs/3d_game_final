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
    public GameObject minusReward;
    public GameObject minusEnergy;

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
    public GameObject[] foodList;
    Queue<GameObject> foodInScene;
    int[] foodPrice;
    List<int> indices;
    public Transform spawnPointFood;
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
        foodPrice = new int[3];
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

        playerDollars = -2000;
        curShowItemIndex = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        failWindow.SetActive(false);
        questWindow.SetActive(false);
        successWindow.SetActive(false);
        questUI.SetActive(false);
        questDetailWindow.SetActive(false);
        timerCountMenu.SetActive(false);
        parkingManagers.SetActive(false);
        tooFarTXT.SetActive(false);
        EnergyNotEnoughTXT.SetActive(false);
        endDayWindow.SetActive(false);
        getMoney.SetActive(false);
        minusEnergy.SetActive(false);
        minusReward.SetActive(false);

        indices = new List<int>();
        foodInScene = new Queue<GameObject>();

        // waypoints.SetActive(false);
        RandomDisactiveWaypoints();

        limitDistance = 0;
        playing = true;

        moneyTXT.text = "$ " + playerDollars;
        scoreTXT.text = "得分: " + score;
        parkingLotUsing = null;
        Instantiate(goalIcon, new Vector3(0, 0, 0), Quaternion.identity);
        goalIconObj = GameObject.FindWithTag("GoalIcon");
        goalIconObj.SetActive(false);

        curDay = 1;
        DayTXT.text = "第 " + curDay + " 天";
        // buildings = GameObject.FindGameObjectsWithTag("Building");
        goalTown1 = GameObject.FindGameObjectsWithTag("GoalTown1");
        goalTown2 = GameObject.FindGameObjectsWithTag("GoalTown2");
        // goalCity = GameObject.FindGameObjectsWithTag("GoalCity");
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
                goalDistanceTXT.text = "距離目標 " + goalDis.ToString("00") + " 公尺";
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
                    else
                    {
                        StartCoroutine(closeQuestUI());
                    }
                    inQuest = false;
                }
            }
        }
    }
    IEnumerator addMoney()
    {
        stretch1.Play();
        yield return new WaitForSeconds(stretch1["stretch"].length);
        getMoney.SetActive(true);
        getMoney.GetComponent<TextMeshProUGUI>().text = "+$" + goalList[chosedTown]["reward"];
        getMoney.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(getMoney.GetComponent<Animation>()["money"].length);
        getMoney.SetActive(false);
        playerDollars += goalList[chosedTown]["reward"];
        updatePlayerDollars();
        if ((int)(goalDis / 5) > 0)
        {
            stretch2.Play();
            float energy;
            if ((int)(goalDis / 5) > questTester.GetComponent<HealthTester>().curHealth)
            {
                energy = questTester.GetComponent<HealthTester>().curHealth;
            }
            else
            {
                energy = (int)(goalDis / 5);
            }
            yield return new WaitForSeconds(stretch2["stretch"].length);
            questTester.GetComponent<HealthTester>().consumeEnergy(energy);
            minusEnergy.SetActive(true);
            minusEnergy.GetComponent<TextMeshProUGUI>().text = "-" + (int)energy;
            minusEnergy.GetComponent<Animation>().Play();
            yield return new WaitForSeconds(minusEnergy.GetComponent<Animation>()["minusEnergy"].length);
            minusEnergy.SetActive(false);
        }
        StartCoroutine(closeQuestUI());
    }
    IEnumerator closeQuestUI()
    {
        questUI.transform.GetComponent<Animation>()["questIn"].speed = -1;
        questUI.transform.GetComponent<Animation>()["questIn"].time = questUI.transform.GetComponent<Animation>()["questIn"].length;
        questUI.transform.GetComponent<Animation>().Play();
        questDetailWindow.SetActive(false);
        while (foodInScene.Count > 0)
        {
            Destroy(foodInScene.Dequeue());
        }
        yield return new WaitForSeconds(questUI.transform.GetComponent<Animation>()["questIn"].length);
        questUI.SetActive(false);
    }
    public IEnumerator MinusRewardPlay(int price){
        minusReward.SetActive(true);
        minusReward.GetComponent<TextMeshProUGUI>().text = "-$" + price;
        minusReward.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(minusReward.GetComponent<Animation>()["minusReward"].length);
        minusReward.SetActive(false);
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
        yield return new WaitForSeconds(lightAnime["change day"].length);
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
        infoText.text = "報酬: " + goalList[chosedTown]["reward"] + "\n";
        infoText.text += "時間限制: " + goalList[chosedTown]["time"] + " secs" + "\n";
        infoText.text += "距離限制: " + goalList[chosedTown]["distance"] + " m" + "\n";
        infoText.text += "消耗體力: " + goalList[chosedTown]["consume"];
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
        int distance = Random.Range(100, 151);

        indices.Clear();
        while (indices.Count < 3)
        {
            int index = Random.Range(0, foodList.Length);
            if (!indices.Contains(index))
            {
                indices.Add(index);
            }
        }
        
        

        goalList = new Dictionary<string, dynamic>[]{
            new Dictionary<string, dynamic>(){
                {"name", "town1"},
                {"goal", goal1},
                {"reward", 0},
                {"time", Mathf.Max((int)goal1Dis / 3, 30)},
                {"distance", distance}
            },
            new Dictionary<string, dynamic>(){
                {"name", "town2"},
                {"goal", goal2},
                {"reward", 0},
                {"time", Mathf.Max((int)goal2Dis / 3, 30)},
                {"distance", distance}
            },
            // new Dictionary<string, dynamic>(){
            //     {"name", "city"},
            //     {"goal", city},
            //     {"reward", 0},
            //     {"time", Mathf.Max((int)goal2Dis / 3, 30)},
            //     {"distance", distance}
            // }
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

        StartCoroutine(spawnFood());

        inQuest = true;
        questFinished = false;
        finishParking = false;
        parkingLotUsing = null;
        // wayPointArrow.SetActive(true);
        PlayerControll(true);
        parkingManagers.SetActive(true);
        homeParkingLot.SetActive(false);

        // timer reset
        timerText.color = Color.black;
        MParkingManager.endTime = Time.time + 4;
    }

    IEnumerator spawnFood()
    {
        print(this.transform.position);
        foreach (int index in indices)
        {
            GameObject spawnedFood = Instantiate(foodList[index], spawnPointFood.position, Quaternion.identity);
            spawnedFood.AddComponent<DropFood>();
            foodInScene.Enqueue(spawnedFood);
            for(int i = 0; i < goalList.Length; i++){
                print(spawnedFood.GetComponent<DropFood>().price);
                goalList[i]["reward"] += spawnedFood.GetComponent<DropFood>().price;
            }
            yield return new WaitForSeconds(0.5f);
        }
        questUI.SetActive(true);
        questDetailWindow.SetActive(true);
        questDetailWindow.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Limit Distance: " + limitDistance;
        questDetailWindow.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "報酬: " + goalList[chosedTown]["reward"];
        questUI.transform.GetComponent<Animation>()["questIn"].speed = 1;
        questUI.transform.GetComponent<Animation>().Play();
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
