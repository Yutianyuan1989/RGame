using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScene : MonoBehaviour {

    public Button btn;
    public string SceneName;
	// Use this for initialization
	void Start () {
        btn.onClick.AddListener(() => SceneManager.LoadScene(SceneName));
	}
	

}
