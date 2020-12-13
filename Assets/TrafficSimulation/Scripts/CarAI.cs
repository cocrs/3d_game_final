﻿// Traffic Simulation
// https://github.com/mchrbn/unity-traffic-simulation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Vehicles.Car;

namespace TrafficSimulation {
    public class CarAI : MonoBehaviour {
        [Header("Traffic System")]
        public TrafficSystem trafficSystem;
        public int waypointThresh = 2;

        [Header("Raycast")]
        public Transform raycastAnchor;
        public float raycastLength = 5;
        public int raySpacing = 2;
        public int raysNumber = 6;

        [Header("Vehicle")]
        public float minTopSpeed;
        public float maxTopSpeed;

        public bool hasToStop = false;
        public bool hasToGo = false;
        public bool crushIntoPlayer = false;

        VehiclePhysics carController;
        NavMeshAgent agent;
        int curWp = 0;
        public int curSeg = 0;
        float initialTopSpeed;
        bool regularPath = true;
        GameObject[] playerTarget;
        bool pause = false;


        void Start() {
            carController = this.GetComponent<VehiclePhysics>();

            initialTopSpeed = Random.Range(minTopSpeed, maxTopSpeed);
            carController.Topspeed = initialTopSpeed;

            //Create navmesh agent children
            GameObject aiGo = new GameObject("NavmeshAgent");
            aiGo.transform.SetParent(this.transform, false);
            agent = aiGo.AddComponent<NavMeshAgent>();
            agent.radius = 0.7f;
            agent.height = 1;
            agent.speed = 1;

            crushIntoPlayer = Random.Range(0f, 1f) > 0.5f;

            playerTarget = GameObject.FindGameObjectsWithTag("playerTarget");

            if (trafficSystem == null)
                return;

            //Find segment
            foreach (Segment segment in trafficSystem.segments) {
                if (segment.IsOnSegment(this.transform.position)) {
                    curSeg = segment.id;
                    //Find nearest waypoint to start within the segment
                    float minDist = float.MaxValue;
                    for (int j = 0; j < trafficSystem.segments[curSeg].waypoints.Count; j++) {
                        float d = Vector3.Distance(this.transform.position, trafficSystem.segments[curSeg].waypoints[j].transform.position);
                        //Only take in front points
                        Vector3 lSpace = this.transform.InverseTransformPoint(trafficSystem.segments[curSeg].waypoints[j].transform.position);
                        if (d < minDist && lSpace.z > 0) {
                            minDist = d;
                            curWp = j;
                        }
                    }
                    break;
                }
            }
        }

        void Update() {
            if (trafficSystem == null)
                return;

            if (this.pause) {
                Debug.Log("pause");
            } else {
                //Set navmesh agent in front of the car
                agent.transform.position = this.transform.position + (this.transform.forward * carController.MotorWheels[0].transform.localPosition.z);
                if (crushIntoPlayer) {
                    playerTargetChecker();
                }

                WaypointChecker();
                float topSpeed = GetCarSpeed();
                MoveVehicle(topSpeed);
            }
        }


        void WaypointChecker() {
            GameObject waypoint = trafficSystem.segments[curSeg].waypoints[curWp].gameObject;
            //Position of next waypoint relative to the car
            Vector3 nextWp = this.transform.InverseTransformPoint(new Vector3(waypoint.transform.position.x, this.transform.position.y, waypoint.transform.position.z));
            if (regularPath) {
                //Set agent destination depending on waypoint
                agent.SetDestination(waypoint.transform.position);
            }

            //Go to next waypoint if arrived to current
            if (nextWp.magnitude < waypointThresh) {
                curWp++;
                if (curWp >= trafficSystem.segments[curSeg].waypoints.Count) {
                    curSeg = GetNextSegmentId();
                    curWp = 0;
                }
            }
        }

        void playerTargetChecker() {
            if (playerTarget.Length == 0) {
                return;
            }
            GameObject target = playerTarget[0];
            float dist = Vector3.Distance(this.gameObject.transform.position, target.transform.position);
            float dirZ = this.gameObject.transform.InverseTransformPoint(target.transform.position).z;
            float dirX = this.gameObject.transform.InverseTransformPoint(target.transform.position).x;
            if (dist < 5f && dist > 0.5f && dirZ > -2f && (dirX > 0.2f || dirX < -0.2f)) {
                regularPath = false;
                agent.SetDestination(target.transform.position);
            } else {
                regularPath = true;
            }
        }

