using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TagController : MonoBehaviour {
	public string TagID;
	public GameObject airplane_popup;
	public GameObject terminate_popup;
	public GameObject gate_popup;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public void OnCloseTagPopup()
	{
		int id = Global.GetIndexOfSpot(TagID);
		Global.count_time = 0.0f;
		if (airplane_popup.activeSelf) {
			StartCoroutine (HidePopup (1, airplane_popup, id));
		} else if (terminate_popup.activeSelf) {
			StartCoroutine (HidePopup (2, terminate_popup, id));
		} else if (gate_popup.activeSelf) {
			StartCoroutine (HidePopup (3, gate_popup, id));
		}
		Global.selLanguge = -1;
	}    

	public void popup_Click()
	{
		int id = Global.GetIndexOfSpot(TagID);
		if (id < 0)
		{
			Debug.Log("The name " + TagID + "is invalid");
			return;
		}
		if (Global.is_showing_popup) {
			OnCloseTagPopup ();
		} else {
			GameObject.Find("UIManager").transform.GetComponent<UIManager>().LoadTagWindow(id);
			Global.is_showing_popup = true;
			Global.current_spot_id = id;
			try{
				if (Global.Spots[id].SpotOnMapObject != null)
				{
					Global.Spots[id].SpotOnMapObject.transform.Find("bg").gameObject.SetActive(false);
					if(Global.Spots[id].SpotOnMapObject.transform.Find("bg1")!=null)
						Global.Spots[id].SpotOnMapObject.transform.Find("bg1").gameObject.SetActive(true);
				}
			}
			catch(Exception ex)
			{
				Debug.Log("popup_Click error:" + ex.ToString());
			}
		}
	}
	IEnumerator HidePopup(int type, GameObject obj, int ID)
	{
		yield return new WaitForSeconds(0.2f);
		try{
			if (ID >= 0 && Global.Spots[ID].SpotOnMapObject != null)
			{
				Global.Spots[ID].SpotOnMapObject.transform.Find("bg").gameObject.SetActive(true);
				Global.Spots[ID].SpotOnMapObject.transform.Find("bg1").gameObject.SetActive(false);
			}
		}catch(Exception){
		}
		obj.SetActive (false);
		Global.is_showing_popup = false;
		Global.current_spot_id = -1;
	}
}
