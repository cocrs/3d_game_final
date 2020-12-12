using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using QuantumTek.QuantumUI;


public class ShopSystem : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject ShopWindow;
    public GameObject infoWindow;
    public QUI_Window confirmWindow;
    List<GameObject> shopButtons;
    TextMeshProUGUI infoText;
    Dictionary<string, dynamic>[] shopItems;
    int currentChoice;
    void Start() {
        shopItems = new Dictionary<string, dynamic>[]{
            new Dictionary<string, dynamic>(){
                {"name", "Item1"},
                {"discription", "Discription1"},
                {"price", 100}
            },
            new Dictionary<string, dynamic>(){
                {"name", "Item2"},
                {"discription", "Discription2"},
                {"price", 200}
            },
            new Dictionary<string, dynamic>(){
                {"name", "Item3"},
                {"discription", "Discription3"},
                {"price", 300}
            },
            new Dictionary<string, dynamic>(){
                {"name", "Item4"},
                {"discription", "Discription4"},
                {"price", 400}
            },
        };

        shopButtons = new List<GameObject>() { };
        foreach (Transform child in ShopWindow.transform.GetChild(0)) {
            Debug.Log(child.name);
            if (child.name == "Buttons") {
                foreach (Transform COC in child.transform) {
                    shopButtons.Add(COC.gameObject);
                }
            }
        }

        infoText = infoWindow.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();


    }

    // Update is called once per frame
    void Update() {
    }

    public void ChangeInfoText(int num) {
        infoText.text = shopItems[num]["discription"];
    }
    public void ResetInfoText() {
        infoText.text = "INNER INFO";
    }
    public void ClickItem(int num) {
        currentChoice = num;
        confirmWindow.SetActive(true);
    }

    public void BuyItem() {
        Debug.Log("Buy " + currentChoice);
        confirmWindow.SetActive(false);
    }
    public void CancelConfirm() {
        confirmWindow.SetActive(false);
    }
}
