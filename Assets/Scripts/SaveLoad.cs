using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoad 
{
    public static void Save(List<int> T,float money,DateTime date)
    {
        //Debug.Log("Game Saved");
        PlayerPrefs.SetString(GameManager.instance.SaveFileName, string.Join("|", T)+"|"+money+"|"+date);
    }

    public static string Load()
    {
        //Debug.Log("Game Loaded");
        return PlayerPrefs.GetString(GameManager.instance.SaveFileName);
    }

}
