using System.Collections;
using System.Collections.Generic;
using HVRCORE;
using UnityEngine;


public class NOLOControllerAnimator : MonoBehaviour {

    private Animator m_menu,m_system,m_grip,m_trigger,m_touchpad;

    public ControllerIndex controllerIndex = 0;

    private IController m_Controller = null;

    private GameObject m_BatteryLevel0;
    private GameObject m_BatteryLevel1;
    private GameObject m_BatteryLevel2;
    private GameObject m_BatteryLevel3;

    // Use this for initialization
    void Start () {
        m_menu = transform.Find("Buttons/b_m").GetComponent<Animator>();
        m_system = transform.Find("Buttons/b_s").GetComponent<Animator>();
        m_grip = transform.Find("Buttons/b_g").GetComponent<Animator>();
        m_trigger = transform.Find("Buttons/b_t").GetComponent<Animator>();
        m_touchpad = transform.Find("Buttons/b_f").GetComponent<Animator>();

        m_BatteryLevel0 = transform.Find("Battery/level0").gameObject;
        m_BatteryLevel1 = transform.Find("Battery/level1").gameObject;
        m_BatteryLevel2 = transform.Find("Battery/level2").gameObject;
        m_BatteryLevel3 = transform.Find("Battery/level3").gameObject;

    }
	
	// Update is called once per frame
	void Update () {

        if (m_Controller == null)
        {
            if (controllerIndex == ControllerIndex.LEFT_CONTROLLER)
            {
                m_Controller = HVRController.m_LeftController;
            }
            else
            {
                m_Controller = HVRController.m_RightController;
            }
        }

        if (m_Controller == null || !m_Controller.IsAvailable())
        {
            return;
        }

        updateBatteryLevel();

        //menu - for back button of huawei controller
        if (m_Controller.IsButtonPressed(ButtonType.ButtonBack))
        {
            m_menu.SetBool("isPressed", true);
        }
        else if(m_Controller.IsButtonUp(ButtonType.ButtonBack))
        {
            m_menu.SetBool("isPressed", false);
        }
        //system
        if (m_Controller.IsButtonPressed(ButtonType.ButtonHome))
        {
            m_system.SetBool("isPressed", true);
        }
        else if(m_Controller.IsButtonUp(ButtonType.ButtonHome))
        {
            m_system.SetBool("isPressed", false);
        }
        //grip
        //if (Input.GetKey(KeyCode.C))
        //{
        //    m_grip.SetBool("isPressed", true);
        //}
        //else
        //{
        //    m_grip.SetBool("isPressed", false);
        //}

        //trigger
        if (m_Controller.IsButtonPressed(ButtonType.ButtonTrigger))
        {
            m_trigger.SetBool("isPressed", true);
        }
        else if(m_Controller.IsButtonUp(ButtonType.ButtonTrigger))
        {
            m_trigger.SetBool("isPressed", false);
        }

        //touchpad 
        if (m_Controller.IsButtonPressed(ButtonType.ButtonConfirm))
        {
            m_touchpad.SetBool("isPressed", true);
        }
        else if(m_Controller.IsButtonUp(ButtonType.ButtonConfirm))
        {
            m_touchpad.SetBool("isPressed", false);
        }
    }

    void updateBatteryLevel() {

        int level = m_Controller.GetBatteryLevel();
        m_BatteryLevel0.SetActive(false);
        m_BatteryLevel1.SetActive(false);
        m_BatteryLevel2.SetActive(false);
        m_BatteryLevel3.SetActive(false);

        if (level > 75) {
            m_BatteryLevel3.SetActive(true);
        }
        else if (level >50 )
        {
            m_BatteryLevel2.SetActive(true);
        }
        else if (level > 25)
        {
            m_BatteryLevel1.SetActive(true);
        }
        else
        {
            m_BatteryLevel0.SetActive(true);
        }
    }
}


