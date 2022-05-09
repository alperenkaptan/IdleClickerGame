using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.Serializable]
    public class AnyFood
    {
        [HideInInspector] public int foodAmount;
        public Food food;
        public bool unlocked;
        [HideInInspector] public bool instanced;
        [HideInInspector] public ItemHolder holder;
    }

    public List<AnyFood> foodList = new List<AnyFood>();

    [Header("MONEY")]
    public float money;
    public float allGPS;
    public DateTime date;
    public Text totalMoneyText;
    public Text totalGPSText;

    [Header("USER INTERFACE")]
    public GameObject itemHolderUI;
    public Transform grid;

    [Header("IDLE SAVE LOAD")]
    public string SaveFileName;
    public GameObject saveText;
    public GameObject gameLoadNotfy;
    public Text idleTimeText;
    public Text idleIncomeText;

    [Header("UNITY AD")]
    public AdsInitializer adsInitializer;

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
        if (PlayerPrefs.HasKey(SaveFileName))
        {
            LoadTheGame();
        }
        else
        {
            FillList();
        }
        
        UpdateMoneyUI();
        CalculateGPS();
        StartCoroutine(Tick());
        AutoSave();
    }

    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            foreach (AnyFood f in foodList)
            {
                if (f.foodAmount > 0)
                {
                    money += f.food.CalculateIncome(f.foodAmount);
                    money = (float)Mathf.Round(money * 100) / 100;

                    UpdateMoneyUI();
                }
            }
        }
    }


    void FillList()
    {
        foreach (var f in foodList.Select((value, i) => (value, i)))
        {
            if (f.value.unlocked)
            {
                if (f.value.foodAmount > 0 || f.value.instanced)
                {
                    continue;
                }

                GameObject itemHolder = Instantiate(itemHolderUI,grid,false) as GameObject;
                f.value.holder = itemHolder.GetComponent<ItemHolder>();
                if (f.value.foodAmount > 0)
                {
                    f.value.holder.itemImage.sprite = f.value.food.foodImage;
                    f.value.holder.itemNameText.text = f.value.food.foodName;
                    f.value.holder.amountText.text = "Amount: " + f.value.foodAmount.ToString("N0");
                    f.value.holder.gpsText.text = "GPS: " + f.value.food.CalculateIncome(f.value.foodAmount).ToString("N2");
                    f.value.holder.costText.text = "Cost: " + f.value.food.CalculateCost(f.value.foodAmount).ToString("N2");
                }
                else
                {
                    f.value.holder.itemImage.sprite = f.value.food.unknownFoodImage;
                    f.value.holder.itemNameText.text = "????";
                    f.value.holder.amountText.text = "Amount: " + f.value.foodAmount.ToString("N0");
                    f.value.holder.gpsText.text = "GPS: " + f.value.food.CalculateIncome(f.value.foodAmount).ToString("N2");
                    f.value.holder.costText.text = "Cost: " + f.value.food.CalculateCost(f.value.foodAmount).ToString("N2");
                }
                f.value.holder.buyButton.id = f.i;
                f.value.instanced = true;

            }
        }
    }

    public void GiveAdReward()
    {
        float allGPS = 0;
        foreach (AnyFood f in foodList)
        {
            if (f.foodAmount > 0)
            {
                allGPS += f.food.CalculateIncome(f.foodAmount);
            }
        }
        money += allGPS * 900; //15mins
        UpdateMoneyUI();
    }

    public void BuyItem(int id)
    {
        if (money < foodList[id].food.CalculateCost(foodList[id].foodAmount))
        {
            //Debug.Log("not enough money");
            return;
        }
        money -= foodList[id].food.CalculateCost(foodList[id].foodAmount);

        //UPDATE UI IN HOLDER
        if (foodList[id].foodAmount < 1)
        {
            foodList[id].holder.itemImage.sprite = foodList[id].food.foodImage;
            foodList[id].holder.itemNameText.text = foodList[id].food.foodName;
        }
        foodList[id].foodAmount++;
        foodList[id].holder.amountText.text = "Amount: " + foodList[id].foodAmount;
        foodList[id].holder.gpsText.text = "GPS: " + foodList[id].food.CalculateIncome(foodList[id].foodAmount).ToString("N2");
        foodList[id].holder.costText.text = "Cost: " + foodList[id].food.CalculateCost(foodList[id].foodAmount).ToString("N2");

        //UNLOCK THE NEXT FOOD
        if (id < foodList.Count - 1 && foodList[id].foodAmount > 0)
        {
            foodList[id + 1].unlocked = true;
            FillList();
        }
        //UPDATE STATS AND MONEY
        UpdateMoneyUI();
        CalculateGPS();

    }

    public void AddMoney(int clickAmount)
    {
        money += clickAmount;
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        totalMoneyText.text = "Total Money: " + money.ToString("N2");
    }

    void CalculateGPS()
    {
        foreach (AnyFood f in foodList)
        {
            if (f.foodAmount > 0)
            {
                allGPS += f.food.CalculateIncome(f.foodAmount);
            }
        }
        totalGPSText.text = "Total GPS: " + allGPS.ToString("N2");
    }

    void SaveTheGame()
    {
        List<int> l = new List<int>();
        foreach (AnyFood f in foodList)
        {
            l.Add(f.foodAmount);
        }
        SaveLoad.Save(l,money,DateTime.Now);
    }

    void AutoSave()
    {
        SaveTheGame();
        saveText.SetActive(true);
        Invoke("AutoSave", 30f);
    }

    float CalculateIdleIncome(DateTime date)
    {
        TimeSpan dateDiff = DateTime.Now - date;
        idleTimeText.text = "Idle Time(sec): " + dateDiff.TotalSeconds.ToString("N0");
        float tempInc = (float)dateDiff.TotalSeconds * allGPS;
        idleIncomeText.text = "Idle Income: " + tempInc.ToString("N2");
        return tempInc;
    }

    void LoadTheGame()
    {
        gameLoadNotfy.SetActive(true);
        if (PlayerPrefs.HasKey(SaveFileName))
        {
            string data = SaveLoad.Load();
            //Debug.Log("data: " + data);
            string[] stringList = data.Split("|"[0]);
            int i;
            for (i = 0; i < stringList.Length-2; i++)
            {
                int temp = int.Parse(stringList[i]);
                foodList[i].foodAmount = temp;

                if (temp > 0)
                {
                    if (i+1 < foodList.Count)
                    {
                        foodList[i + 1].unlocked = true;
                    }
                    FillSingleItem(i);
                }
            }
            
            date = DateTime.Parse(stringList[i+1]);
            //Debug.Log("date: " + date);
            CalculateGPS();
            money = float.Parse(stringList[i]) + CalculateIdleIncome(date);
            UpdateMoneyUI();

            FillList();
        }

    }

    void FillSingleItem(int id)
    {
        if (foodList[id].unlocked)
        {
            GameObject itemHolder = Instantiate(itemHolderUI, grid, false) as GameObject;
            foodList[id].holder = itemHolder.GetComponent<ItemHolder>();

            if (foodList[id].foodAmount > 0)
            {
                foodList[id].holder.itemImage.sprite = foodList[id].food.foodImage;
                foodList[id].holder.itemNameText.text = foodList[id].food.foodName;
                foodList[id].holder.amountText.text = "Amount: " + foodList[id].foodAmount.ToString("N0");
                foodList[id].holder.gpsText.text = "GPS: " + foodList[id].food.CalculateIncome(foodList[id].foodAmount).ToString("N2");
                foodList[id].holder.costText.text = "Cost: " + foodList[id].food.CalculateCost(foodList[id].foodAmount).ToString("N2");
            }
            else
            {
                foodList[id].holder.itemImage.sprite = foodList[id].food.unknownFoodImage;
                foodList[id].holder.itemNameText.text = "????";
                foodList[id].holder.amountText.text = "Amount: " + foodList[id].foodAmount;
                foodList[id].holder.gpsText.text = "GPS: " + foodList[id].food.CalculateIncome(foodList[id].foodAmount).ToString("N2");
                foodList[id].holder.costText.text = "Cost: " + foodList[id].food.CalculateCost(foodList[id].foodAmount).ToString("N2");
            }
            foodList[id].holder.buyButton.id = id;
            foodList[id].instanced = true;
        }
    }

}
