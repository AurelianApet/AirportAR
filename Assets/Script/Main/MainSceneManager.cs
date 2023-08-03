using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour {
    private float screen_rate = 0.0f;
    float trueHeading = 0;
    private float compassThreshold = 8;
    System.DateTime mouseIconStartTime;

#if UNITY_ANDROID
    private AndroidJavaObject curActivity;
#endif

    // Use this for initialization
    void Start () {
        Global.TouchIconObjs = new List<GameObject>();
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        screen_rate = (float)1080 / (float)Screen.width;

#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        curActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    }
    
    // Update is called once per frame
    void Update () {
#if UNITY_ANDROID
#else
		float heading = Input.compass.trueHeading;
#endif
        float offset = trueHeading - heading;
        if (offset > 360) offset -= 360;
        else if (offset < -360) offset += 360;
#if UNITY_ANDROID
#elif UNITY_IPHONE
        if (Mathf.Abs(offset) > compassThreshold)
        {
//            trueHeading = heading;
//            MapHeadingObj.transform.rotation = Quaternion.Euler(0, 0, trueHeading);
//            MapHeadingObj1.transform.rotation = Quaternion.Euler(0, 0, trueHeading);
        }
#else
#endif
    }

    IEnumerator RemoveMouseIcon(GameObject TouchIcon, System.DateTime startTime)
    {
        if((System.DateTime.Now- startTime).TotalMilliseconds < 1000)
            yield return new WaitForSeconds(1f);
        else
            yield return new WaitForSeconds(0.1f);

        GameObject.DestroyObject(TouchIcon);
        TouchIcon = null;
    }
}
