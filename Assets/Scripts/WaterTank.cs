using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTank : MonoBehaviour
{
    int clickAmount = 1;

    Animator anim;
    public GameObject popUpTextPrefab;
    public AudioClip clip;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Click()
    {
        GameManager.instance.AddMoney(GameManager.instance.foodList[0].foodAmount + clickAmount);
        anim.SetTrigger("clickTrigger");

        GameObject pop = Instantiate(popUpTextPrefab, this.transform, false) as GameObject;
        if (Input.touchCount > 0)
        {
            pop.transform.position = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            pop.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
        }
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);

        pop.GetComponent<PopUpText>().ShowInfo(GameManager.instance.foodList[0].foodAmount + clickAmount);
    }
}
