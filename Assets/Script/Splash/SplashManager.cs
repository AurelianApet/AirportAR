using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SceneManager.LoadScene(Global.MainSceneName);
	}
}
