using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

public class UIManager : MonoBehaviour {
	public GameObject SleepingWnd;

	//Help Wnd
	public GameObject HelpWnd;
	int isMovingHelpWnd = 0;        //1 : move to left, 2 : move to right
	private float sumTime = 0.0f, actionTime = 0.3f;
	private Vector3 HelpWndLastPos = new Vector3(0, 0, 0);

	//Map Wnd
	public GameObject SmallMapObj;
	public GameObject BigMapObj;

	//Tag Variables
	public GameObject TagCollectionObject;
	public GameObject TagPrefab;
	public GameObject GatePopupPrefab;
	public GameObject PlanePopupPrefab;
	public GameObject TerminalPopupPrefab;

	// Use this for initialization
	void Start () {
		InitialWindows();
		LocalizationManager.CurrentLanguage = "Korean";
        
	}

	public void InitialWindows()
	{
		Global.LastUsingTime = System.DateTime.Now;
	}

	public GameObject bg;
	// Update is called once per frame
	void Update () {
		if(Global.is_showing_popup){
			bg.SetActive(true);
		}
		else{
			bg.SetActive(false);
		}
		if ((System.DateTime.Now - Global.LastUsingTime).TotalSeconds > 300)
		{
			//            SleepingWnd.SetActive(true);
		}else
		{
			//            SleepingWnd.SetActive(false);
		}

		if (Input.touchCount > 0)
		{
			Global.LastUsingTime = System.DateTime.Now;
		}
		if(GameObject.Find("UI Root/MapWnd/BigMap/radar") !=null)
		{
			GameObject.Find("UI Root/MapWnd/BigMap/radar").transform.Rotate(0.0f, 0.0f, -1.0f);
		}

		//Lerp Help Wnd
		if (isMovingHelpWnd == 1 && HelpWnd.transform.localPosition.x < 0)
		{
			sumTime += Time.deltaTime;
			float fracJourney = sumTime / actionTime;
			HelpWnd.transform.localPosition = Vector3.Lerp(new Vector3(HelpWndLastPos.x, HelpWnd.transform.localPosition.y, 0.0f), new Vector3(HelpWndLastPos.x + 1080.0f, HelpWnd.transform.localPosition.y, 0.0f), fracJourney);
			if (fracJourney >= 1.0f)
			{
				sumTime = 0.0f;
				isMovingHelpWnd = 0;
			}
		}else if(isMovingHelpWnd == 2 && HelpWnd.transform.localPosition.x > -3240)
		{
			sumTime += Time.deltaTime;
			float fracJourney = sumTime / actionTime;
			HelpWnd.transform.localPosition = Vector3.Lerp(new Vector3(HelpWndLastPos.x, HelpWnd.transform.localPosition.y, 0.0f), new Vector3(HelpWndLastPos.x - 1080.0f, HelpWnd.transform.localPosition.y, 0.0f), fracJourney);
			if (fracJourney >= 1.0f)
			{
				sumTime = 0.0f;
				isMovingHelpWnd = 0;
			}
		}

		try{
			if(Global.is_showing_popup)
			{
				Global.count_time += Time.deltaTime;
				if (Global.count_time >= 30.0f) {
					//close popup
					Global.count_time = 0.0f;
					Debug.Log ("Time Init = " + Global.count_time);
					HideTagInfoWnd();
				}
			}
			else
			{
				Global.count_time = 0.0f;
			}
		}
		catch(Exception) {
		}

        System.DateTime theTime = System.DateTime.Now;
        string time = theTime.Hour + ":" + theTime.Minute + ":" + theTime.Second;
        current_time.text = time;
	}

	#region Bottom Buttons
	public Texture kor;
	public Texture sel_kor;
	public Texture eng;
	public Texture sel_eng;
	public Texture chi;
	public Texture sel_chi;
	public Texture jp;
	public Texture sel_jp;

