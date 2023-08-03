using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSerialController : MonoBehaviour {
	float delta_curX;
	public GameObject MapHeadingObj;
	public Transform AirStickers;
	// Use this for initialization
	void Start () {
//		Global.angle = PlayerPrefs.GetFloat ("angle");
//		if(PlayerPrefs.GetFloat ("radio_angle") != 0)
//			Global.radio_angle = PlayerPrefs.GetFloat ("radio_angle");
//		if(PlayerPrefs.GetFloat ("hangle") != 0)
//			Global.hangle = PlayerPrefs.GetFloat ("hangle");
		/*
		MapHeadingObj.transform.rotation = Quaternion.Euler(0, 0, -Global.angle);
		float angle1 = (float)(OnlineMapsUtils.Deg2Rad * 0.5f);
		float d = (float)(Mathf.Sqrt (Screen.width * Screen.width / 4.0f + Screen.height * Screen.height / 4.0f));
		//		float l = Mathf.Abs((float)(2 * d * Mathf.Sin (angle / 2.0f)));
		//		delta_curX = Mathf.Abs (l * Mathf.Cos (angle));
		delta_curX = Mathf.Sqrt(2 * d * d * (1 - Mathf.Cos(angle1))) * Mathf.Cos(angle1);
		if (Global.angle > 0) {
			AirStickers.localPosition += new Vector3 (delta_curX, 0, 0) * Global.angle / 0.5f;
		} else {
			AirStickers.localPosition -= new Vector3 (delta_curX, 0, 0) * Global.angle / 0.5f;
		}
		Debug.Log ("radio_angle=" + Global.radio_angle);
		Debug.Log ("angle=" + Global.angle);
//		Debug.Log ("hangle=" + Global.hangle);
		*/
	}
	
	// Update is called once per frame
	void Update () {
/*		
		if(Input.GetKey(KeyCode.LeftArrow))
        {	
			AirStickers.localPosition -= new Vector3 (delta_curX, 0, 0);
			Global.angle += 0.5f;
			Debug.Log ("angle=" + Global.angle);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
			AirStickers.localPosition += new Vector3 (delta_curX, 0, 0);
			Global.angle -= 0.5f;
			Debug.Log ("angle=" + Global.angle);
        }
		else if(Input.GetKey(KeyCode.A))
		{	
			Global.radio_angle += 0.5f;
			Debug.Log ("radio_angle=" + Global.radio_angle);
		}
		else if(Input.GetKey(KeyCode.Z))
		{
			Global.radio_angle -= 0.5f;
			Debug.Log ("radio_angle=" + Global.radio_angle);
		}
		else if(Input.GetKey(KeyCode.Q))
		{	
			Global.hangle += 0.5f;
			Debug.Log ("hangle=" + Global.hangle);
		}
		else if(Input.GetKey(KeyCode.W))
		{
			Global.hangle -= 0.5f;
			Debug.Log ("hangle=" + Global.hangle);
		}

		MapHeadingObj.transform.rotation = Quaternion.Euler(0, 0, -Global.angle);
//		PlayerPrefs.SetFloat ("radio_angle", Global.radio_angle);
//		PlayerPrefs.SetFloat ("angle", Global.angle);
//		PlayerPrefs.SetFloat ("hangle", Global.hangle);
*/
	}
}
