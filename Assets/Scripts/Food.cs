using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName= "Idle Game Asset/Foods")]
public class Food : ScriptableObject
{
    public string foodName;
    public float basePrice;
    public float multiplier = 1.15f; //1.07 - 1.15, easy - hard
    public float baseIncome;

    public Sprite foodImage;
    public Sprite unknownFoodImage;

    public float CalculateCost(int amount)
    {
        float newPrice = basePrice * Mathf.Pow(multiplier, amount);
        float rounded = (float)Mathf.Round(newPrice * 100) / 100;
        return rounded;
    }

    public float CalculateIncome(int amount)
    {
        return baseIncome * amount;
    }

}