        /// If vehicle within raycast hit length, return its speed. Otherwise return the original initial top speed of the vehicle.
        /// In order to avoid that the current car is going fast than the front one and collide.
        /// If the car has to stop, return 0
        float GetCarSpeed() {
            //If car has to stop, set speed to 0
            if (hasToStop)
                return 0;

            Vector3 anchor = new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z);
            if (raycastAnchor != null)
                anchor = raycastAnchor.position;

            //Check if we are going to collide with a car in front
            CarAI otherCarAI = null;
            CarController hitObj = null;
            float topSpeed = initialTopSpeed;
            float initRay = (raysNumber / 2f) * raySpacing;

            for (float a = -initRay; a <= initRay; a += raySpacing) {
                float hitDist;
                CastRay(anchor, a, this.transform.forward, raycastLength, out otherCarAI, out hitDist, out hitObj);

                //If rays collide with a car, adapt the top speed to be the same as the one of the front vehicle
                if (otherCarAI != null && otherCarAI.carController != null && carController.Topspeed > otherCarAI.carController.Topspeed) {
                    //Check if the car is on the same lane or not. If not the same lane, then we do not adapt the vehicle speed to the one in front
                    //(it just means that the rays are hitting a car on the opposite lane...which shouldn't influence the car's speed)
                    if (hasToGo && !IsOnSameLane(otherCarAI.transform))
                        return topSpeed;

                    //If the hit distance is too close, "emergency slow down" the car so they don't collide
                    else if (hitDist < 2f)
                        return topSpeed = 0f;

                    //Otherwise adapt the car speed to the one in front
                    topSpeed = otherCarAI.carController.Topspeed - 0.05f;
                    break;
                }
                if (hitObj != null) {
                    return 0f;
                }
            }

            //If no collision detected then keep the car top speed
            return topSpeed;
        }


        void MoveVehicle(float _topSpeed) {
            //Wheel steering value
            float steering = Mathf.Clamp(this.transform.InverseTransformDirection(agent.desiredVelocity).x, -1f, 1f);

            //If car is turning then decrease it's maximum
            float topSpeed = _topSpeed;
            if (steering > 0.2f || steering < -0.2f && carController.Topspeed > 15) topSpeed = initialTopSpeed / 3f;
            carController.Topspeed = topSpeed;

            //Move the car
            carController.Move(steering, 1f, 0f);
        }


        void CastRay(Vector3 anchor, float angle, Vector3 dir, float length, out CarAI outCarAI, out float outHitDistance, out CarController outHitObject) {

            outCarAI = null;
            outHitObject = null;
            outHitDistance = -1f;

            //Draw raycast
            Debug.DrawRay(anchor, Quaternion.Euler(0, angle, 0) * dir * length, new Color(1, 0, 0, 0.5f));

            //Detect hit only on the autonomous vehicle layer
            int layerAuto = 1 << LayerMask.NameToLayer("AutonomousVehicle");
            int layerPlayer = 1 << LayerMask.NameToLayer("Player");
            //int layerPlayer = 1 << LayerMask.NameToLayer("Player");
            RaycastHit hit;
            if (Physics.Raycast(anchor, Quaternion.Euler(0, angle, 0) * dir, out hit, length, layerAuto)) {
                outCarAI = hit.collider.GetComponentInParent<CarAI>();
                outHitDistance = hit.distance;
            }
            if (Physics.Raycast(anchor, Quaternion.Euler(0, angle, 0) * dir, out hit, length, layerPlayer)) {
                outHitObject = hit.collider.GetComponentInParent<CarController>();
                outHitDistance = hit.distance;
            }
        }

        int GetNextSegmentId() {
            if (trafficSystem.segments[curSeg].nextSegments.Count == 0)
                return 0;
            int c = Random.Range(0, trafficSystem.segments[curSeg].nextSegments.Count);
            return trafficSystem.segments[curSeg].nextSegments[c].id;
        }

        bool IsOnSameLane(Transform otherCar) {
            Vector3 diff = this.transform.forward - otherCar.transform.forward;
            if (Mathf.Abs(diff.x) < 0.3f && Mathf.Abs(diff.z) < 0.3f) return true;
            else return false;
        }
        public void Pause() {
            Debug.Log("Pause");
            this.pause = true;
            this.enabled = false;
            this.carController.enabled = false;
        }
        public void Resume() {
            Debug.Log("Resume");
            this.pause = false;
            this.enabled = true;
            this.carController.enabled = true;
        }
        public bool IsPaused() {
            return this.pause;
        }
    }

}