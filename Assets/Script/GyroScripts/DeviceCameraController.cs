/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class DeviceCameraController : MonoBehaviour {

	public enum CameraMode
	{
		FACE_C,
		DEFAULT_C,
		NONE
	}

	[HideInInspector]
	public WebCamTexture cameraTexture; 

	private bool isPlay = false;
	private CameraMode currentMode = CameraMode.DEFAULT_C;
	GameObject e_CameraPlaneObj;
	bool isCorrected = false;
	float screenVideoRatio = 1.0f;
	public bool isPlaying
	{
		get{
			return isPlay;
		}
	}

	//return current mode
	public CameraMode switchMode(CameraMode mode){
		bool bswitched = false;
		WebCamDevice[] devices = WebCamTexture.devices;
		for (int i = 0; i < devices.Length; i++) {
			if (devices [i].isFrontFacing == (mode == CameraMode.FACE_C)) {
				currentMode = mode;
				bswitched = true;
			}
		}
		if (bswitched) {
			StopWork ();
			isCorrected = false;
			StartCoroutine(CamCon());
		}
		return currentMode;
	}

	// Use this for initialization  
	void Awake()  
	{
		StartCoroutine(CamCon());  
		e_CameraPlaneObj = transform.Find ("CameraPlane").gameObject;
//		Global.radio_angle = GameObject.Find ("MyCamera/DeviceCamera").GetComponent<Camera> ().fieldOfView;
	}


	
	// Update is called once per frame  
	void Update()  
	{  
		if (isPlay) {  
			if(e_CameraPlaneObj.activeSelf)
			{
				e_CameraPlaneObj.GetComponent<Renderer>().material.mainTexture = cameraTexture;
			}
		}


		if (cameraTexture != null && cameraTexture.isPlaying) {
			if (cameraTexture.width > 200 && !isCorrected) {
				correctScreenRatio();
			}
		}

	}

	IEnumerator CamCon()  
	{  
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);  
		if (Application.HasUserAuthorization(UserAuthorization.WebCam))  
		{  
			WebCamDevice[] devices = WebCamTexture.devices;
			for (int i = 0; i < devices.Length; i++) {

#if UNITY_EDITOR_WIN || UNITY_STANDALONE
                cameraTexture = new WebCamTexture(1920, 1080); //Screen.width, Screen.height
                break;
#else
				if (devices[i].isFrontFacing == (currentMode == CameraMode.FACE_C)){
					cameraTexture = new WebCamTexture(devices[i].name, 640,480);  
				}
#endif
			}
			if (cameraTexture != null) {
				cameraTexture.Play ();
				isPlay = true;  
			}
		}  
	}

	/// <summary>
	/// Stops the work.
	/// when you need to leave current scene ,you must call this func firstly
	/// </summary>
	public void StopWork()
	{
		isPlay = false;
		if (this.cameraTexture != null && this.cameraTexture.isPlaying) {
			this.cameraTexture.Stop();
			Destroy(this.cameraTexture);
			this.cameraTexture = null;
		}
	}

	/// <summary>
	/// Corrects the screen ratio.
	/// </summary>
	void correctScreenRatio()
	{
        int videoWidth = 1080;//640;
        int videoHeight = 1920;//480;
        int ScreenWidth = 1080;//640;
        int ScreenHeight = 1920;//480;

		float videoRatio = 1;
		float screenRatio = 1;

		if (cameraTexture != null) {
			videoWidth = cameraTexture.width;
			videoHeight = cameraTexture.height;
		}
		videoRatio = videoWidth * 1.0f / videoHeight;
		ScreenWidth = Mathf.Max (Screen.width, Screen.height);
		ScreenHeight = Mathf.Min (Screen.width, Screen.height);
		screenRatio = ScreenWidth * 1.0f / ScreenHeight;

		screenVideoRatio = screenRatio / videoRatio;
		isCorrected = true;

		if (e_CameraPlaneObj != null) {
			e_CameraPlaneObj.GetComponent<CameraPlaneController> ().init (currentMode == CameraMode.FACE_C);
			e_CameraPlaneObj.GetComponent<CameraPlaneController>().correctPlaneScale(screenVideoRatio);
		}

	}

	public float getScreenVideoRatio()
	{
		return screenVideoRatio;
	}
}