using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassManager : MonoBehaviour {

	float delta_curX;
	public GameObject MapHeadingObj;
	public Transform AirStickers;
	private float temp_angle = 0.0f;
	private float delta_angle = 0.0f;
    #if UNITY_ANDROID
        private AndroidJavaObject curActivity;
    #endif
	// Use this for initialization
	void Start () {
        #if UNITY_ANDROID
            //AndroidJavaClass jc = new AndroidJavaClass("com.asplugins.view999.myunityplugintest.MainActivity");
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            curActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

            //string retn = curActivity.CallStatic<string>("HelloWorld", ":Test");

            string retn = curActivity.Call<string>("HelloWorld", ":initiated Sensor");
        #endif
		Input.compass.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		float angle = 0.0f;
		float anglex = 0.0f;
		float anglez = 0.0f;
        #if UNITY_ANDROID
            angle = curActivity.Call<float>("RetrieveAngle", "");
            anglex = curActivity.Call<float>("RetrieveAngle1", "");
			anglez = curActivity.Call<float>("RetrieveAngle2", "");
		#else
		angle = Input.compass.trueHeading;
        #endif
		MapHeadingObj.transform.localRotation = Quaternion.Euler(new Vector3(Global.up_offset + anglex-270.0f, angle+Global.rot_offset, anglez));        
		delta_angle = anglex - temp_angle;
		float angle1 = (float)(OnlineMapsUtils.Deg2Rad * delta_angle);
		float d = (float)(Mathf.Sqrt (Screen.width * Screen.width / 4 + Screen.height * Screen.height / 4));
		float l = Mathf.Abs((float)(2 * d * Mathf.Sin (angle1 / 2)));
		delta_curX = Mathf.Abs (l * Mathf.Cos (angle1));
		AirStickers.localPosition += new Vector3 (delta_curX, 0, 0);
	}

    string GetAndroidStr()
    {
        string result = "123";
        #if UNITY_ANDROID
            result = curActivity.Call<string>("HelloWorld", ":Test");
	    #endif
        return result;
    }
}
