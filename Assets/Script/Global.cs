using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Global {
    #region Scene Names
    public static string SplashSceneName = "Splash";
    public static string MainSceneName = "Main";
    #endregion

    public static int sel_spot;
    public static int sel_spot1;
	public static int sel_spot2;
    #region Variables for Main
    public static System.DateTime LastUsingTime;
    public static int tagID = 0;
    #endregion
    //public static string Domain = "http://172.26.100.15:80";
    public static string Domain = "http://58.229.208.247:8080";
//	public static string Domain = "http://172.26.100.16:80";
    public static int LanguageCode = 1;//한국어, 2-영어, 3-중어,4-일어
    public static float rot_offset = 0;
    public static float up_offset = 0;
	public static float count_time = 0.0f;
    public static int mode = 1;
	public static int current_spot_id = -1;//현재의 팝업아이디

	public static bool is_showing_popup = false;
    public static List<GameObject> TouchIconObjs;
	public static int BSpots_count = 0;
	public static List<SpotInformation> Spots;

	public static float camera3_radio = 145.43f;
	public static float distance_tower = 0.0f;
    //public static List<GameObject> ObjonMaps;
    public static GPS currPosition;

	public static float plane_height = 15.0f;

    public static bool is_ngui_click = false;

    //큰 지도에서 영역 제한
	public static double LeftTopLngOnBig= 126.4600927666311;
	public static double LeftTopLatOnBig= 37.46303572388308;
	public static double RightTopLngOnBig = 126.43182761087022;
	public static double RightTopLatOnBig = 37.44676845241015;
	public static double LeftBottomLngOnBig = 126.44888646020493;
	public static double LeftBottomLatOnBig = 37.47537434585513;
	public static double RightBottomLngOnBig  = 126.42034775628647;
	public static double RightBottomLatOnBig  = 37.45950151288729;

	public static double MaxLngValueOnBig = LeftTopLngOnBig;
	public static double MinLngValueOnBig = RightBottomLngOnBig;
	public static double MaxLatValueOnBig = LeftBottomLatOnBig;
	public static double MinLatValueOnBig = RightTopLatOnBig;

	//작은 지도에서 영영 제한
	public static double LeftTopLngOnSmall = 126.4645286860275;
	public static double LeftTopLatOnSmall = 37.45615638338264;
	public static double RightTopLngOnSmall = 126.43749201915739;
	public static double RightTopLatOnSmall = 37.441233543171975;
	public static double LeftBottomLngOnSmall = 126.4456888499069;
	public static double LeftBottomLatOnSmall = 37.47737712086509;
	public static double RightBottomLngOnSmall = 126.41882384441374;
	public static double RightBottomLatOnSmall = 37.462390386941166;

	public static double MaxLngValueOnSmall = LeftTopLngOnSmall;
	public static double MinLngValueOnSmall = RightBottomLngOnSmall;
	public static double MaxLatValueOnSmall = LeftBottomLatOnSmall;
	public static double MinLatValueOnSmall = RightTopLatOnSmall;

//비행기, 건물 스티커 선택
	public static int selBuilding = 2;//1-비행기, 2-게이트,터미널
    public static int selLanguge = -1;//1-비행기, 2-게이트,터미널
                                       //날씨
    public static string maxTemp;
	public static string minTemp;

	public static float radio_angle = 65.0f;//카메라 수평보임각도
	public static float angle = 0.0f;//라디오 회전각도
	public static float hangle = 0.0f;//카메라 배치각도
	public static float camera_height = 34.0f;//카메라 설치 높이
	
//	public static GPS T1_Postion = new GPS(126.43464345270922, 37.46636538621945);
	public static GPS T1_Postion = new GPS(126.43560904795459, 37.46721268267706);

	public static double k1 = 0.86;
	public static double k2 = -1.7;
	public static double k3 = 1.08;// 0.06;
	public static double p1 = 0;
	public static double p2 = 0;
	static float hfov = 97.1f;
	static float vfov = 65.0f;

	public static double fx = (double)((float)Screen.width / (2.0f * Mathf.Tan(0.5f * hfov * Mathf.Deg2Rad)));
	public static double fy = (double)((float)Screen.height / (2.0f * Mathf.Tan(0.5f * vfov * Mathf.Deg2Rad)));
	public static double cy = Screen.height / 2.0;
	public static double cx = Screen.width / 2.0;

	//스티커 x비
	public static double[] stick_rate = new double[]{		
		5.1277f,//start
		5.1277f,//104
		1.862364615f,//106
		1.251803871f,//108
		1.157937931f,//110
		1.079506269f,//112
		1.043716945f,//114
		0.964727395f,//1관제탑
		1.011405546f,//관제탑
		0.987133178f,//118
		0.959569948f,//122
		0.949321974f,//124
		0.939566317f,//126
		0.929849474f,//128
		0.94389537f,//130
		0.94389537f//end
	};
	public static float [] x_u = new float[]{
		0.0f,//start
		102.554f,//104
		121.0537f,//106
		194.0296f,//108
		268.6416f,//110
		361.6346f,//112
		437.3174f,//114
		574.02f,//1관제탑
		619.9916f,//관제탑
		636.7009f,//118
		731.1923f,//122
		807.873f,//124
		895.4067f,//126
		971.6927f,//128
		1019.407f,//130
		1080.0f//end
	};

	public static Vector3 disortPoint(Vector3 uPt)
	{
		Vector3 dPt = new Vector3();
		double y_nu = (uPt.y - cy) / fy;
		double x_nu = (uPt.x - cx) / fx;
		double ru2 = x_nu * x_nu + y_nu * y_nu;    // ru2 = ru*ru
		double radial_d = 1 + k1 * ru2 + k2 * ru2 * ru2 + k3 * ru2 * ru2 * ru2;
		double x_nd = radial_d * x_nu + 2 * p1 * x_nu * y_nu + p2 * (ru2 + 2 * x_nu * x_nu);
		double y_nd = radial_d * y_nu + p1 * (ru2 + 2 * y_nu * y_nu) + 2 * p2 * x_nu * y_nu;
		dPt.x = (float)(fx * x_nd + cx);
		dPt.y = (float)(fy * y_nd + cy);
		return dPt;
	}

	public static Vector3 distort_position(Vector3 point){
		double dis = -0.01f;
		for (int i = 0; i < x_u.Length - 1; i++) {
			if(point.x >= x_u[i] && point.x < x_u[i + 1]){
//				Debug.Log ("delta" + i + " = " + delta_r / delta_x * i);
				dis = stick_rate [i];
				break;
			}
		}
		return new Vector3((float)(point.x / dis), point.y, 0);
	}

    public static int GetIndexOfSpot(string SpotName)
    {
        if (Spots == null)
            return -1;
        int nCount = Spots.Count;
        for (int i = 0; i < nCount; i++)
        {
            if (Spots[i].SpotID == SpotName)
                return i;
        }

        // can't have the index
        return -1;
    }
    public static bool JsonDataContainsKey(JsonData data, string key)
    {
        bool result = false;
        if (data == null)
            return result;
        if (!data.IsObject)
        {
            return result;
        }
        IDictionary tdictionary = data as IDictionary;
        if (tdictionary == null)
            return result;
        if (!tdictionary.Contains(key))
        {
            return result;
        }
        if (data[key] != null)
        {
            result = true;
        }
            
        return result;
    }

}

