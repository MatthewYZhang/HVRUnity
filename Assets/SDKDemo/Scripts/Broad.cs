using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Broad : MonoBehaviour {

    Button btn;
    AndroidJavaObject androijavaObject;

    void Start () {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return;
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        androijavaObject = new AndroidJavaObject("com.example.hellojni.BroadCastSender", activity);
        
        HVREventListener.Get(transform.gameObject).onClick = send;
        
    }
	void Update () {
		
	}

    private void send(GameObject go)
    {
        androijavaObject.Call("sendMsg", int.Parse(go.name));
    }
}
