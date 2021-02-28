using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HVRCORE;
public class drag : MonoBehaviour {
    private static readonly string TAG = "drag";
    // Use this for initialization
    void Start () {
        HVREventListener.Get(transform.gameObject).onDrag = onPoninterDrag;
        HVREventListener.Get(transform.gameObject).onDrop = onPoninterDrop;
		gameObject.AddComponent<RectTransform> ();
	}
	private Vector3 pos;
	private bool onece = true;
	private Vector3 offset;
    public void Button()
    {
        HVRLogCore.LOGI(TAG, "drag");
    }
    private void onPoninterDrag(GameObject go, PointerEventData eventData){
        if (go == transform.gameObject) {
			var rt = this.GetComponent<RectTransform>();
            if (Application.platform == RuntimePlatform.Android)
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out pos))//将屏幕空间上的点eventData.position转换为位于给定RectTransform平面上的世界空间中的位置pos。cam参数应该是与屏幕点相关的相机。对于Canvas设置为“Screen Space - Overlay mode”模式的情况，cam参数应该为null。
                {
                    if (onece)
                    {
                        onece = false;
                        offset = pos - rt.position;
                    }
                    rt.position = pos - offset;

                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {

                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, Input.mousePosition, Camera.allCameras[1], out pos))//将屏幕空间上的点eventData.position转换为位于给定RectTransform平面上的世界空间中的位置pos。cam参数应该是与屏幕点相关的相机。对于Canvas设置为“Screen Space - Overlay mode”模式的情况，cam参数应该为null。
                {
                    if (onece)
                    {
                        onece = false;
                        offset = pos - rt.position;

                    }
                    rt.position = pos - offset;
                    HVRLogCore.LOGI(TAG, eventData.position + ": " + Input.mousePosition);
                }
            }
           
           
		}
	}
	private void onPoninterDrop(GameObject go, PointerEventData eventData){
		if (go == transform.gameObject) {
			HVRLogCore.LOGI(TAG, "onPoninterDrop");
			onece = true;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