public class GPS
{
    public double lng;
    public double lat;

    public GPS(double ln, double lt)
    {
        this.lng = ln;
        this.lat = lt;
    }
}

public class SpotInformation
{
    public int SpotType; // 스팟의 형태, 1 = 비행기, 2 = 건물, 3 = 게이트, 4 = 현재 position  :  3개에서 다 이용함
    public string SpotID; //스팟의 아이디, 내부적으로 이용하는 영문이름     :  3개에서 다 이용함
    public string SpotNumber;
    public string SpotName_kr; //스팟의 이름, 스팟에 나타나는 이름         :  3개에서 다 이용함
    public GPS SpotPosition; //스팟의 위치                               :  3개에서 다 이용함
    public double SpotHeight;//스팟의 높이 (m)                           :  3개에서 다 이용함
    public GameObject SpotObject;//스팟의 GameObject포인터                :  3개에서 다 이용함
    public GameObject SpotOnMapObject;//지도상에서 GameObject포인터                :  3개에서 다 이용함
    public string SpotImageUrl;//스팟 정보창의 기본 이미지 주소                :  Build , Terminal
    public Vector3 PrevSpotPos;//   전프레임 스팟의 2차원 좌표값                :  3개에서 다 이용함
    public Vector3 CurSpotPos;//   현재프레임 스팟의 2차원 좌표값                :  3개에서 다 이용함
    public Vector3 NextSpotPos;//   다음프레임 스팟의 2차원 좌표예측 값                :  3개에서 다 이용함

    public bool SpotShownFlag;   //현재 스팟이 보이는지 나타내는 flag    :  3개에서 다 이용함
    public bool SpotPopupOpened; //정보 팝업 표시 flag                    :  3개에서 다 이용함

    //비행기인경우 변수들
    public double lastPlaneLat;
    public double lastPlaneLng;
	public bool isArrival;
	public string passenger;
	public string need_time;

	public string gate;
	public string airbus;
    public string SpotStartID;
    public string SpotStartCountry;
    public string SpotStartCity;
    public string SpotStartCityEn;
    public string SpotStartCityCn;
    public string SpotStartCityJp;
    public string SpotStartDate;
    public string SpotStartWhetherIcon;
    public string SpotStartWhetherValue;
    public string SpotEndID;
    public string SpotEndCountry;
    public string SpotEndCity;
    public string SpotEndCityEn;
    public string SpotEndCityCn;
    public string SpotEndCityJp;
    public string SpotEndDate;
    public string SpotEndWhetherIcon;
    public string SpotEndWhetherValue;
    public string SpotWindSpeed;        //비행기 속도
    public string SpotGateText;             //게이트 정보(비행기)

    public string MessageType;          //D : not showing on Map
    public string PlaneTarget;          //A : landing, D : flying
    public string FlightCarrier;        //항공사 이미지 이름

    //게이트에 필요한 변수들
    public List<GateInfo> GateInfos;

    public SpotInformation(int t, string n, string n_number,string n_kr, double lat, double lng, double h)
    {
        SpotType = t;
		SpotID = n;
        SpotNumber = n_number;
        SpotName_kr = n_kr.Trim();
        SpotPosition = new GPS(lng, lat);
        SpotHeight = h;
    }

    public SpotInformation(string name, string id, double lat, double lng, double h)
    {        
        SpotID = id;
        SpotName_kr = name.Trim();
        SpotPosition = new GPS(lng, lat);
        SpotHeight = h;
    }
}

public struct GateInfo
{
    public string time;             //시간
    public string GatePosition;           //출발/도착 flag - false=출발, true=도착
    public string DepartCity;       //출발지 도시
    public string DepartCityEn;       //출발지 도시
    public string DepartCityCn;       //출발지 도시
    public string DepartCityJp;       //출발지 도시
    public string ArrivalCity;      //도착지 도시
    public string ArrivalCityEn;      //도착지 도시
    public string ArrivalCityCn;      //도착지 도시
    public string ArrivalCityJp;      //도착지 도시
    public string GateTypeImageUrl;            //항공사
    public string AirlineName;         //편명
}