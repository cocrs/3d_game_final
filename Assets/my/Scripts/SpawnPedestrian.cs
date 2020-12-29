using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPedestrian : MonoBehaviour
{
    public GameObject[] prefab;
    public int maxCount = 2;
    Queue<GameObject> persons;
    float lastSpawn;
    void Start() {
        persons = new Queue<GameObject>();
        lastSpawn = Time.time;
    }

    // Update is called once per frame
    void Update() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5, 1 << 12 | 1 << 10);
        if (colliders.Length == 0 && Time.time - lastSpawn > 5f && persons.Count < maxCount) {
            GameObject spawnedPerson = Instantiate(prefab[Random.Range(0, prefab.Length)], this.transform.position, this.transform.rotation);
            persons.Enqueue(spawnedPerson);
            // if (persons.Count > maxCount) {
            //     Destroy(persons.Dequeue());
            // }
            lastSpawn = Time.time;
        }
    }
}
