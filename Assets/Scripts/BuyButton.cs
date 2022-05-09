using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButton : MonoBehaviour
{
    [HideInInspector]
    public int id;

    public void BuyItem()
    {
        GameManager.instance.BuyItem(id);
    }
}
