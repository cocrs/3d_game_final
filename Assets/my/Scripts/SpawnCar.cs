using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class SpawnCar : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject carPrefab;
    public GameObject carUnusualPrefab;
    public TrafficSystem trafficSystem;
    public GameObject explosion;
    public float crushPropability = 0.5f;
    public int maxCarCount = 50;
    List<GameObject> cars;
    float lastSpawn;
    void Start() {
        cars = new List<GameObject>();
        lastSpawn = Time.time;
        crushPropability = crushPropability < 0f ? 0f : crushPropability > 1f ? 1f : crushPropability;
    }

    // Update is called once per frame
    void Update() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5, 1 << 11 | 1 << 10);
        if (colliders.Length == 0 && Time.time - lastSpawn > 10f) {
            GameObject spawnedCar;
            if (Random.Range(0f, 1f) > crushPropability) {
                spawnedCar = Instantiate(carPrefab, this.transform.position, this.transform.rotation);
                spawnedCar.GetComponent<CarAI>().trafficSystem = trafficSystem;
                spawnedCar.GetComponent<CarAI>().crushIntoPlayer = false;
            } else {
                spawnedCar = Instantiate(carUnusualPrefab, this.transform.position, this.transform.rotation);
                spawnedCar.GetComponent<CarAI>().trafficSystem = trafficSystem;
                spawnedCar.GetComponent<CarAI>().crushIntoPlayer = true;
            }
            cars.Add(spawnedCar);
            while (cars.Count > maxCarCount) {
                GameObject toDestroy = cars[maxCarCount];
                cars.RemoveAt(maxCarCount);
                Destroy(toDestroy);
            }
            for (int i = 0; i < cars.Count; i++) {
                CarAI carai = cars[i].GetComponent<CarAI>();
                if ((Time.time - carai.notTurnOver > 5f && carai.notTurnOver != 0f) || (Time.time - carai.nonStopTime > 5f && carai.nonStopTime != 0f)) {
                    Debug.Log("自爆");
                    Debug.Log(Time.time);
                    Debug.Log(carai.notTurnOver);
                    GameObject toDestroy = cars[i];
                    cars.RemoveAt(i);
                    Vector3 explodePos = toDestroy.transform.position;
                    Instantiate(explosion, explodePos, Quaternion.Euler(0f, 0f, 0f));
                    float explodeRadius = 10f;
                    Collider[] explosionEffect = UnityEngine.Physics.OverlapSphere(explodePos, explodeRadius);
                    foreach (Collider c in explosionEffect) {
                        Rigidbody r = c.GetComponent<Rigidbody>();
                        if (r) {
                            Debug.Log(r.name);
                            r.AddExplosionForce(800000f, explodePos, explodeRadius);
                        }
                    }
                    Destroy(toDestroy);
                }
            }
            lastSpawn = Time.time;
        }
    }
}
