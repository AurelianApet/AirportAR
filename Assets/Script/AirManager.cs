using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LitJson;

public class AirManager : MonoBehaviour {
	public GameObject AirSpot;
	public GameObject TerminalSpot;
	public GameObject GateSpot;
	public GameObject Big_Plane_Prefab;
	public GameObject Small_Plane_Prefab;
	public Transform Big_Plane_Parent;
	public Transform Small_Plane_Parent;
	public Transform AirStickers;
	public Camera VirtualCamera;
	//	public GameObject MapHeadingObj;
	string fxmlUrl;
	string buildAPIUrl;
	string gateAPIUrl;
	static int nSeconds = 001;

	public UIButton airSpotBtn;
	// Use this for initialization
	IEnumerator Start () {
		airSpotBtn.enabled = false;
		Global.currPosition = Global.T1_Postion;
		loadTowerDistance ();
		fxmlUrl = Global.Domain + "/api/fims/list/20180130/12/30/";
		//		fxmlUrl = Global.Domain + "/api/fims/list/" + System.DateTime.Now.ToString("yyyyMMdd/HH/mm");
		buildAPIUrl = Global.Domain + "/api/building";
		gateAPIUrl = Global.Domain + "/api/gateinfo/";
		yield return StartCoroutine ("initBuildings");
		yield return StartCoroutine("initStickers");
		StartCoroutine (UpdateStickers());
	}

	IEnumerator initStickers()
	{
		WWW www = new WWW(fxmlUrl + "001");
		yield return www;
		if(www.error == null)
		{
			JsonData result = JsonMapper.ToObject (www.text);        
			if (Global.Spots == null) {
				Global.Spots = new List<SpotInformation> ();
			}
			//Global.ObjonMaps = new List<GameObject>();
			JsonData dataArray = result ["data"];
			if(dataArray != null && dataArray.Count > 0){
				yield return StartCoroutine(LoadValues (dataArray));
				load_airplane_Map ();
				//비행기스티커
				StartCoroutine (LoadMainWethereIcon(dataArray));
			}
		}
	}

	// Update is called once per frame
	IEnumerator UpdateStickers () {
		while (true) {
			nSeconds++;
			if (nSeconds > 134) {
				nSeconds = 0;
			}
			WWW www = new WWW (fxmlUrl + nSeconds.ToString ("D3"));
			yield return www;
			if (www.error == null) {
				JsonData result = JsonMapper.ToObject (www.text);
				JsonData dataArray = result ["data"];
				if (dataArray != null && dataArray.Count > 0) {
					int nCount = dataArray.Count;
					for (int i = 0; i < nCount; i++) {
						int index = -1;
						bool bFound = false;
						SpotInformation aSpot = null;
						for (int j = Global.BSpots_count; j < Global.Spots.Count; j++) {
							try {
								if (Global.Spots [j].SpotID == dataArray [i] ["publicIdentifier"].ToString ()) {
									bFound = true;
									aSpot = Global.Spots [j];
									aSpot.lastPlaneLat = aSpot.SpotPosition.lat;
									aSpot.lastPlaneLng = aSpot.SpotPosition.lng;
									aSpot.SpotPosition.lng = double.Parse (dataArray [i] ["longitude"].ToString ());
									aSpot.SpotPosition.lat = double.Parse (dataArray [i] ["latitude"].ToString ());
									aSpot.SpotHeight = double.Parse (dataArray [i] ["altitude"].ToString ());
									index = j;
									break;
								}
							} catch (Exception) {
							}
						}
						if (bFound) { //이미 존재하는 스팟인 경우
							show_airplane_Map(aSpot, index, false);

						} else {//새로운 스팟인 경우
							aSpot = new SpotInformation(1, dataArray[i]["publicIdentifier"].ToString(), dataArray[i]["publicIdentifier"].ToString(),
								dataArray[i]["publicIdentifier"].ToString(),
								double.Parse(dataArray[i]["latitude"].ToString()),
								double.Parse(dataArray[i]["longitude"].ToString()),
								double.Parse(dataArray[i]["altitude"].ToString()));
							aSpot = load_plane_information(aSpot, dataArray[i]);
							add_sticker(aSpot);
						}
					}
					int tCount = Global.Spots.Count;
					int jCount = 0;

					for (int i = Global.BSpots_count; i < tCount; i++) {
						bool found = false;
						for (int j = 0; j < nCount; j++) {
							try {
								if (Global.Spots [i - jCount].SpotID == dataArray [j] ["publicIdentifier"].ToString ()) {
									found = true;break;
								}
							} catch (Exception) {
							}
						}
						if (!found) {
							del_sticker(i - jCount);
							Debug.Log("Delete Sticker : " + (i - jCount));
							jCount++; tCount--;

						}
					}
				}
			}
			yield return new WaitForSeconds (1.0f);
		}
	}

	void del_sticker(int stickerID){
		//지도상의 비행기오브젝트 삭제
		if (Global.Spots[stickerID].SpotOnMapObject != null)
		{
			DestroyImmediate(Global.Spots[stickerID].SpotOnMapObject);
			Global.Spots[stickerID].SpotOnMapObject = null;
		}
		//카메라상의 스팟오브젝트 삭제
		if (Global.Spots[stickerID].SpotObject != null)
		{
			DestroyImmediate(Global.Spots[stickerID].SpotObject);
			Global.Spots[stickerID].SpotObject = null;
		}
		Global.Spots.RemoveAt(stickerID);
	}

