using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using QuantumTek.QuantumUI;


public class ShopSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public Test gameManager;
    public GameObject ShopWindow;
    public GameObject infoWindow;
    public QUI_Window confirmWindow;
    public List<GameObject> shopButtons;
    public GameObject notEnoughMoneyTXT;
    TextMeshProUGUI infoText;
    Dictionary<string, dynamic>[] shopItems;
    int currentChoice;
    void Start()
    {
        notEnoughMoneyTXT.SetActive(false);
        shopItems = gameManager.items;

        // get all buttons
        shopButtons = new List<GameObject>() { };
        foreach (Transform child in ShopWindow.transform.GetChild(0))
        {
            Debug.Log(child.name);
            if (child.name == "Buttons")
            {
                foreach (Transform COC in child.transform)
                {
                    shopButtons.Add(COC.gameObject);
                }
            }
        }

        infoText = infoWindow.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();


    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeInfoText(int num)
    {
        infoText.text = shopItems[num]["discription"];
    }
    public void ResetInfoText()
    {
        infoText.text = "INNER INFO";
    }
    public void ClickItem(int num)
    {
        currentChoice = num;
        confirmWindow.SetActive(true);
    }

    public void BuyItem()
    {
        if (shopItems[currentChoice]["price"] <= gameManager.playerDollars)
        {
            Debug.Log("Buy " + currentChoice);
            shopItems[currentChoice]["belong"] = "player";
            confirmWindow.SetActive(false);
            shopButtons[currentChoice].GetComponent<Button>().interactable = false;
            gameManager.playerGetItem(currentChoice);

        }
        else{
            notEnoughMoneyTXT.SetActive(true);
            notEnoughMoneyTXT.GetComponent<Animation>().Stop();
            notEnoughMoneyTXT.GetComponent<Animation>().Play();
        }
    }
    public void CancelConfirm()
    {
        confirmWindow.SetActive(false);
    }
}
