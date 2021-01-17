using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MParkingManager : MonoBehaviour {
    public bool t1, t2, t3, t0;
    public GameObject TimerCountMen;
    public GameObject tooFarTXT;
    public MeshRenderer ParkRenderer;
    public Text CountDownText;
    private bool isFinish, FinisheD, canLoadinnn = true;
    public bool timeLimit;
    public static float endTime;
    private Color myYellow, myRed, myGreen;

    void Awake() {
        TimerCountMen = GameObject.Find("TimerCount");
        CountDownText = GameObject.Find("TimerCountTXT").GetComponent<Text>();
        tooFarTXT = GameObject.Find("TooFarTXT");
        myGreen = Color.green;
        myGreen.a = 0.5f;
        myRed = Color.red;
        myRed.a = 0.5f;
        myYellow = Color.yellow;
        myYellow.a = 0.5f;
    }
    // Start is called before the first frame update
    IEnumerator Start() {
        //This is parking timer
        endTime = Time.time + 4;

        // Start count down from 3 to 0
        CountDownText.text = "3";

        yield return new WaitForSeconds(.03f);
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.inQuest) {
            if (!FinisheD) {

                if (t0 && t2 && t3 && t1) {
                    if (GameManager.isTooFarFromGoal()) {
                        tooFarTXT.SetActive(true);
                        ParkRenderer.materials[0].SetColor("_Color", myRed);
                        GameManager.parkingLotUsing = this.gameObject;
                    } else {
                        StartCoroutine(CheckTimeToFinisheD());
                        GameManager.parkingLotUsing = this.gameObject;
                        ParkRenderer.materials[0].SetColor("_Color", myGreen);

                        if (canLoadinnn) {
                            TimerCountMen.SetActive(true);
                            CountDownText.gameObject.SetActive(true);
                        }

                        int timeLeft = (int)(endTime - Time.time);
                        if (timeLeft <= 0) {
                            timeLeft = 0;
                            isFinish = true;
                        }

                        CountDownText.text = timeLeft.ToString();
                    }
                } else if (GameManager.parkingLotUsing == this.gameObject) {
                    StopCoroutine(CheckTimeToFinisheD());
                    tooFarTXT.SetActive(false);
                    GameManager.parkingLotUsing = null;

                } else if (GameManager.parkingLotUsing == null) {
                    isFinish = false;
                    TimerCountMen.SetActive(false);

                    endTime = Time.time + 4;

                    CountDownText.text = "3";

                    ParkRenderer.materials[0].color = myYellow;
                }

            }
        }
    }
    IEnumerator CheckTimeToFinisheD() {
        yield return new WaitForSeconds(4f);
        if (isFinish == true) {
            FinisheD = true;

            TimerCountMen.SetActive(false);

            CountDownText.gameObject.SetActive(false);

            GameManager.finishParking = true;
            GameManager.questFinished = true;
            ParkRenderer.materials[0].SetColor("_Color", myYellow);
            isFinish = false;
            FinisheD = false;
            t0 = false;
            t1 = false;
            t2 = false;
            t3 = false;
            CountDownText.text = "3";
        }
    }
    public void ResetTireTrigger() {
        t0 = false;
        t1 = false;
        t2 = false;
        t3 = false;
    }
}
