using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HVRCORE;
public class Move : MonoBehaviour {

    private static readonly string TAG = "Move";
    private int num = 599;
	// Use this for initialization
	void Start () {
        HVREventListener.Get(transform.gameObject).onMove = onPoninterMove;
	}

	private void onPoninterMove(GameObject go, AxisEventData eventData){
		if (go == transform.gameObject) {
			if (eventData.moveDir == MoveDirection.Up) {
                HVRLogCore.LOGI(TAG, "MoveDirection: up");
				num++;
			} else if (eventData.moveDir == MoveDirection.Down) {
                HVRLogCore.LOGI(TAG, "MoveDirection: down");
				num--;
			} else if (eventData.moveDir == MoveDirection.Right) {
                HVRLogCore.LOGI(TAG, "MoveDirection: right");
				num++;
			} else if (eventData.moveDir == MoveDirection.Left) {
				num--;
                HVRLogCore.LOGI(TAG, "MoveDirection: left");
				
			}
			GetComponent<Renderer> ().material.mainTexture = Resources.Load ("Textures/" + (1+num%6).ToString())as Texture;
		} 
	}
	// Update is called once per frame
	void Update () {
		
	}
}
