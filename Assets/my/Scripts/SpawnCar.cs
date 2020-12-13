using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class SpawnCar : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject carPrefab;
    public TrafficSystem trafficSystem;
    public int maxCarCount = 50;
    Queue<GameObject> cars;
    float lastSpawn;
    void Start() {
        cars = new Queue<GameObject>();
        lastSpawn = Time.time;
    }

    // Update is called once per frame
    void Update() {
        if (Time.time - lastSpawn > 5f) {
            GameObject spawnedCar = Instantiate(carPrefab, this.transform.position, this.transform.rotation);
            spawnedCar.GetComponent<CarAI>().trafficSystem = trafficSystem;
            cars.Enqueue(spawnedCar);
            if (cars.Count > maxCarCount) {
                Destroy(cars.Dequeue());
            }
            lastSpawn = Time.time;
        }
    }
}
