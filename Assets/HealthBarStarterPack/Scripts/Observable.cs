using System.Collections.Generic;
using UnityEngine;

public class Observable : MonoBehaviour {
	private List<Observer> observers = new List<Observer>();
	public void NotifyObservers() {
		for (int i = 0; i < observers.Count; i++) {
			observers [i].Notify ();
		}
	}
	public void AddObserver(Observer o) {
		observers.Add (o);
	}
	public void RemoveObserver(Observer o) {
		if (observers.Contains (o)) observers.Remove (o);
	}
}
