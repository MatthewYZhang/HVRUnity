using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HVRCORE;
using UnityEngine.UI;
public class Click : MonoBehaviour {
    private static readonly string TAG = "Click";
    private Scene mScene1;
	private Scene mScene2;
	[SerializeField]
	private Text mButtonInfo;
    // Use this for initialization
    private void OnEnable()
    {       
       
    }
    void Start () {
		mScene1 = SceneManager.GetSceneByName ("scene1");
		mScene2 = SceneManager.GetSceneByName ("scene2");
        HVREventListener.Get(transform.gameObject).onClick = onPointerClick;
       
    }
	private void onPointerClick(GameObject go){
		if (go == transform.gameObject) {
			HVRLogCore.LOGI(TAG, "onPointerClick");
			if (gameObject.name == "LoadScene") {
               
                if (mScene1.isLoaded) {
					SceneManager.LoadScene ("scene2");
					HVRLogCore.LOGI(TAG, "change to scene2");
				} else if (mScene2.isLoaded) {
					SceneManager.LoadScene ("scene1");
					HVRLogCore.LOGI(TAG, "change to scene1");
				}
			}else if (gameObject.name == "Capture") {
				HvrApi.GetRenderHandle ().CaptureEyeImage ("sdcard/image.jpg");
			}else if (gameObject.name == "ResetYaw") {
				HvrApi.GetHelmetHandle ().ResetYaw ();
			}else if (gameObject.name == "LockPose") {
				HvrApi.GetHelmetHandle ().SetPoseLock (true);
				mButtonInfo.text = "UnLockPose";
				gameObject.name = "UnLockPose";
			}else if (gameObject.name == "UnLockPose") {
				HvrApi.GetHelmetHandle ().SetPoseLock (false);
				mButtonInfo.text = "LockPose";
				gameObject.name = "LockPose";
			}
			else if (gameObject.name == "EnableCAC") {
				HvrApi.GetRenderHandle ().EnableChromaticAberration (true);
				mButtonInfo.text = "UnableCAC";
				gameObject.name = "UnableCAC";
			}else if (gameObject.name == "UnableCAC") {
				HvrApi.GetRenderHandle ().EnableChromaticAberration (false);
				mButtonInfo.text = "EnableCAC";
				gameObject.name = "EnableCAC";
			}
		} 
	}
}
