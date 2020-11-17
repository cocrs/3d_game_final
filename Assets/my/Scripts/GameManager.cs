using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameObject parkingLotUsing;
    public GameObject[] buildings;
    public GameObject startPos;
    public GameObject goal;
    public GameObject goalIcon;
    // Start is called before the first frame update
    void Start()
    {
        parkingLotUsing = null;
        buildings = GameObject.FindGameObjectsWithTag("Building");
        SetRandomGoal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetRandomGoal(){
        goal = buildings[Random.Range(0, buildings.Length)];
        Vector3 center = goal.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.center;
        Instantiate(goalIcon, new Vector3(center.x, 30f, center.z), Quaternion.identity);
    }
}
