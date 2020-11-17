using UnityEngine;
using System.Collections;

public class CarLoader : MonoBehaviour
{

	//List of the car prefabs
	public GameObject[] Car;

	void Awake ()
	{
		// Instantiate car by loaded  Car ID from  selected in car garage   
		Instantiate (Car [PlayerPrefs.GetInt ("CarID")], transform.position, transform.rotation);
	}
}