	void add_sticker(SpotInformation aSpot)
	{
		Global.Spots.Add (aSpot);
		int tex_index = 2;

		GameObject airSpot = Instantiate(AirSpot);
		airSpot.transform.name = "Sticker_" + aSpot.SpotID;
		airSpot.transform.SetParent(AirStickers);
		airSpot.transform.localScale = Vector3.one;
		airSpot.GetComponent<TagController>().TagID = aSpot.SpotID;

		/// 스팟의 크기 결정
		float distance1 = (float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS(aSpot.SpotPosition.lat, aSpot.SpotPosition.lng, Global.currPosition.lat, Global.currPosition.lng));
		float d_angle = (float)(OnlineMapsUtils.bearingP1toP2(Global.currPosition.lat, Global.currPosition.lng, aSpot.SpotPosition.lat, aSpot.SpotPosition.lng));
		distance1 = Mathf.Abs(distance1 * Mathf.Cos ((d_angle - Global.camera3_radio) * Mathf.PI / 180.0f));

        tex_index = chk_distance_tower(airSpot, distance1);

        aSpot.SpotObject = airSpot;
		aSpot.SpotObject.SetActive(true);
		setAirTexture(Global.Spots.Count - 1, tex_index);

		try
		{
			aSpot.SpotObject.transform.Find("AirTagPrefab/Name").GetComponent<UILabel>().text = aSpot.SpotName_kr;
			show_airplane_Map (aSpot, Global.Spots.Count - 1, true);//새로운 비행기

		}
		catch (Exception ex)
		{
			Debug.Log("----Displaying Plane info Error : " + ex.Message);
		}
	}

	private void load_airplane_Map()
	{
		int tex_index = 0;
		//비행기스티커 표시
		for (int i = Global.BSpots_count; i < Global.Spots.Count; i++)
		{
			if (Global.Spots[i] != null)
			{
				GameObject airSpot = Instantiate(AirSpot);
				airSpot.transform.name = "Sticker_" + Global.Spots[i].SpotID;
				airSpot.transform.SetParent(AirStickers);
				airSpot.transform.localScale = Vector3.one;
				airSpot.GetComponent<TagController>().TagID = Global.Spots[i].SpotID;

				float distance = (float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS(Global.Spots[i].SpotPosition.lat, Global.Spots[i].SpotPosition.lng, Global.currPosition.lat, Global.currPosition.lng));
				float d_angle = (float)(OnlineMapsUtils.bearingP1toP2(Global.currPosition.lat, Global.currPosition.lng,Global.Spots[i].SpotPosition.lat , Global.Spots[i].SpotPosition.lng));
				distance = Mathf.Abs(distance * Mathf.Cos ((d_angle - Global.camera3_radio) * Mathf.PI / 180.0f));
                tex_index = chk_distance_tower(airSpot, distance);


				Global.Spots[i].SpotObject = airSpot;
				Global.Spots[i].SpotObject.SetActive(true);
				//StartCoroutine(LoadFlightCarrierImg(i));
				setAirTexture(i,tex_index);

				try
				{
					airSpot.transform.Find("AirTagPrefab/Name").GetComponent<UILabel>().text = Global.Spots[i].SpotName_kr;
					show_airplane_Map(Global.Spots[i], i, true);

				}
				catch (Exception ex)
				{
					Debug.Log("----Displaying Plane info Error : " + ex.Message);
				}
			}
		}
	}

