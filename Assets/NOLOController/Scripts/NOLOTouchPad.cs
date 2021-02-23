using System.Collections;
using System.Collections.Generic;
using HVRCORE;
using UnityEngine;

public class NOLOTouchPad : MonoBehaviour {


    private GameObject m_TouchPadPoint;
    private IController m_Controller = null;
    // Use this for initialization
    void Start () {
        m_TouchPadPoint = transform.Find("TouchPoint").gameObject;      
        m_TouchPadPoint.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        if (m_Controller == null)
        {
            
            if (transform.parent.parent.parent.parent.name.Equals("HVRLeftController"))
            {
                m_Controller = HVRController.m_LeftController;  
            }
            else if(transform.parent.parent.parent.parent.name.Equals("HVRRightController"))
            {
                m_Controller = HVRController.m_RightController;
            }
        }

        if (m_Controller == null || !m_Controller.IsAvailable())
        {
            return;
        }
        Vector2 pos = new Vector2(0.5f,0.5f);
        m_Controller.GetTouchpadTouchPos(ref pos);
        pos.y = 1 - pos.y;
        UpdateTouchPoint(pos);
    }

    private void UpdateTouchPoint(Vector2 pos)
    {
        float touch_x = 0.5f - Mathf.Clamp01(pos.x);
        float touch_y = 0.5f - Mathf.Clamp01(pos.y);
        float diameterx = 26f;
        float diametery = 26f;
        Vector3 offset = new Vector3(touch_x * diameterx, 0.0f, touch_y * diametery);
        m_TouchPadPoint.transform.localPosition = offset;
    }
}
