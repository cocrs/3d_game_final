using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QuantumTek.QuantumUI;
using TMPro;
using cakeslice;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameObject parkingLotUsing;
    public GameObject[] buildings;
    public GameObject[] goalTown1;
    public GameObject[] goalTown2;
    public GameObject[] goalCity;
    private GameObject goal;
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
    public GameObject Camera;
    public GameObject startCamera;
    public GameObject cameraObj;
    public GameObject moneyBar;
    public GameObject mainMenu;
    public GameObject spawnPoints;
    public GameObject tutorial;
    private bool first;
    public SEController seController;

    [Header("Timer")]
    // public GameObject timerMenu;
    public Text timerText;
    public float timer;
    [Header("Game State")]
    public static bool playing = false;
    public static bool inQuest = false;
    public static bool finishParking = false;
    public static bool questFinished = false;
    public static bool gameFinished = false;
    public static bool changeDay = false;
    public static float limitDistance;
    public Animation lightAnime;
    public Animation stretch1;
    public Animation stretch2;
    public GameObject fail;
    public GameObject getMoney;
    public GameObject minusReward;
    public GameObject minusMoney;
    public GameObject minusEnergy;
    private float lastUpdateTime;

    [Header("Statistics")]
    public GameObject statisticsWindow;
    public GameObject[] stars;
    private int acceptQuestCount;
    private int questSuccessCount;
    private int dropFoodCount;

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
    public GameObject notifyText;
    public GameObject map;
    private bool mapActive = false;

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
    public GameObject successEffect;
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
    private int initialMoney;
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
        fail.SetActive(false);
        Camera.GetComponent<OutlineEffect>().enabled = false;
        cameraObj.SetActive(false);
        waypoints.SetActive(false);
        spawnPoints.SetActive(false);
        moneyBar.SetActive(false);
        questTester.SetActive(false);
        DayTXT.gameObject.SetActive(false);
        player.SetActive(false);

        indices = new List<int>();
        foodInScene = new Queue<GameObject>();
        initialMoney = playerDollars;

        // waypoints.SetActive(false);
        RandomDisactiveWaypoints();

        limitDistance = 0;

        questSuccessCount = 0;
        dropFoodCount = 0;
        acceptQuestCount = 0;

        updatePlayerDollars();
        scoreTXT.text = "得分: " + score;
        parkingLotUsing = null;
        Instantiate(goalIcon, new Vector3(0, 0, 0), Quaternion.identity);
        goalIconObj = GameObject.FindWithTag("GoalIcon");
        goalIconObj.SetActive(false);

        curDay = 1;
        DayTXT.text = "剩餘 " + curDay + " 天";
        // buildings = GameObject.FindGameObjectsWithTag("Building");
        goalTown1 = GameObject.FindGameObjectsWithTag("GoalTown1");
        goalTown2 = GameObject.FindGameObjectsWithTag("GoalTown2");
        goalCity = GameObject.FindGameObjectsWithTag("GoalCity");
        setRandomGoalThisRound();
        // SetRandomGoal();
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            if(Input.GetKeyDown(KeyCode.M)){
                mapActive = !mapActive;
                map.SetActive(mapActive);
            }
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
                if (goalList[chosedTown]["reward"] == 0 || questTester.GetComponent<HealthTester>().curHealth <= 0)
                {
                    questFinished = true;
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
                    Camera.GetComponent<OutlineEffect>().enabled = false;

                    if (finishParking)
                    {
                        StartCoroutine(addMoney());
                        parkingLotUsing.GetComponent<MParkingManager>().ResetTireTrigger();
                        parkingLotUsing = null;
                        questSuccessCount += 1;
                    }
                    else
                    {
                        fail.SetActive(true);
                        fail.GetComponent<Animation>().Stop();
                        fail.GetComponent<Animation>().Play();
                        StartCoroutine(MinusMoneyPlay(50));
                        seController.playClip(3);
                        StartCoroutine(closeQuestUI());
                        setRandomGoalThisRound();
                        if (questTester.GetComponent<HealthTester>().curHealth <= 0)
                        {
                            StartCoroutine(SendBackToHome());
                        }
                    }
                    inQuest = false;
                }
            }
            else{
                if(Time.time - lastUpdateTime > 15){
                    RandomDisactiveWaypoints();
                }
            }
            if (questTester.GetComponent<HealthTester>().curHealth <= 0)
            {
                if(first){
                    StartCoroutine(SendBackToHome());
                    first = false;
                }
            }
            else{
                first = true;
            }
        }
    }
    public void ShowStatisticsWindow()
    {
        print(1);
        int spendDays = 7 - curDay;
        float moneyEarn = playerDollars - initialMoney;
        TextMeshProUGUI tmp = statisticsWindow.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        tmp.text = spendDays + "\n" + moneyEarn + "\n" + acceptQuestCount + "\n" + questSuccessCount + "\n" + dropFoodCount + "\n";
        print(2);
        float successRate = (float)questSuccessCount / acceptQuestCount;
        print(successRate);
        float scores = ((float)curDay / 7f) + ((moneyEarn - (dropFoodCount * 20f)) / 4000f);
        print("scores: " + scores);
        if(scores > 2){
            stars[2].SetActive(true);
        }
        if(scores > 1.7f){
            stars[1].SetActive(true);
        }
        if(moneyEarn > 1000){
            stars[0].SetActive(true);
        }

        statisticsWindow.SetActive(true);
    }
    public void LoadScene(){
        SceneManager.LoadScene("Game");
    }
    public void StartGame()
    {
        tutorial.SetActive(false);
        mainMenu.SetActive(false);
        cameraObj.SetActive(true);
        startCamera.SetActive(false);
        waypoints.SetActive(true);
        spawnPoints.SetActive(true);
        player.SetActive(true);
        moneyBar.SetActive(true);
        questTester.SetActive(true);
        DayTXT.gameObject.SetActive(true);
        GameManager.playing = true;
    }
    IEnumerator addMoney()
    {
        Instantiate(successEffect, spawnPointFood.position - new Vector3(0, 0.5f, 0), Quaternion.identity);
        seController.playClip(4);
        seController.playClip(5);
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
        setRandomGoalThisRound();
    }
    IEnumerator closeQuestUI()
    {
        questUI.transform.GetComponent<Animation>()["questIn"].speed = -1;
        questUI.transform.GetComponent<Animation>()["questIn"].time = questUI.transform.GetComponent<Animation>()["questIn"].length;
        questUI.transform.GetComponent<Animation>().Play();
        questDetailWindow.SetActive(false);
        if (playing)
        {
            while (foodInScene.Count > 0)
            {
                Destroy(foodInScene.Dequeue());
            }
        }
        yield return new WaitForSeconds(questUI.transform.GetComponent<Animation>()["questIn"].length);
        questUI.SetActive(false);
    }
    public void OpenQuestUIForTalk()
    {
        questUI.SetActive(true);
        questDetailWindow.SetActive(true);
        questUI.transform.GetComponent<Animation>()["questIn"].speed = 1;
        questUI.transform.GetComponent<Animation>().Play();
    }
    public void CloseQuestUIForTalk()
    {
        StartCoroutine(closeQuestUI());
    }
    public void UpdateRewardText()
    {
        questDetailWindow.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "報酬: " + goalList[chosedTown]["reward"];
    }
    public IEnumerator MinusRewardPlay(int price)
    {
        seController.playClip(Random.Range(0, 3));
        dropFoodCount += 1;
        GameObject spawnedText = Instantiate(minusReward);
        spawnedText.transform.SetParent(notifyText.transform);
        spawnedText.GetComponent<TextMeshProUGUI>().text = "-$" + price;
        spawnedText.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(spawnedText.GetComponent<Animation>()["minusReward"].length);
        goalList[chosedTown]["reward"] -= price;
        UpdateRewardText();
        Destroy(spawnedText);
    }
    public IEnumerator MinusMoneyPlay(int price)
    {
        GameObject spawnedText = Instantiate(minusMoney);
        spawnedText.transform.SetParent(notifyText.transform);
        spawnedText.GetComponent<TextMeshProUGUI>().text = "-$" + price;
        spawnedText.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(spawnedText.GetComponent<Animation>()["minusMoney"].length);
        playerDollars -= price;
        updatePlayerDollars();
        Destroy(spawnedText);
    }
    public void SetPlayingState(bool state)
    {
        playing = state;
    }
    public void CloseUIForTalk()
    {
        moneyBar.SetActive(false);
        questTester.SetActive(false);
        DayTXT.gameObject.SetActive(false);
        if(inQuest){
            StartCoroutine(closeQuestUI());
        }
    }
    public void OpenUIForTalk()
    {
        moneyBar.SetActive(true);
        questTester.SetActive(true);
        DayTXT.gameObject.SetActive(true);
        if(inQuest){
            OpenQuestUIForTalk();
        }
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
        if(playerDollars < 0){
            moneyTXT.color = Color.red;
        }
        else{
            moneyTXT.color = Color.green;
        }
        moneyTXT.text = "$ " + playerDollars;
    }
    public void acceptQuest()
    {
        seController.playClip(6);
        seController.playClip(7);
        chosedTown = Random.Range(0, 3);
        // if (questTester.GetComponent<HealthTester>().consumeEnergy(goalList[chosedTown]["consume"])) {
        // questWindow.SetActive(false);
        // questConfirmWindow.SetActive(false);
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
        foreach (Transform child in waypoints.transform)
        {
            child.gameObject.SetActive(true);
        }
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
        lastUpdateTime = Time.time;
    }
    public void toNextDay()
    {
        HealthTester.recoverEnergy = true;
        StartCoroutine(addDate());
    }
    IEnumerator addDate()
    {
        changeDay = true;
        PlayerControll(false);
        lightAnime.Play();
        yield return new WaitForSeconds(lightAnime["change day"].length);
        curDay -= 1;
        StartCoroutine(MinusMoneyPlay(500));
        DayTXT.text = "剩餘 " + curDay + " 天";
        PlayerControll(true);
        if(curDay == 0){
            PlayerControll(false);
            gameFinished = true;
            Color newColor;
            ColorUtility.TryParseHtmlString("#D42121", out newColor);
            statisticsWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = newColor;
            statisticsWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "脫離失敗";
            statisticsWindow.SetActive(true);
        }
        changeDay = false;
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
        GameObject goal3 = goalCity[Random.Range(0, goalCity.Length)];
        float goal1Dis = Vector3.Distance(player.transform.position, goal1.transform.position);
        float goal2Dis = Vector3.Distance(player.transform.position, goal2.transform.position);
        float goal3Dis = Vector3.Distance(player.transform.position, goal3.transform.position);
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
            new Dictionary<string, dynamic>(){
                {"name", "city"},
                {"goal", goal3},
                {"reward", 0},
                {"time", Mathf.Max((int)goal2Dis / 3, 30)},
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
        Camera.GetComponent<OutlineEffect>().enabled = true;

        StartCoroutine(spawnFood());

        inQuest = true;
        questFinished = false;
        finishParking = false;
        parkingLotUsing = null;
        // wayPointArrow.SetActive(true);
        PlayerControll(true);
        parkingManagers.SetActive(true);
        homeParkingLot.SetActive(false);
        acceptQuestCount += 1;

        // timer reset
        timerText.color = Color.black;
        MParkingManager.endTime = Time.time + 4;
    }

    IEnumerator spawnFood()
    {
        foreach (int index in indices)
        {
            GameObject spawnedFood = Instantiate(foodList[index], spawnPointFood.position, Quaternion.identity);
            spawnedFood.AddComponent<DropFood>();
            spawnedFood.AddComponent<cakeslice.Outline>();
            spawnedFood.GetComponent<cakeslice.Outline>().color = 1;
            foodInScene.Enqueue(spawnedFood);
            // print("hi" + spawnedFood.GetComponent<DropFood>().price);
            goalList[chosedTown]["reward"] += spawnedFood.GetComponent<DropFood>().price;
            yield return new WaitForSeconds(0.5f);
        }
        questUI.SetActive(true);
        questDetailWindow.SetActive(true);
        questDetailWindow.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "距離目標 " + limitDistance + " 公尺以內";
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
        player.GetComponent<Rigidbody>().isKinematic = !state;
    }
    public IEnumerator SendBackToHome()
    {
        yield return new WaitForSeconds(2f);
        PlayerControll(false);
        player.transform.position = homeParkingLot.transform.position;
        player.transform.rotation = Quaternion.identity;
    }
}