	private void show_airplane_Map(SpotInformation aSpot, int i, bool isupdated/*(초기-false)*/)
	{
		if (Global.Spots[i] == null) return;
		try{
			//비행기스티커
			double distance = OnlineMapsUtils.DistanceBetweenPointsD_GRS (Global.Spots [i].SpotPosition.lat, Global.Spots [i].SpotPosition.lng, Global.currPosition.lat, Global.currPosition.lng);
			double angle = OnlineMapsUtils.bearingP1toP2 (Global.currPosition.lat, Global.currPosition.lng, Global.Spots [i].SpotPosition.lat, Global.Spots [i].SpotPosition.lng);
			float posx = (float)(Math.Sin (angle * OnlineMapsUtils.Deg2Rad) * distance);
			float posz = (float)(Math.Cos (angle * OnlineMapsUtils.Deg2Rad) * distance);
			float posy = (float)Global.Spots [i].SpotHeight - Global.camera_height + Global.plane_height;

			if(float.IsNaN(posx))
				posx = 0;
			if(float.IsNaN(posy))
				posy = 0;
			if(float.IsNaN(posz))
				posz = 0;

			Vector3 newPos = new Vector3(posx, posy, posz);

			aSpot.PrevSpotPos = aSpot.CurSpotPos;
			Vector3 sticker2DPos = VirtualCamera.WorldToScreenPoint (newPos);
			aSpot.CurSpotPos = Global.disortPoint(sticker2DPos) - new Vector3(Screen.width / 2, Screen.height / 2, 0);

			aSpot.NextSpotPos = new Vector3(aSpot.CurSpotPos.x + (aSpot.CurSpotPos.x - aSpot.PrevSpotPos.x),
				aSpot.CurSpotPos.y + (aSpot.CurSpotPos.y - aSpot.PrevSpotPos.y),
				aSpot.CurSpotPos.z + (aSpot.CurSpotPos.z - aSpot.PrevSpotPos.z));

			StartCoroutine(Moveliner(aSpot.SpotObject, aSpot.PrevSpotPos, aSpot.CurSpotPos));
			aSpot.SpotObject.transform.localScale = new Vector3(1f, 1f, 1f);
			UpdateSpotTexture(aSpot.SpotObject, i);

			if (Global.selBuilding == 1)
				aSpot.SpotObject.SetActive (true);
			else
				aSpot.SpotObject.SetActive (false);
		}catch(Exception){
		}

		if (isupdated) {
			aSpot.SpotOnMapObject = Instantiate (Big_Plane_Prefab);
			aSpot.SpotOnMapObject.transform.Find ("txt").GetComponent<UILabel> ().text = aSpot.SpotID;
			aSpot.SpotOnMapObject.transform.SetParent (Big_Plane_Parent);
            
            aSpot.SpotOnMapObject.SetActive (false);
           // aSpot.SpotOnMapObject.GetComponent<UITexture>().mainTexture = texTmp;

        } else {
			if (aSpot.SpotOnMapObject != null)
			{
				aSpot.SpotOnMapObject.SetActive(false);
			}
		}
 
        //지도에 비행기 표시
        if (Global.Spots [i].SpotPosition.lng > Global.MinLngValueOnBig && Global.Spots [i].SpotPosition.lng < Global.MaxLngValueOnBig &&
			Global.Spots [i].SpotPosition.lat > Global.MinLatValueOnBig && Global.Spots [i].SpotPosition.lat < Global.MaxLatValueOnBig) 
		{			
			float w = Mathf.Abs ((float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS (Global.LeftTopLatOnBig, Global.LeftTopLngOnBig, 
				Global.RightTopLatOnBig, Global.RightTopLngOnBig)));
			float h = Mathf.Abs ((float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS (Global.LeftTopLatOnBig, Global.LeftTopLngOnBig, 
				Global.LeftBottomLatOnBig, Global.LeftBottomLngOnBig)));

			double angle = OnlineMapsUtils.bearingP1toP2 (Global.LeftTopLatOnBig, Global.LeftTopLngOnBig, Global.Spots [i].SpotPosition.lat, Global.Spots [i].SpotPosition.lng);
			float wx1 = Mathf.Abs ((float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS (Global.LeftTopLatOnBig, Global.LeftTopLngOnBig, 
				Global.Spots [i].SpotPosition.lat, Global.Spots [i].SpotPosition.lng)));
			float wx2 = Mathf.Abs ((float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS (Global.LeftBottomLatOnBig, Global.LeftBottomLngOnBig, 
				Global.Spots [i].SpotPosition.lat, Global.Spots [i].SpotPosition.lng)));
			float wx3 = Mathf.Abs ((float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS (Global.RightTopLatOnBig, Global.RightTopLngOnBig, 
				Global.Spots [i].SpotPosition.lat, Global.Spots [i].SpotPosition.lng)));

			float s = getArea (h, wx1, wx2);
			float wx = getHeight (s, h);

			s = getArea (w, wx1, wx3);
			float wy = getHeight (s, w);

			float m_posx = wx / w * 1080.0f;
			float m_posy = wy / h * 601.0f;

			if (Global.Spots[i].SpotOnMapObject != null)
			{
				Global.Spots[i].SpotOnMapObject.transform.localScale = Vector3.one;
//				Global.Spots[i].SpotOnMapObject.transform.localPosition = new Vector3 (m_posx, -m_posy, 0) - new Vector3 (540, -601, 0);
				Global.Spots[i].SpotOnMapObject.SetActive (true);
				if (angle > 234.06667 && angle < 324.21766)
				{
				}
				else if(angle < 234.06667){
					//좌측밖으로 벗어났을때
					m_posx = -m_posx;
				}
				else if(angle > 324.21766){
					//우측밖으로 벗어났을때
				}

				if (isupdated) {
					Global.Spots [i].SpotOnMapObject.transform.localPosition = new Vector3 (m_posx, -m_posy, 0) - new Vector3 (540, -601, 0);
				} 
				else {
					Vector3 destVec = new Vector3 (m_posx, -m_posy, 0) - new Vector3 (540, -601, 0);
					float RotValue = getAngle (destVec, Global.Spots [i].SpotOnMapObject.transform.localPosition);
					if (Global.Spots [i].SpotOnMapObject.transform.localRotation.eulerAngles != new Vector3 (0, 0, RotValue)
						&& Global.Spots [i].SpotOnMapObject.transform.localRotation.eulerAngles != new Vector3 (0, 0, RotValue + 360)) 
					{
//						RotValue = RotValue - 90;
						Global.Spots [i].SpotOnMapObject.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, RotValue));
						Debug.Log ("nSecond=" + nSeconds);
						Debug.Log ("SpotID=" + Global.Spots[i].SpotID);
						Debug.Log ("localposition=" + Global.Spots [i].SpotOnMapObject.transform.localPosition.x + "," + Global.Spots [i].SpotOnMapObject.transform.localPosition.y);
						Debug.Log ("currentposition=" + (m_posx - 540) + "," + (-m_posy + 601));
						Debug.Log ("SpotRot=" + RotValue);

						if (Global.Spots [i].SpotOnMapObject.transform.localPosition.y < 0 || -m_posy + 601 < 0) {
							Global.Spots [i].SpotOnMapObject.transform.localPosition = new Vector3 (m_posx, -m_posy, 0) - new Vector3 (540, -601, 0);
						} else {
							// 유연하게 움직이도록 
							StartCoroutine (MovingThread (m_posx, m_posy, i));
						}
					}
				}

				if(Global.Spots [i].SpotOnMapObject.transform.localPosition.y < 0){
					Global.Spots[i].SpotOnMapObject.SetActive (false);
				}
			}
		}
	}

	float getAngle(Vector2 target, Vector2 current)
	{
		if (target.y == current.y) {
			if (target.x > current.x) {
				return 180.0f;
			}
			return 0.0f;
		} else if (target.x == current.x) {
			if (target.y > current.y) {
				return 90.0f;
			}
			return -90.0f;
		}
		float angle = Mathf.Atan2 (target.y - current.y, target.x - current.x) * 180 / Mathf.PI - 90;
		return angle;
	}

	/*float getAngel1(double target_x, double target_y, double current_x, double currrent_y){
		double angle = OnlineMapsUtils.bearingP1toP2 (target_x, target_y, current_x, currrent_y) - Global.camera3_radio;
		return angle;
	}*/

	IEnumerator MovingThread(float xpos,float ypos,int idx)
	{
		Vector3 destVec = new Vector3 (xpos, -ypos, 0) - new Vector3 (540, -601, 0);
		Vector3 originPos = Global.Spots[idx].SpotOnMapObject.transform.localPosition;
		if (Global.Spots [idx] != null) {
			for (int i = 1; i <= 5; i++) {

				if (Global.Spots.Count > idx && Global.Spots [idx] != null && Global.Spots [idx].SpotOnMapObject != null) {
					Global.Spots [idx].SpotOnMapObject.transform.localPosition = Vector3.Lerp (originPos, destVec, 0.2f * i);
				}
				yield return new WaitForSeconds (0.2f);
			}
		}
	}

	IEnumerator initBuildings()
	{
		// 건물 스티커 초기화
		WWW www = new WWW(buildAPIUrl);
		yield return www;
		if (www.error == null) {
			JsonData result = JsonMapper.ToObject(www.text);
			Global.Spots = new List<SpotInformation>();
			JsonData dataArray = result["data"];
			if (dataArray.IsArray)
			{
				int nCount = dataArray.Count;
				for (int i = 0; i < nCount; i++)
				{
					SpotInformation aSpot = new SpotInformation ("", "", 0, 0, 0);
					try{
						aSpot = new SpotInformation(dataArray[i]["itemName"].ToString(),
							dataArray[i]["itemCode"].ToString(), double.Parse(dataArray[i]["itemLatitude"].ToString()),
							double.Parse(dataArray[i]["itemLongitude"].ToString()), double.Parse(dataArray[i]["itemAltitude"].ToString()));
					}
					catch(Exception ex){
						Debug.Log("----init building Error : " + i + " : " + ex.Message);
					}

					try{
						if (dataArray[i]["itemType"].ToString().Contains("T") || dataArray[i]["itemType"].ToString().Contains("B")) //터미널 또는 건물
						{
							aSpot.SpotType = 2;
						}
						else if(dataArray[i]["itemType"].ToString().Contains("G")) // 게이트 
						{
							aSpot.SpotType = 3;
						}else if(dataArray[i]["itemType"].ToString().Contains("M"))
						{
							aSpot.SpotType = 4;
						}

						if (dataArray[i]["itemImage"] != null)
							aSpot.SpotImageUrl = Global.Domain + "/obin/" + dataArray[i]["itemImage"].ToString();
						else
							aSpot.SpotImageUrl = "";
					}catch(Exception ex) {
						Debug.Log ("----init building Error : " + i + " : " + ex.Message);
					}

					if (aSpot.SpotType == 3) // 게이트인 경우
					{
						aSpot.GateInfos = new List<GateInfo>();
						WWW gate_www = new WWW(gateAPIUrl + aSpot.SpotID);
						yield return gate_www;
						if (gate_www.error == null)
						{
							JsonData gate_result = JsonMapper.ToObject(gate_www.text);
							try{
								JsonData gatedataArray = gate_result["data"];
								if (gatedataArray.IsArray)
								{
									int GateInfoCount = gatedataArray.Count;
									for (int k = 0; k < GateInfoCount; k++)
									{
										GateInfo newInfo = new GateInfo();
										newInfo.time = gatedataArray[k]["adTime"].ToString();
										newInfo.GatePosition = gatedataArray[k]["arrivalOrDeparture"].ToString();
										newInfo.DepartCity = gatedataArray[k]["departureCity"].ToString();
										newInfo.DepartCityEn = gatedataArray[k]["departureFullName"].ToString();
										newInfo.DepartCityCn = gatedataArray[k]["departureCcity"].ToString();
										newInfo.DepartCityJp = gatedataArray[k]["departureJcity"].ToString();

										if (gatedataArray[k]["arrivalCity"] != null)
										{
											newInfo.ArrivalCity = gatedataArray[k]["arrivalCity"].ToString();
											newInfo.ArrivalCityEn = gatedataArray[k]["arrivalFullName"].ToString();
											newInfo.ArrivalCityCn = gatedataArray[k]["arrivalCcity"].ToString();
											newInfo.ArrivalCityJp = gatedataArray[k]["arrivalJcity"].ToString();
										}
										else
										{
											newInfo.ArrivalCity = "";
											newInfo.ArrivalCityEn = "";
											newInfo.ArrivalCityCn = "";
											newInfo.ArrivalCityJp = "";
										}
										newInfo.GateTypeImageUrl = Global.Domain + "/img/" + 
											gatedataArray[k]["flightCarrier"].ToString() + ".png";
										newInfo.AirlineName = gatedataArray[k]["publicIdentifier"].ToString();
										aSpot.GateInfos.Add(newInfo);
									}
								}
							}catch(Exception ex) {
								Debug.Log ("----Gate Popup Error : " + ex.Message);
							}
						}
					}
					Global.Spots.Add(aSpot);
				}
			}
			//건물스티커
			for (int ii = 0; ii < Global.Spots.Count; ii++)
			{
				try{
					GameObject SpotObj = null;
					if (Global.Spots[ii].SpotType == 2)
						SpotObj = Instantiate(TerminalSpot);
					else if (Global.Spots[ii].SpotType == 3)
						SpotObj = Instantiate(GateSpot);
					else
						continue;
					SpotObj.transform.name = "Sticker_" + Global.Spots[ii].SpotID;
					SpotObj.transform.SetParent(AirStickers);
					SpotObj.GetComponent<TagController>().TagID = Global.Spots[ii].SpotID;
					SpotObj.transform.Find("Parent/Name").GetComponent<UILabel>().text = Global.Spots[ii].SpotName_kr;
					SpotObj.SetActive(false);

					Global.Spots[ii].SpotObject = SpotObj;

					//				Global.Spots[i].SpotObject.transform.Find("Parent/tagline").GetComponent<UISprite>().height = Global.temp_length[temp];

					double distance = OnlineMapsUtils.DistanceBetweenPointsD_GRS(Global.Spots[ii].SpotPosition.lat, Global.Spots[ii].SpotPosition.lng, Global.currPosition.lat, Global.currPosition.lng);
					double angle = OnlineMapsUtils.bearingP1toP2(Global.currPosition.lat, Global.currPosition.lng, Global.Spots[ii].SpotPosition.lat, Global.Spots[ii].SpotPosition.lng);
					float posx = (float)(Math.Sin(angle * OnlineMapsUtils.Deg2Rad) * distance);
					float posz = (float)(Math.Cos(angle * OnlineMapsUtils.Deg2Rad) * distance);
					float posy = (float)Global.Spots[ii].SpotHeight - Global.camera_height;

					if(float.IsNaN(posx))
						posx = 0;
					if(float.IsNaN(posy))
						posy = 0;
					if(float.IsNaN(posz))
						posz = 0;
					SpotObj.transform.localPosition = new Vector3(posx, posy, posz);
					SpotObj.transform.localScale = new Vector3(1f, 1f, 1f);

					Vector3 sticker2DPos = VirtualCamera.WorldToScreenPoint(SpotObj.transform.localPosition);
					SpotObj.transform.localPosition = Global.disortPoint(sticker2DPos) - new Vector3(Screen.width / 2, Screen.height / 2, 0);
					//					Debug.Log("posx" + ii + " = " + sticker2DPos.x);
					//					Debug.Log("posz" + ii + " = " + posz);
					//					SpotObj.transform.localPosition = Global.distort_position(sticker2DPos) - new Vector3(Screen.width / 2, Screen.height / 2, 0);

					//SpotObj.transform.localPosition = ChangeNGUIPoint(SpotObj.transform.position);

					//					Vector3 targetpos = new Vector3(0, posy, 0);
					//					Vector3 relativpos = targetpos - SpotObj.transform.position;
					//					Quaternion rotation = Quaternion.LookRotation(relativpos, Vector3.up);
					//					SpotObj.transform.rotation = rotation;

					//건물스티커 위치 보정
					Global.Spots[ii].SpotObject.SetActive(true);
                 
					if (ii != 0 && ii <= 12)
					{

                        if(Global.Spots[ii].SpotID=="251")
                        {
                            SpotObj.transform.localPosition=new Vector3(SpotObj.transform.localPosition.x, SpotObj.transform.localPosition.y+600, SpotObj.transform.localPosition.z) ;
                        }
                        else
                        {
						   GameObject.Find("UI Root/TagCollections/" + Global.Spots[ii].SpotObject.transform.name + "/Parent/background").SetActive(false);
						   GameObject.Find("UI Root/TagCollections/" + Global.Spots[ii].SpotObject.transform.name + "/Parent/background" + ii).SetActive(true);
                        }

                    }
					else if (ii == 19)
					{
						//제1관제탑
						GameObject.Find("UI Root/TagCollections/" + Global.Spots[ii].SpotObject.transform.name + "/Parent/background").SetActive(false);
						GameObject.Find("UI Root/TagCollections/" + Global.Spots[ii].SpotObject.transform.name + "/Parent/background1").SetActive(true);
					}

					StartCoroutine(stickerEffect(SpotObj, ii));
				}catch(Exception ex) {
					Debug.Log ("----Building Popup Error : " + ii + " : " + ex.Message);
				}
			}
		}
		airSpotBtn.enabled = true;
		if(Global.Spots != null)
			Global.BSpots_count = Global.Spots.Count;
	}

	public Texture air_tex1;
	public Texture air_tex2;
	public Texture air_tex3;
	public Texture air_tex4;

	private SpotInformation load_plane_information(SpotInformation aSpot, JsonData data){
		try{
			if (Global.JsonDataContainsKey(data, "arrivalOrDeparture") && data["arrivalOrDeparture"].ToString() == "A") { 
				aSpot.isArrival = true;
			}
			else{
				aSpot.isArrival = false;
			}
			if(Global.JsonDataContainsKey(data, "gateArrivalNumber"))
			{
				aSpot.gate = data["gateArrivalNumber"].ToString();
			}
			if(Global.JsonDataContainsKey(data, "aircraftRegistrationNumber"))
			{
				aSpot.airbus = data["aircraftRegistrationNumber"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"publicIdentifier"))
			{
				aSpot.passenger = data["publicIdentifier"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"flyngNeedHm"))
			{
				aSpot.need_time = data["flyngNeedHm"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalOrDeparture"))
			{
				aSpot.PlaneTarget = data["arrivalOrDeparture"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"messageType"))
			{
				aSpot.MessageType = data["messageType"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"departureFullName"))
			{
				aSpot.SpotStartID = data["departureFullName"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"departureCounty"))
			{
				aSpot.SpotStartCountry = data["departureCounty"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"departureCity"))
			{
				aSpot.SpotStartCity = data["departureCity"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"departureFullName"))
			{
				aSpot.SpotStartCityEn = data["departureFullName"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"departureCcity"))
			{
				aSpot.SpotStartCityCn = data["departureCcity"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"departureJcity"))
			{
				aSpot.SpotStartCityJp = data["departureJcity"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"departTempTime"))
			{
				aSpot.SpotStartDate = data["departTempTime"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"departureMintemp") && Global.JsonDataContainsKey(data, "departureMaxtemp"))
			{
				aSpot.SpotStartWhetherValue = data["departureMintemp"].ToString() + "/" + data["departureMaxtemp"].ToString() + "°";
			}
			if (Global.JsonDataContainsKey(data,"departureWeatherCode"))
			{
				aSpot.SpotStartWhetherIcon = data["departureWeatherCode"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalFullName"))
			{
				aSpot.SpotEndID = data["arrivalFullName"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalCounty"))
			{
				aSpot.SpotEndCountry = data["arrivalCounty"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalCity"))
			{
				aSpot.SpotEndCity = data["arrivalCity"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalFullName"))
			{
				aSpot.SpotEndCityEn = data["arrivalFullName"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalCcity"))
			{
				aSpot.SpotEndCityCn = data["arrivalCcity"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalJcity"))
			{
				aSpot.SpotEndCityJp = data["arrivalJcity"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalTempTime"))
			{
				aSpot.SpotEndDate = data["arrivalTempTime"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"arrivalMintemp") && Global.JsonDataContainsKey(data, "arrivalMaxtemp"))
			{
				aSpot.SpotEndWhetherValue = data["arrivalMintemp"].ToString() + "/" + data["arrivalMaxtemp"].ToString() + "°";
			}
			if (Global.JsonDataContainsKey(data,"arrivalWeatherCode"))
			{
				aSpot.SpotEndWhetherIcon = data["arrivalWeatherCode"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"gateArrivalNumber"))
			{
				aSpot.SpotGateText = data["gateArrivalNumber"].ToString();
			}
			if (Global.JsonDataContainsKey(data,"speed"))
			{
				aSpot.SpotWindSpeed = data["speed"].ToString();

			}
			if (Global.JsonDataContainsKey(data,"flightCarrier"))
			{
				aSpot.FlightCarrier = data["flightCarrier"].ToString();
			}

		}
		catch (Exception ex)
		{
			var line = 000;
			Debug.LogException(ex, this);
			Debug.Log("error while parsing json. line "+ line + ex.Message);
		}
		return aSpot;
	}

	private IEnumerator LoadValues(JsonData dataArray){
		int nCount = dataArray.Count;
		for (int i = 0; i < nCount; i++) {
			string airPlaneName = "";
			string airPlaneNumber = "";
			try{
				airPlaneName = dataArray[i]["publicIdentifier"].ToString();
				airPlaneNumber = dataArray[i]["publicIdentifier"].ToString();
			}catch(Exception){
			}

			if (dataArray [i] ["arrivalCity"].ToString () == "서울/ 인천") {
				Global.maxTemp = dataArray [i] ["arrivalMaxtemp"].ToString ();
				Global.minTemp = dataArray [i] ["arrivalMintemp"].ToString ();
			}
			else if(dataArray [i] ["departureCity"].ToString () == "서울/ 인천"){
				Global.maxTemp = dataArray [i] ["departureMaxtemp"].ToString ();
				Global.minTemp = dataArray [i] ["departureMintemp"].ToString ();
			}

			loadTemperature ();

			SpotInformation aSpot = new SpotInformation("", "", 0, 0, 0);
			try{
				aSpot = new SpotInformation(1, dataArray[i]["publicIdentifier"].ToString(), airPlaneNumber,
					airPlaneName,
					double.Parse(dataArray[i]["latitude"].ToString()),
					double.Parse(dataArray[i]["longitude"].ToString()),
					double.Parse(dataArray[i]["altitude"].ToString()));
			}catch(Exception){
			}

			if(aSpot.SpotType == 1) //비행기인 경우
			{
				aSpot = load_plane_information(aSpot, dataArray[i]);
				Global.Spots.Add(aSpot);
			}
			//			Global.ObjonMaps.Add(null);
			//			Global.ObjonMaps[i] = null;
		}
		yield return null;
	}

	private void loadTemperature()
	{
		GameObject.Find ("UI Root/whether/txt1").GetComponent<UILabel> ().text = Global.minTemp + "~" + Global.maxTemp + "°";
	}

	IEnumerator LoadMainWethereIcon(JsonData dataArray)
	{
		WWW www = new WWW(Global.Domain + "/img/W" + dataArray[0]["departureWeatherCode"].ToString() + ".png");
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Debug.Log (www.error);
		else {
			try{
				Texture2D texTmp = new Texture2D(64, 64, TextureFormat.ARGB32, false);
				www.LoadImageIntoTexture(texTmp);
				GameObject.Find("UI Root/whether/whether_icon/bg").GetComponent<UITexture>().mainTexture = texTmp;
			}
			catch(Exception) {
			}
		}
	}

	IEnumerator MovingThread(int stickerID, Vector3 startPos, Vector3 endPos, float time)
	{
		float deltaTime = time / 15;
		int callingCount = 0;
		float delta_x = (endPos.x - startPos.x) / 15;
		float delta_y = (endPos.y - startPos.y) / 15;
		if (delta_x != 0.0f && delta_y != 0.0f) {
			while (callingCount < 15)
			{
				callingCount++;
				if (Global.Spots.Count > stickerID &&  Global.Spots[stickerID].SpotOnMapObject != null) {
					Global.Spots[stickerID].SpotOnMapObject.transform.localPosition = new Vector3 (Global.Spots[stickerID].SpotOnMapObject.transform.localPosition.x + delta_x,
						Global.Spots[stickerID].SpotOnMapObject.transform.localPosition.y + delta_y, 0);
				} else {
					StopCoroutine ("MovingThread");
				}
				yield return new WaitForSeconds(deltaTime);
			}
		}
    }

    private int chk_distance_tower(GameObject airSpot, float distance)
    {
        if (distance > Global.distance_tower)
        {
            float val = distance- Global.distance_tower  ;
            //airSpot.transform.GetChild(0).GetChild(4).GetComponent<UITexture>().SetDimensions(20, 20);
            airSpot.transform.GetChild(0).GetChild(4).GetComponent<UITexture>().SetDimensions(65, 65);
            //   airSpot.transform.GetChild(0).GetChild(4).GetComponent<UITexture>().mainTexture = air_tex2;
          //  if (val > 300) airSpot.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
          //  else
            if (val > 600) airSpot.transform.localScale = new Vector3(0.4f, 0.4f, 04f);
            else airSpot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
           // airSpot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //airSpot.transform.GetChild(0).GetChild(4).GetComponent<UITexture>().mainTexture = air_tex1;
           // return 1;
        }
        else
        {
            float val=Global.distance_tower- distance  ;
            airSpot.transform.GetChild(0).GetChild(4).GetComponent<UITexture>().SetDimensions(65, 65);
          //  airSpot.transform.GetChild(0).GetChild(4).GetComponent<UITexture>().mainTexture = air_tex2;
            if(val > 800)
            {
                airSpot.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (val > 600) airSpot.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            else if (val > 400) airSpot.transform.localScale = new Vector3(0.7f, 0.7f, 07f);
            else   airSpot.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            //  return 2;
        }
        return 2;
    }

	private void UpdateSpotTexture(GameObject airSpot, int index)
	{
		int i = index;
		int tex_index = 0;

		float distance = (float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS(Global.Spots[i].SpotPosition.lat, Global.Spots[i].SpotPosition.lng, Global.currPosition.lat, Global.currPosition.lng));
		float d_angle = (float)(OnlineMapsUtils.bearingP1toP2(Global.currPosition.lat,Global.currPosition.lng,Global.Spots[i].SpotPosition.lat,Global.Spots[i].SpotPosition.lng  ));
		distance = Mathf.Abs(distance * Mathf.Cos ((d_angle - Global.camera3_radio) * Mathf.PI / 180.0f));
        tex_index = chk_distance_tower(airSpot, distance);
        Global.Spots[i].SpotObject = airSpot;
		Global.Spots[i].SpotObject.SetActive(true);
		Global.Spots[i].SpotObject.GetComponent<TagController>().TagID = Global.Spots[i].SpotID;
		//StartCoroutine(LoadFlightCarrierImg(i));
		setAirTexture(i, tex_index);
	}

	private Vector3 ChangeNGUIPoint(Vector3 target)
	{
		Camera worldCam = VirtualCamera;
		Camera guiCam = NGUITools.FindCameraForLayer(AirSpot.layer);
		Vector3 vp_pos = worldCam.WorldToViewportPoint(target);
		//  Debug.Log("vp_pos = " + vp_pos);
		Vector3 pos = new Vector3(-1000f, -1000f, -1000f);
		Vector3 retPoint = Vector3.zero;
		if (vp_pos.normalized.z > 0)
		{
			pos = guiCam.ViewportToWorldPoint(vp_pos);
			//Z는 0으로...
			pos.z = 0;
			retPoint = new Vector3(pos.x * 1080 , pos.y * 1920, pos.z);

		}
		return retPoint;
	}

	IEnumerator Moveliner(GameObject target, Vector3 startpos, Vector3 endpos)
	{
		if (target != null)
		{
			float deltatime = 0;
			for (int i = 0; i < 20; i++)
			{
				deltatime += 0.05f;
				if (target != null)
				{
					target.transform.localPosition = Vector3.Lerp(startpos, endpos, deltatime);
				}
				//Vector3 deltaDist = Vector3.Lerp(startpos, endpos, deltatime) - startpos;
				//target.transform.localPosition = endpos + deltaDist;
				yield return new WaitForSeconds(0.05f);
			}
		}
	}

	public Texture airTexArrive;
	public Texture airTexLeave;

	void setAirTexture(int stickerID,int tex_index)
	{
		if (Global.Spots[stickerID].SpotObject.transform.Find("AirTagPrefab/logo") != null &&
			Global.Spots[stickerID].SpotObject.transform.Find("AirTagPrefab/logo").GetComponent<UITexture>() != null)
		{
			//if(Global.Spots[])
			if (Global.Spots [stickerID].isArrival) {
				if(tex_index == 1)
					Global.Spots [stickerID].SpotObject.transform.Find ("AirTagPrefab/logo").GetComponent<UITexture> ().mainTexture = air_tex1;
				else if(tex_index == 2)
					Global.Spots [stickerID].SpotObject.transform.Find ("AirTagPrefab/logo").GetComponent<UITexture> ().mainTexture = air_tex2;
			} else {
				if(tex_index == 1)
					Global.Spots [stickerID].SpotObject.transform.Find ("AirTagPrefab/logo").GetComponent<UITexture> ().mainTexture = air_tex3;
				else if(tex_index == 2)
					Global.Spots [stickerID].SpotObject.transform.Find ("AirTagPrefab/logo").GetComponent<UITexture> ().mainTexture = air_tex4;
			}
		}
	}

	IEnumerator LoadFlightCarrierImg(int stickerID)
	{
		if (Global.Spots[stickerID] != null && Global.Spots[stickerID].FlightCarrier != null && Global.Spots[stickerID].FlightCarrier.Length > 0)
		{
			WWW www = null;
			www = new WWW(Global.Domain + "/img/AIR_LOGO/" + Global.Spots[stickerID].FlightCarrier + ".png");
			yield return www;
			if(www.error == null)
			{
				if (Global.Spots[stickerID].SpotObject.transform.Find("AirTagPrefab/logo") != null &&
					Global.Spots[stickerID].SpotObject.transform.Find("AirTagPrefab/logo").GetComponent<UITexture>() != null)
				{
					Texture2D texTmp = new Texture2D(64, 64, TextureFormat.ARGB32, false);
					www.LoadImageIntoTexture(texTmp);
					Global.Spots[stickerID].SpotObject.transform.Find("AirTagPrefab/logo").GetComponent<UITexture>().mainTexture = texTmp;
				}
			}
		}
	}

	private void loadTowerDistance(){
        	float distance_comp = (float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS(37.460910f, 126.440480f, Global.currPosition.lat, Global.currPosition.lng));
        	float dis_angle = (float)(OnlineMapsUtils.bearingP1toP2(Global.currPosition.lat, Global.currPosition.lng, 37.460910f, 126.440480f));
        	Global.distance_tower = Mathf.Abs(distance_comp * Mathf.Cos ((dis_angle - Global.camera3_radio) * Mathf.PI / 180.0f));
    }

	IEnumerator stickerEffect(GameObject obj, int i)
	{
		float time = i * 0.1f;
		yield return new WaitForSeconds (time);
		float delta = 0.0f;
		while (true) {
			while (delta < 5.0f) {
				delta += 0.5f;
				if(obj!=null && obj.transform != null) obj.transform.position += new Vector3(0.0f, 0.002f, 0.0f);
				yield return new WaitForSeconds (0.1f);
			}
			while (delta > 0.0f) {
				delta -= 0.5f;
				if (obj != null && obj.transform!=null) obj.transform.position -= new Vector3(0.0f, 0.002f, 0.0f);
				yield return new WaitForSeconds (0.1f);
			}
		}
	}

	private float getX(float x1, float y1, float x2, float y2){
		float distance = (float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS(x1, y1, x2, y2));
		float angle = (float)(OnlineMapsUtils.bearingP1toP2(x2, y2, x1, y1));
		float l = Mathf.Sin((float)(angle * Math.PI / 180.0f)) * distance;
		return l;
	}

	private float getY(float x1, float y1, float x2, float y2){
		float distance = (float)(OnlineMapsUtils.DistanceBetweenPointsD_GRS(x1, y1, x2, y2));
		float angle = (float)(OnlineMapsUtils.bearingP1toP2(x2, y2, x1, y1));
		float l = Mathf.Cos((float)(angle * Math.PI / 180.0f)) * distance;
		return l;
	}

	private float getHeight(float s, float c)
	{
		float h = (2.0f * s) / c;
		return h;
	}

	private float getArea(float a, float b, float c)
	{
		try{
			float p = (a + b + c) / 2.0f;
			float s = Mathf.Sqrt (p * (p - a) * (p - b) * (p - c));
			return s;
		}catch(Exception){
			return 0.0f;
		}
	}
}
