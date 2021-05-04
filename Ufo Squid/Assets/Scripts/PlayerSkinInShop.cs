using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinInShop : MonoBehaviour
{
    [Range(1, 7)]
    public int skin = 1;

    void OnEnable()
    {
        GetComponent<Animator>().SetInteger("Skin", skin);
    }
}
