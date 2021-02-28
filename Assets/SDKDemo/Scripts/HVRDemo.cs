/************************************************************************************

Filename    :   HVRDemo.cs
Authors     :   HuaweiVRSDK
Copyright   :   Copyright HUAWEI Technologies Co., Ltd. 2019. All Rights reserved.

*************************************************************************************/
//#define HuaweiVRUsed

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HVRCORE;


public class HVRDemo : MonoBehaviour
{
    private static readonly string TAG = "Unity_HVRDemo";

    [SerializeField]
	private Text m_touchInfo = null;
	[SerializeField]
	private Text m_otherInfo = null;
	[SerializeField]
	private Text m_headPosInfo = null;
	[SerializeField]
	private Text m_backInfo = null;

	[SerializeField]
	private Text m_StatusText;
	[SerializeField]
	private Text m_BatteryText;
	[SerializeField]
	private Text m_OrientationText;
	[SerializeField]
	private Text m_GyroscopeText;
	[SerializeField]
	private Text m_AccelerometerText;

	[SerializeField]
	private Button m_HomeButton;
	[SerializeField]
	private Button m_BackButton;
	[SerializeField]
	private Button m_ConfirmButton;
	[SerializeField]
	private Button m_TriggerButton;
	[SerializeField]
	private Button m_TouchpadButton;
	[SerializeField]
	private Transform m_TouchPointTransform;
	[SerializeField]
	private Text m_TouchpadText;

	private Scene m_Scene1;
	private Scene m_Scene2;

	private IHelmetHandle m_HelmetHandle = null;
	private IController m_Controller = null;

	private Vector2 m_TouchpadPos;
	private Vector3 m_TouchpadInitPosititon;
	private const float m_TriggerThreshold = 0.7f;

    void Start ()
	{
        m_Scene1 = SceneManager.GetSceneByName ("Scene1");
		m_Scene2 = SceneManager.GetSceneByName ("scene2");
        
		m_TouchpadInitPosititon = m_TouchPointTransform.position;

        GetControllerByHelmetModel();
    }

    private void GetControllerByHelmetModel() {
        m_HelmetHandle = HvrApi.GetHelmetHandle();
        if (m_HelmetHandle == null)
        {
            HVRLogCore.LOGE(TAG, "mHelmetHandle is null");
            return;
        }
        HelmetModel helmetModel = HelmetModel.HVR_HELMET_THIRD_GEN;
        int ret = m_HelmetHandle.GetHelmetInfo(ref helmetModel);
        HVRLogCore.LOGI(TAG, "helmetModel: " + helmetModel);
        if (ret == 0)
        {
            IControllerHandle controllerHandle = HvrApi.GetControllerHandle();
            if (controllerHandle == null)
            {
                HVRLogCore.LOGE(TAG, "controllerHandle is null");
                return;
            }
            int[] indices = controllerHandle.GetValidIndices();
            for (int i = 0; i < indices.Length; i++)
            {
                HVRLogCore.LOGI(TAG, "controller indices : " + indices[i]);
            }
            switch (helmetModel)
            {
                case HelmetModel.HVR_HELMET_FIRST_GEN:
                    HVRLogCore.LOGI(TAG, "HUAWEI VR 1.0 helmet");
                    m_Controller = controllerHandle.GetControllerByIndex(indices[0]);
                    break;
                case HelmetModel.HVR_HELMET_SECOND_GEN:
                case HelmetModel.HVR_HELMET_THIRD_GEN:
                    HVRLogCore.LOGI(TAG, "HUAWEI VR 2 or HUAWEI VR Glass");
                    m_Controller = controllerHandle.GetControllerByIndex(indices[1]);
                    break;
                case HelmetModel.HVR_HELMET_NOT_FOUND:
                    m_Controller = controllerHandle.GetControllerByIndex(indices[1]);
                    if (null != m_Controller)
                    {
                        if (m_Controller.IsAvailable())
                        {
                            HVRLogCore.LOGI(TAG, "controller is available");
                        }
                    }
                    else
                    {
                        m_Controller = controllerHandle.GetControllerByIndex(indices[0]);
                    }
                    HVRLogCore.LOGI(TAG, "No helmet , mobile sensor");
                    break;
                case HelmetModel.HVR_HELMET_UNKNOWN:
                    HVRLogCore.LOGI(TAG, "Unknow helmet");
                    break;
            }
        }
    }

