using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateLoadText : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        AnimatorClipInfo[] info = anim.GetCurrentAnimatorClipInfo(0);

        Invoke("Deactivate", info[0].clip.length);
    }

    void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