    public UILabel current_time;
	public void OnKoreanBtnClick()
	{
		//        HideAllInfoWnds();
		Global.LanguageCode = 1;
		StartCoroutine(ReLoadBuildingImages());
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnKorean/Background").GetComponent<UITexture> ().mainTexture = sel_kor;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnEnglish/Background").GetComponent<UITexture> ().mainTexture = eng;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnChinese/Background").GetComponent<UITexture> ().mainTexture = chi;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnJapanese/Background").GetComponent<UITexture> ().mainTexture = jp;

		arrivalCityLbl.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotStartCity;
		departCityLbl.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotEndCity;
		arrivalCityLbl1.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotStartCity;
		departCityLbl1.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotEndCity;

		for(int i=0;i<Grid.transform.childCount;i++)
		{
			GameObject GridTmp = Grid.transform.GetChild(i).gameObject;
			GridTmp.transform.GetChild(4).transform.GetComponent<UILabel>().text =
				Global.Spots[Global.sel_spot1].GateInfos[i].DepartCity;
			GridTmp.transform.GetChild(5).transform.GetComponent<UILabel>().text =
				Global.Spots[Global.sel_spot1].GateInfos[i].ArrivalCity;
		}

		string name_data = Global.Spots[Global.sel_spot1].SpotName_kr;
		name_data = name_data.Substring(name_data.Length - 3, 3);
		name_data = name_arr[Global.LanguageCode - 1] + name_data;
		GateNameInfo.GetComponent<UILabel>().text = name_data;

		StartCoroutine(LoadPopupImage(Global.sel_spot2));
	}

	public GameObject GateNameInfo;
	public GameObject departCityLbl;
	public GameObject arrivalCityLbl;
	public GameObject departCityLbl1;
	public GameObject arrivalCityLbl1;
	public void OnEnglishBtnClick()
	{
		//        HideAllInfoWnds();
		Global.LanguageCode = 2;
		StartCoroutine(ReLoadBuildingImages());
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnKorean/Background").GetComponent<UITexture> ().mainTexture = kor;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnEnglish/Background").GetComponent<UITexture> ().mainTexture = sel_eng;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnChinese/Background").GetComponent<UITexture> ().mainTexture = chi;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnJapanese/Background").GetComponent<UITexture> ().mainTexture = jp;

		arrivalCityLbl.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotStartCityEn;
		departCityLbl.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotEndCityEn;
		arrivalCityLbl1.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotStartCityEn;
		departCityLbl1.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotEndCityEn;

		for (int i = 0; i < Grid.transform.childCount; i++)
		{
			GameObject GridTmp = Grid.transform.GetChild(i).gameObject;
			GridTmp.transform.GetChild(4).transform.GetComponent<UILabel>().text =
				Global.Spots[Global.sel_spot1].GateInfos[i].DepartCityEn;
			GridTmp.transform.GetChild(5).transform.GetComponent<UILabel>().text =
				Global.Spots[Global.sel_spot1].GateInfos[i].ArrivalCityEn;
		}

		string name_data = Global.Spots[Global.sel_spot1].SpotName_kr;
		name_data = name_data.Substring(name_data.Length - 3, 3);
		name_data = name_arr[Global.LanguageCode - 1] + name_data;
		GateNameInfo.GetComponent<UILabel>().text = name_data;

		StartCoroutine(LoadPopupImage(Global.sel_spot2));
	}

	public void OnChineseBtnClick()
	{
		//        HideAllInfoWnds();
		Global.LanguageCode = 3;
		StartCoroutine(ReLoadBuildingImages());
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnKorean/Background").GetComponent<UITexture> ().mainTexture = kor;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnEnglish/Background").GetComponent<UITexture> ().mainTexture = eng;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnChinese/Background").GetComponent<UITexture> ().mainTexture = sel_chi;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnJapanese/Background").GetComponent<UITexture> ().mainTexture = jp;

		arrivalCityLbl.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotStartCityCn;
		departCityLbl.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotEndCityCn;
		arrivalCityLbl1.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotStartCityCn;
		departCityLbl1.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotEndCityCn;

		for (int i = 0; i < Grid.transform.childCount; i++)
		{
			GameObject GridTmp = Grid.transform.GetChild(i).gameObject;
			GridTmp.transform.GetChild(4).transform.GetComponent<UILabel>().text =
				Global.Spots[Global.sel_spot1].GateInfos[i].DepartCityCn;
			GridTmp.transform.GetChild(5).transform.GetComponent<UILabel>().text =
				Global.Spots[Global.sel_spot1].GateInfos[i].ArrivalCityCn;
		}

		string name_data = Global.Spots[Global.sel_spot1].SpotName_kr;
		name_data = name_data.Substring(name_data.Length - 3, 3);
		name_data = name_arr[Global.LanguageCode - 1] + name_data;
		GateNameInfo.GetComponent<UILabel>().text = name_data;

		StartCoroutine(LoadPopupImage(Global.sel_spot2));
	}

	public void OnJapaneseBtnClick()
	{
		//        HideAllInfoWnds();
		Global.LanguageCode = 4;
		StartCoroutine(ReLoadBuildingImages());
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnKorean/Background").GetComponent<UITexture> ().mainTexture = kor;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnEnglish/Background").GetComponent<UITexture> ().mainTexture = eng;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnChinese/Background").GetComponent<UITexture> ().mainTexture = chi;
		GameObject.Find ("UI Root/BottomMenu/LanguageMenu/btnJapanese/Background").GetComponent<UITexture> ().mainTexture = sel_jp;

		arrivalCityLbl.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotStartCityJp;
		departCityLbl.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotEndCityJp;
		arrivalCityLbl1.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotStartCityJp;
		departCityLbl1.GetComponent<UILabel>().text =
			Global.Spots[Global.sel_spot].SpotEndCityJp;

		for (int i = 0; i < Grid.transform.childCount; i++)
		{
			GameObject GridTmp = Grid.transform.GetChild(i).gameObject;
			GridTmp.transform.GetChild(4).transform.GetComponent<UILabel>().text =
				Global.Spots[Global.sel_spot1].GateInfos[i].DepartCityJp;
			GridTmp.transform.GetChild(5).transform.GetComponent<UILabel>().text =
				Global.Spots[Global.sel_spot1].GateInfos[i].ArrivalCityJp;
		}

		string name_data = Global.Spots[Global.sel_spot1].SpotName_kr;
		name_data = name_data.Substring(name_data.Length - 3, 3);
		name_data = name_arr[Global.LanguageCode - 1] + name_data;
		GateNameInfo.GetComponent<UILabel>().text = name_data;

		StartCoroutine(LoadPopupImage(Global.sel_spot2));
	}

	public void HideAllInfoWnds()
	{
		PlanePopupPrefab.SetActive (false);
		GatePopupPrefab.SetActive (false);
		TerminalPopupPrefab.SetActive (false);
	}

	public void ShowPlanes(bool sw)
	{
		if (Global.Spots == null) return;
		for (int i = 0; i < Global.Spots.Count; i++)
		{
			if (Global.Spots[i] != null && Global.Spots[i].SpotObject != null  && Global.Spots[i].SpotType == 1)
			{
				Global.Spots[i].SpotObject.SetActive(sw);
			}
		}
	}
	public void ShowBuildings(bool sw)
	{
		if (Global.Spots == null) return;
		for (int i = 0; i < Global.Spots.Count; i++)
		{
			if (Global.Spots[i] != null  && Global.Spots[i].SpotObject!=null && Global.Spots[i].SpotType != 1)
			{
				/*if (Global.Spots[i].SpotObject.name == "Sport_6" || Global.Spots[i].SpotObject.name == "Sport_7" || Global.Spots[i].SpotObject.name == "Sport_20")
                    continue;*/
				Global.Spots[i].SpotObject.SetActive(sw);

			}
		}
	}

	public GameObject building_press;
	public GameObject building_disable;
	public GameObject plane_press;
	public GameObject plane_disable;

	public void OnBuildingBtnClick()
	{
		Global.selBuilding = 2;
		ShowPlanes(false);
		ShowBuildings(true);
		building_press.SetActive(true);
		building_disable.SetActive(false);
		plane_press.SetActive(false);
		plane_disable.SetActive(true);
	}

	public void OnPlaneBtnClick()
	{
		Global.selBuilding = 1;
		ShowPlanes(true);
		ShowBuildings(false);
		building_press.SetActive(false);
		building_disable.SetActive(true);
		plane_press.SetActive(true);
		plane_disable.SetActive(false);
	}


	public void OnHelpBtnClick()
	{
		GameObject.Find("UI Root/BottomMenu/btnHelp/Text").GetComponent<UILabel>().color = new Color((float)207 /(float)255, (float)148/ (float)255, (float)69/ (float)255);
		HelpWnd.SetActive(true);
	}

	#endregion

	#region Help Button

	IEnumerator help_leftEffect(int i)
	{
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/Page/btnLeft/Background").SetActive (false);
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/Page/btnLeft/Background1").SetActive (true);
		yield return new WaitForSeconds (0.15f);
		isMovingHelpWnd = 1;
		HelpWndLastPos = HelpWnd.transform.localPosition;
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/Page/btnLeft/Background").SetActive (true);
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/Page/btnLeft/Background1").SetActive (false);
	}

	public void OnHelpLeftBtn()
	{
		StartCoroutine (help_leftEffect (1));
	}

	public void OnHelpLeftBtn1()
	{
		StartCoroutine (help_leftEffect (2));
	}

	public void OnHelpLeftBtn2()
	{
		StartCoroutine (help_leftEffect (3));
	}

	IEnumerator help_rightEffect(int i)
	{
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/Page/btnRight/Background").SetActive (false);
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/Page/btnRight/Background1").SetActive (true);
		yield return new WaitForSeconds (0.15f);
		isMovingHelpWnd = 2;
		HelpWndLastPos = HelpWnd.transform.localPosition;
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/Page/btnRight/Background").SetActive (true);
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/Page/btnRight/Background1").SetActive (false);
	}

	public void OnHelpRightBtn()
	{
		StartCoroutine (help_rightEffect (1));
	}

	public void OnHelpRightBtn1()
	{
		StartCoroutine (help_rightEffect (2));
	}

	public void OnHelpRightBtn2()
	{
		StartCoroutine (help_rightEffect (3));
	}

	IEnumerator help_closeEffect(int i)
	{
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/closeHelpBtn/Background").SetActive (false);
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/closeHelpBtn/Background1").SetActive (true);
		yield return new WaitForSeconds (0.15f);
		GameObject.Find("UI Root/BottomMenu/btnHelp/Text").GetComponent<UILabel>().color = new Color(1,1,1);
		HelpWnd.SetActive(false);
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/closeHelpBtn/Background").SetActive (true);
		GameObject.Find ("UI Root/HelpWnd/HelpWnd" + i + "/closeHelpBtn/Background1").SetActive (false);
	}

	public void OnHelpCloseBtn()
	{
		if ((HelpWnd.transform.localPosition.x % 1080) != 0)
			return;
		StartCoroutine(help_closeEffect (1));
	}

	public void OnHelpCloseBtn1()
	{
		if ((HelpWnd.transform.localPosition.x % 1080) != 0)
			return;
		StartCoroutine(help_closeEffect (2));
		GameObject.Find("UI Root/BottomMenu/btnHelp/Text").GetComponent<UILabel>().color = new Color(1,1,1);
		HelpWnd.SetActive(false);
	}

	public void OnHelpCloseBtn2()
	{
		if ((HelpWnd.transform.localPosition.x % 1080) != 0)
			return;
		StartCoroutine(help_closeEffect (3));
		GameObject.Find("UI Root/BottomMenu/btnHelp/Text").GetComponent<UILabel>().color = new Color(1,1,1);
		HelpWnd.SetActive(false);
	}

	#endregion

	#region Map Btns
	IEnumerator effectZoomMap()
	{
		GameObject.Find ("UI Root/MapWnd/SmallMap/MapImage").SetActive (false);
		//		GameObject.Find ("UI Root/MapWnd/SmallMap/MapImage1").SetActive (true);
		yield return new WaitForSeconds (0.2f);
		GameObject.Find ("UI Root/MapWnd/SmallMap/MapImage").SetActive (true);
		//		GameObject.Find ("UI Root/MapWnd/SmallMap/MapImage1").SetActive (false);
		SmallMapObj.SetActive(false);
		BigMapObj.SetActive(true);
	}

	public void ZoopInMap()
	{
		StartCoroutine (effectZoomMap ());
	}

	public void ZoomOutMap()
	{
		SmallMapObj.SetActive(true);
		BigMapObj.SetActive(false);
	}
	#endregion

	#region Load Tag Windows

	public GameObject controller;
	public void LoadTagWindow(int stickerID)
	{
		switch (Global.Spots[stickerID].SpotType)
		{
		case 1:                     //AirPlane Popup
			//controller.SetActive(true);
			PlanePopupPrefab.SetActive (true);
			PlanePopupPrefab.GetComponent<TagController>().TagID = Global.Spots[stickerID].SpotID;
			GameObject planePopup;
			if (Global.Spots [stickerID].isArrival) {
				PlanePopupPrefab.transform.Find ("PlaneInfo/arrival").gameObject.SetActive (true);
				PlanePopupPrefab.transform.Find ("PlaneInfo/departure").gameObject.SetActive (false);
				planePopup = PlanePopupPrefab.transform.Find ("PlaneInfo/arrival").gameObject;
				StartCoroutine (LoadAirIcon (stickerID, true));
				StartCoroutine (LoadWethereIcons (stickerID, true));
				planePopup.transform.Find ("CenterWnd/StartValue").GetComponent<UILabel> ().text = Global.Spots [stickerID].SpotStartWhetherValue;
			} else {
				PlanePopupPrefab.transform.Find ("PlaneInfo/arrival").gameObject.SetActive (false);
				PlanePopupPrefab.transform.Find ("PlaneInfo/departure").gameObject.SetActive (true);
				planePopup = PlanePopupPrefab.transform.Find ("PlaneInfo/departure").gameObject;
				StartCoroutine (LoadAirIcon (stickerID, false));
				StartCoroutine (LoadWethereIcons (stickerID, false));
				planePopup.transform.Find ("CenterWnd/StartValue").GetComponent<UILabel> ().text = Global.Spots [stickerID].SpotEndWhetherValue;
			}
			//정보 표시
			for (int ii = 1; ii <= 4; ii++) {
				if (ii == Global.LanguageCode) {
					planePopup.transform.Find ("background" + ii.ToString() ).gameObject.SetActive (true);
				} else {
					planePopup.transform.Find ("background" + ii.ToString ()).gameObject.SetActive (false);
				}
			}

			planePopup.transform.Find ("Passenger/p1").GetComponent<UILabel> ().text = Global.Spots [stickerID].passenger;

            if (planePopup.transform.Find("Passenger/p2") != null)
            {
                planePopup.transform.Find("Passenger/p2").GetComponent<UILabel>().text = Global.Spots[stickerID].SpotGateText;
            }

            planePopup.transform.Find("CenterWnd/AirName").GetComponent<UILabel>().text = Global.Spots[stickerID].gate;
            planePopup.transform.Find("CenterWnd/AirBusName").GetComponent<UILabel>().text = "Airbus "+Global.Spots[stickerID].airbus;
            
            //Global.Spots[stickerID].SpotID;
			Global.sel_spot = stickerID;
			if(Global.LanguageCode == 1)
				planePopup.transform.Find ("CenterWnd/StartPlace").GetComponent<UILabel> ().text = Global.Spots [stickerID].SpotStartCity ;
			else if (Global.LanguageCode == 2)
				planePopup.transform.Find("CenterWnd/StartPlace").GetComponent<UILabel>().text = Global.Spots[stickerID].SpotStartCityEn;
			else if (Global.LanguageCode == 3)
				planePopup.transform.Find("CenterWnd/StartPlace").GetComponent<UILabel>().text = Global.Spots[stickerID].SpotStartCityCn;
			else if (Global.LanguageCode == 4)
				planePopup.transform.Find("CenterWnd/StartPlace").GetComponent<UILabel>().text = Global.Spots[stickerID].SpotStartCityJp;

			if(Global.Spots [stickerID].SpotStartDate != null){
				planePopup.transform.Find ("CenterWnd/StartDate").GetComponent<UILabel> ().text = GetStyledDate (Global.Spots [stickerID].SpotStartDate);
				planePopup.transform.Find ("CenterWnd/StartTime").GetComponent<UILabel> ().text = GetStyledTime1 (Global.Spots [stickerID].SpotStartDate);
			}

			if (Global.LanguageCode == 1)
				planePopup.transform.Find ("CenterWnd/EndPlace").GetComponent<UILabel> ().text = Global.Spots [stickerID].SpotEndCity ;
			else if (Global.LanguageCode == 2)
				planePopup.transform.Find("CenterWnd/EndPlace").GetComponent<UILabel>().text = Global.Spots[stickerID].SpotEndCityEn;
			else if (Global.LanguageCode == 3)
				planePopup.transform.Find("CenterWnd/EndPlace").GetComponent<UILabel>().text = Global.Spots[stickerID].SpotEndCityCn;
			else if (Global.LanguageCode == 4)
				planePopup.transform.Find("CenterWnd/EndPlace").GetComponent<UILabel>().text = Global.Spots[stickerID].SpotEndCityJp;

			if(Global.Spots [stickerID].SpotEndDate != null){
				planePopup.transform.Find ("CenterWnd/EndDate").GetComponent<UILabel> ().text = GetStyledDate (Global.Spots [stickerID].SpotEndDate);
			}
			if(Global.Spots [stickerID].SpotEndDate != null){
				planePopup.transform.Find ("CenterWnd/EndTime").GetComponent<UILabel> ().text = GetStyledTime1 (Global.Spots [stickerID].SpotEndDate);
			}
			if(Global.Spots [stickerID].SpotWindSpeed != null){
				planePopup.transform.Find ("BottomWnd/distance_value").GetComponent<UILabel> ().text = Global.Spots [stickerID].SpotWindSpeed;
			}
			if(Global.Spots [stickerID].SpotHeight != null){
				planePopup.transform.Find ("BottomWnd/w_value").GetComponent<UILabel> ().text = Global.Spots [stickerID].SpotHeight.ToString();
			}
			if(Global.Spots [stickerID].need_time != null){
				planePopup.transform.Find ("BottomWnd/time_value").GetComponent<UILabel> ().text =
					Global.Spots [stickerID].need_time.Substring(0, 2) + ":" + Global.Spots [stickerID].need_time.Substring(2);
			}
			break;
		case 2:                     //Terminal Popup
			//controller.SetActive(true);
			Global.sel_spot2 = stickerID;
			TerminalPopupPrefab.SetActive (true);
			//정보 표시
			StartCoroutine(LoadPopupImage(stickerID));
			break;

		case 3:                     //GateInfo Popup
			//controller.SetActive(true);
			GatePopupPrefab.SetActive(true);
			//정보 표시

			string name_data = Global.Spots[stickerID].SpotName_kr;
			name_data = name_data.Substring(name_data.Length - 3, 3);
			name_data = name_arr[Global.LanguageCode - 1] + name_data;
			GatePopupPrefab.transform.Find("GatePopup/TopWnd/TitleText").GetComponent<UILabel>().text = name_data;//;Global.Spots[stickerID].SpotName_kr;

			for (int i = 0; i < Grid.childCount;i++ )
			{
				Destroy(Grid.GetChild(i).gameObject);
			}
			/*Debug.Log(ScrollView.clipOffset);
            Debug.Log(ScrollView.transform.localPosition);*/
			ScrollView.clipOffset = new Vector2(0, -176);
			ScrollView.transform.localPosition = new Vector3(0, -18, 0);

			Debug.Log("index count : " + Global.Spots[stickerID].GateInfos.Count);
			Global.sel_spot1 = stickerID;
			for (int i = 0; i < Global.Spots[stickerID].GateInfos.Count; i++)
			{
				GameObject GridTmp = Instantiate(gridContent).gameObject;
				GridTmp.transform.SetParent(Grid);
				GridTmp.transform.localScale = new Vector3(1, 1, 1);

				GridTmp.transform.localPosition = new Vector3(0, -90*i, 0);
				GridTmp.name = "GateInfo" + i.ToString();

				if(Global.Spots[stickerID].GateInfos[i].time != null){
					GridTmp.transform.GetChild(0).transform.GetComponent<UILabel>().text
					= GetStyledTime1(Global.Spots[stickerID].GateInfos[i].time);
				}

				if (Global.Spots[stickerID].GateInfos[i].GatePosition.Contains("D"))
				{
					GridTmp.transform.GetChild(2).gameObject.SetActive(false);
					GridTmp.transform.GetChild(1).gameObject.SetActive(true);
					GridTmp.transform.GetChild(3).GetComponent<UILabel>().text = "출발";
				}
				else
				{
					GridTmp.transform.GetChild(2).gameObject.SetActive(true);
					GridTmp.transform.GetChild(1).gameObject.SetActive(false);
					GridTmp.transform.GetChild(3).GetComponent<UILabel>().text = "도착";
				}

				if (Global.LanguageCode == 1)
				{
					GridTmp.transform.GetChild(4).transform.GetComponent<UILabel>().text =
						Global.Spots[stickerID].GateInfos[i].DepartCity;
					GridTmp.transform.GetChild(5).transform.GetComponent<UILabel>().text =
						Global.Spots[stickerID].GateInfos[i].ArrivalCity;
				}
				else if (Global.LanguageCode == 2)
				{
					GridTmp.transform.GetChild(4).transform.GetComponent<UILabel>().text =
						Global.Spots[stickerID].GateInfos[i].DepartCityEn;
					GridTmp.transform.GetChild(5).transform.GetComponent<UILabel>().text =
						Global.Spots[stickerID].GateInfos[i].ArrivalCityEn;
				}
				else if (Global.LanguageCode == 3)
				{
					GridTmp.transform.GetChild(4).transform.GetComponent<UILabel>().text =
						Global.Spots[stickerID].GateInfos[i].DepartCityCn;
					GridTmp.transform.GetChild(5).transform.GetComponent<UILabel>().text =
						Global.Spots[stickerID].GateInfos[i].ArrivalCityCn;
				}
				else if (Global.LanguageCode == 4)
				{
					GridTmp.transform.GetChild(4).transform.GetComponent<UILabel>().text =
						Global.Spots[stickerID].GateInfos[i].DepartCityJp;
					GridTmp.transform.GetChild(5).transform.GetComponent<UILabel>().text =
						Global.Spots[stickerID].GateInfos[i].ArrivalCityJp;
				}

				StartCoroutine(LoadGateInfoImage(stickerID, i));

				GridTmp.transform.GetChild(7).transform.GetComponent<UILabel>().text =
					Global.Spots[stickerID].GateInfos[i].AirlineName;

				GridTmp.GetComponent<UITexture>().mainTexture = back_textures[i%2];
			}
			break;
		}
	}

	string[] name_arr = { "게이트", "GATE", "登机口", "ゲート" };
	public Texture[] back_textures;
	public GameObject gridContent;
	public Transform Grid;
	public UIPanel ScrollView;
	public Texture popup5;

	IEnumerator ReLoadBuildingImages()
	{
		TerminalPopupPrefab.transform.Find("TerminalPopup/background").GetComponent<UITexture>().mainTexture = popup5;
		if(TerminalPopupPrefab.activeSelf){
			WWW www = new WWW(Global.Domain + "/img/" + Global.current_spot_id + "0" + Global.LanguageCode.ToString() + ".png");
			yield return www;
			if (!string.IsNullOrEmpty (www.error))
				Debug.Log (www.error);
			else {
				try{
					Texture2D texTmp = new Texture2D(64, 64, TextureFormat.ARGB32, false);
					www.LoadImageIntoTexture(texTmp);
					TerminalPopupPrefab.transform.Find("TerminalPopup/background").GetComponent<UITexture>().mainTexture =	texTmp;
				}
				catch(Exception) {
				}
			}
		}
	}

	IEnumerator LoadPopupImage(int stickerID)
	{
		WWW www = new WWW(Global.Domain + "/img/" + Global.Spots[stickerID].SpotID + "0" + Global.LanguageCode.ToString() + ".png");
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Debug.Log (www.error);
		else {
			try{
				Texture2D texTmp = new Texture2D(64, 64, TextureFormat.ARGB32, false);
				www.LoadImageIntoTexture(texTmp);
				TerminalPopupPrefab.transform.Find("TerminalPopup/background").GetComponent<UITexture>().mainTexture = texTmp;
			}catch(Exception) {
			}
		}
	}

	IEnumerator LoadGateInfoImage(int stickerID, int infoID)
	{
		WWW www = new WWW(Global.Spots[stickerID].GateInfos[infoID].GateTypeImageUrl);
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Debug.Log (www.error);
		else {
			try{
				Texture2D texTmp = new Texture2D(64, 64, TextureFormat.ARGB32, false);
				www.LoadImageIntoTexture(texTmp);
				GatePopupPrefab.transform.Find("GatePopup/MainWnd/ScrollView/Grid/GateInfo" + infoID + "/AirIcon").GetComponent<UITexture>().mainTexture = texTmp;
			}
			catch(SystemException) {
			}
		}
	}

	IEnumerator LoadWethereIcons(int stickerID, bool isArrival)
	{
		if (isArrival) {
			//Start Wethere Icon
			WWW www = new WWW (Global.Domain + "/img/W" + Global.Spots [stickerID].SpotStartWhetherIcon + ".png");
			yield return www;
			if (!string.IsNullOrEmpty (www.error))
				Debug.Log (www.error);
			else {
				try {
					Texture2D texTmp = new Texture2D (64, 64, TextureFormat.ARGB32, false);
					www.LoadImageIntoTexture (texTmp);
					PlanePopupPrefab.transform.Find ("PlaneInfo/arrival/CenterWnd/StartWhether").GetComponent<UITexture> ().mainTexture = texTmp;
				} catch (Exception) {
				}
			}
		} else {
			//End Wethere Icon
			WWW www = new WWW(Global.Domain + "/img/W" + Global.Spots[stickerID].SpotEndWhetherIcon + ".png");
			yield return www;
			if (!string.IsNullOrEmpty (www.error))
				Debug.Log (www.error);
			else {
				try{
					Texture2D texTmpEnd = new Texture2D(64, 64, TextureFormat.ARGB32, false);
					www.LoadImageIntoTexture(texTmpEnd);
					PlanePopupPrefab.transform.Find("PlaneInfo/departure/CenterWnd/StartWhether").GetComponent<UITexture>().mainTexture = texTmpEnd;
				}
				catch(Exception) {
				}
			}
		}
	}

	IEnumerator LoadAirIcon(int stickerID, bool isArrival)
	{
		WWW www = new WWW(Global.Domain + "/img/" + Global.Spots[stickerID].FlightCarrier + ".png");
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Debug.Log (www.error);
		else {
			try
			{
				Texture2D texTmp = new Texture2D(64, 64, TextureFormat.ARGB32, false);
				www.LoadImageIntoTexture(texTmp);
				if(isArrival){
					PlanePopupPrefab.transform.Find("PlaneInfo/arrival/TopWnd/TitleIcon").GetComponent<UITexture>().mainTexture = texTmp;
				}
				else{
					PlanePopupPrefab.transform.Find("PlaneInfo/departure/TopWnd/TitleIcon").GetComponent<UITexture>().mainTexture = texTmp;
				}
			}
			catch(Exception) {
			}
		}
	}

	public void HideTagInfoWnd()
	{
		StartCoroutine(HideInfoPopup());
	}

	IEnumerator HideInfoPopup()
	{
		yield return new WaitForSeconds(0.5f);
		if (Global.Spots[Global.current_spot_id].SpotOnMapObject != null)
		{
			Global.Spots[Global.current_spot_id].SpotOnMapObject.transform.Find("bg").gameObject.SetActive(true);
			Global.Spots[Global.current_spot_id].SpotOnMapObject.transform.Find("bg1").gameObject.SetActive(false);
		}
		PlanePopupPrefab.SetActive (false);
		TerminalPopupPrefab.SetActive (false);
		GatePopupPrefab.SetActive (false);
		Global.is_showing_popup = false;
		controller.SetActive(false);
	}

	public void HideTagFunction(int tagObjID)
	{
		StartCoroutine(HideTagThread(tagObjID));
	}

	public IEnumerator HideTagThread(int tagObjID)
	{
		yield return new WaitForSeconds(0.5f);
		Global.Spots[tagObjID].SpotObject.SetActive(false);
		if (Global.Spots[tagObjID].SpotShownFlag = true && Global.Spots[tagObjID].SpotObject.activeInHierarchy == false)
		{
			Global.Spots[tagObjID].SpotObject.SetActive(true);
		}
		else
			Global.Spots[tagObjID].SpotObject.SetActive(false);
	}
	#endregion

	#region My Technical functions
	public string GetStyledDate(string strDate)
	{
		//201712091030
		string returnStr = "";
		if (strDate == null || strDate.Trim() == "")
			return "";
		returnStr = strDate.Substring (0, 4) + "." + strDate.Substring (4, 2) + "." + strDate.Substring (6, 2);
		return returnStr;
	}

	public string GetStyledTime1(string strDate)
	{
		string returnStr = "";
		if (strDate == null || strDate.Trim() == "")
			return "";
		returnStr = strDate.Substring(8, 2) + ":" + strDate.Substring(10, 2);
		return returnStr;
	}

	public string GetStyledTime(string strDate)
	{
		string returnStr = "";
		if (strDate.Trim() == "")
			return "";
		returnStr = strDate.Substring(6, 2) + ":" + strDate.Substring(8, 2);
		return returnStr;
	}
	#endregion
}