	// Update is called once per frame
	void Update ()
	{			
		if (null == m_Controller) {
            HVRLogCore.LOGE(TAG, "mController is null");
            return;
		}

		bool isControllerDataValid = false;
		ControllerStatus controllerStatus = ControllerStatus.ControllerStatusDisconnected;
		controllerStatus = m_Controller.GetControllerStatus ();

		switch (controllerStatus) {
		case ControllerStatus.ControllerStatusDisconnected:
			m_StatusText.text = "Disconnected";
			break;
		case ControllerStatus.ControllerStatusScanning:
			m_StatusText.text = "Scanning";
			break;
		case ControllerStatus.ControllerStatusConnecting:
			m_StatusText.text = "Connecting";
			break;
		case ControllerStatus.ControllerStatusConnected:
			m_StatusText.text = "Connected";
			isControllerDataValid = true;
			break;
		case ControllerStatus.ControllerStatusError:
			m_StatusText.text = "Error";
			break;
		}

		if (!isControllerDataValid) {
			HVRLogCore.LOGI(TAG, "controller is not avalible");
			return;
		}

        Posture controllePos = new Posture();
        m_Controller.GetPosture(ref controllePos);
        //Controller orientation
        m_OrientationText.text = controllePos.rotation.ToString();

        Vector3 gyroscope = new Vector3(0.0f, 0.0f, 0.0f);
        m_Controller.GetGyroscope(ref gyroscope);
        //Gyroscope
        m_GyroscopeText.text = gyroscope.ToString();

        Vector3 accelerometer = new Vector3(0.0f, 0.0f, 0.0f);
        m_Controller.GetAccelerometer(ref accelerometer);
        //Accelerometer
        m_AccelerometerText.text = accelerometer.ToString();

        if (m_Controller.IsButtonPressed(ButtonType.ButtonHome))
        {
            HVRLogCore.LOGI(TAG, "@@@ButtonHome");
            m_HomeButton.OnSubmit(null);
        }
        if (m_Controller.IsButtonPressed(ButtonType.ButtonBack))
        {
            HVRLogCore.LOGI(TAG, "@@@ButtonApp");
            m_BackButton.OnSubmit(null);
        }
        if (m_Controller.IsButtonPressed(ButtonType.ButtonConfirm))
        {
            HVRLogCore.LOGI(TAG, "@@@ButtonConfirm");
            m_ConfirmButton.OnSubmit(null);
        }
        if (m_Controller.IsButtonPressed(ButtonType.ButtonTouchPad))
        {
            HVRLogCore.LOGI(TAG, "@@@ButtonTouchPad");
            m_TouchpadButton.OnSubmit(null);
        }
        if (m_Controller.IsButtonPressed(ButtonType.ButtonTrigger))
        {
            HVRLogCore.LOGI(TAG, "@@@ButtonTrigger");
            m_TriggerButton.OnSubmit(null);
        }

        //touchpad
        if (m_Controller.IsTouchpadTouching())
        {
            m_TouchpadButton.OnSubmit(null);
            m_Controller.GetTouchpadTouchPos(ref m_TouchpadPos);
            m_TouchPointTransform.position = m_TouchpadInitPosititon + new Vector3((m_TouchpadPos.x - 0.5f) / 1.414f, -(m_TouchpadPos.y - 0.5f) / 1.414f, 0.0f);
            m_TouchpadText.text = "Touchpad\n" + m_TouchpadPos;
        }

        float triggerData = 0.0f;
        //Trigger
        m_Controller.GetTriggerData(ref triggerData);
        float mTriggerData = triggerData;
        if (mTriggerData > m_TriggerThreshold)
        {
            HVRLogCore.LOGI(TAG, "mTriggerData：" + mTriggerData);
        }
        m_BatteryText.text = m_Controller.GetBatteryLevel()+"%";

        if (m_Scene2.isLoaded) {
			if (null != m_HelmetHandle) {
				Posture pos = new Posture ();
				m_HelmetHandle.GetPosture (ref pos);
				Quaternion quat = pos.rotation;
				if (m_headPosInfo != null) {
					m_headPosInfo.text = quat.ToString ();
				}
			}
			if (m_Controller.IsTouchpadTouchUp ()) {
				m_touchInfo.text = "Touch Up\n";
			} else if (m_Controller.IsTouchpadTouchDown ()) {
				m_touchInfo.text = "Touch Down\n";
			}  

			if (m_Controller.IsTouchpadSwipeDown ()) {
				m_touchInfo.text = "Down Swipe";
			} else if (m_Controller.IsTouchpadSwipeUp ()) {
				m_touchInfo.text = "Up Swipe";
			} else if (m_Controller.IsTouchpadSwipeLeft ()) {
				m_touchInfo.text = "Left Swipe";
			} else if (m_Controller.IsTouchpadSwipeRight ()) {
				m_touchInfo.text = "Right Swipe";
			} 
			if (m_Controller.IsButtonDown (ButtonType.ButtonBack)) {
				m_backInfo.text = "ButtonBack Down\n";
			} else if (m_Controller.IsButtonUp (ButtonType.ButtonBack)) {
				m_backInfo.text = "ButtonBack Up\n";
			} 
		}
	}

}
