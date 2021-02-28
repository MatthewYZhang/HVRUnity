using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HVRCORE;
public class EnterLeave : MonoBehaviour {
    private static readonly string TAG = "EnterLeave";
    private float mInitPosition;
	// Use this for initialization
	void Start () {
        HVREventListener.Get(transform.gameObject).onEnter = onPointerEnter;
        HVREventListener.Get(transform.gameObject).onExit = onPointerExit;
		mInitPosition = transform.position.z;
	}
	private void onPointerEnter(GameObject go){
		if (go == transform.gameObject) {
			HVRLogCore.LOGI(TAG, "onPointerEnter");
			transform.position = new Vector3 (transform.position.x, transform.position.y, 0.95f * mInitPosition);
		}
	}

	private void onPointerExit(GameObject go){
		if (go == transform.gameObject) {
			HVRLogCore.LOGI(TAG, "onPointerExit");
			transform.position = new Vector3 (transform.position.x, transform.position.y,  mInitPosition);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
