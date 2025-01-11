using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class LoadManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static LoadManager Instance { get; private set; }

    public LevelManager levelManager;

    public bool gameFromSave = false;

    public string nick;
    public string date;

    public void Awake() {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
        }
        else{
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void loadGame(string nickname, string timeDate) {
        gameFromSave = true;
        nick = nickname;
        date = timeDate;
        levelManager.changeScene();
    }
}
