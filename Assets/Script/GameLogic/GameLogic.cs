using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    public static GameLogic _instance;

    public static GameLogic GetInstance()
    {
        if (_instance == null)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load("GameLogic")) as GameObject;
            _instance = obj.GetComponent<GameLogic>();
            DontDestroyOnLoad(obj);
        }
        return _instance;
    }

	// Use this for initialization
	void Start () {
        Init();       
	}

    private void Init()
    {
        TaskManager.instance.Init();
    }

}
