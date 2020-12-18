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

    void Awake() {
        TimerCountMen = GameObject.Find("TimerCount");
        CountDownText = GameObject.Find("TimerCountTXT").GetComponent<Text>();
        tooFarTXT = GameObject.Find("TooFarTXT");
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
            // Is parking finished?
            if (!FinisheD) {// No ,parking isn't finish, check parking state

                if (t0 && t2 && t3 && t1) {// If all of car triggers being entered in parking place
                                           // Checking when timer is reached 0(from    3)
                    if (GameManager.isTooFarFromGoal()) {
                        tooFarTXT.SetActive(true);
                        ParkRenderer.material.color = Color.red;
                        GameManager.parkingLotUsing = this.gameObject;
                    } else {
                        StartCoroutine(CheckTimeToFinisheD());
                        // Level is finished
                        GameManager.parkingLotUsing = this.gameObject;
                        // Park renderer is now green(Correct location on parking place)
                        ParkRenderer.material.color = Color.green;

                        if (canLoadinnn) {
                            TimerCountMen.SetActive(true);
                            CountDownText.gameObject.SetActive(true);
                        }

                        int timeLeft = (int)(endTime - Time.time);
                        if (timeLeft <= 0) {
                            timeLeft = 0;
                            isFinish = true;
                        }

                        //Timer Info  3...2...1....
                        CountDownText.text = timeLeft.ToString();
                    }
                } else if (GameManager.parkingLotUsing == this.gameObject) { // only same parking lot can stop its coroutine
                                                                             // Stop checking car parking state
                    StopCoroutine(CheckTimeToFinisheD());
                    tooFarTXT.SetActive(false);
                    GameManager.parkingLotUsing = null; // no parking lot is active

                } else if (GameManager.parkingLotUsing == null) { // Car doesn't on correct parking place
                                                                  // Car parking doesn't finish
                    isFinish = false;
                    // Stop Count down timer
                    TimerCountMen.SetActive(false);

                    endTime = Time.time + 4;

                    CountDownText.text = "3";

                    // Car parking place now idle state =>color is now white
                    ParkRenderer.material.color = Color.white;
                }

            }
        }
    }
    IEnumerator CheckTimeToFinisheD() {
        yield return new WaitForSeconds(4f);
        if (isFinish == true) {
            FinisheD = true;

            // print("finish");

            // disable car moving
            // GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = false;
            // Stop timer menu
            TimerCountMen.SetActive(false);

            CountDownText.gameObject.SetActive(false);

            GameManager.finishParking = true;
            GameManager.questFinished = true;
            ParkRenderer.material.color = Color.white;
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
