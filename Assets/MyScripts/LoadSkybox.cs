using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HVRCORE;

public class LoadSkybox : MonoBehaviour
{
    public const string RESOURCES_SKYBOX_PATH = "Skybox/Skybox";
    //private IHelmetHandle m_HelmetHandle = null;
    private IRenderHandle m_RenderHandle = null;
    private RenderStatistics renderStatics;
    public GameObject cam;
    public GameObject plane;
    //public GameObject panel;
    //public GameObject screen;
    Posture pos;
    public GameObject METHOD;
    public GameObject SPEED;
    public GameObject POS;
    public Text _methodText;
    public Text _speedText;
    public float method = 0.0f;
    bool lastUpdate = false;
    bool tDown;
    bool tUp;
    float lastAngle = 0.0f;
    float speed = 0.0f;
    public static float cam_y;
    Queue q;
    float qLast;
    int direction;
    bool turnBack = false;
    const int qLength = 10;
    int renderFrameRate;
    int submitFrameRate;
    // method 0: simple version(1x)
    // method 1: 2x
    // method 2: 3x
    // method 3: 4x
    // method 4: our method
    int count;

    protected float[] speedList = { 30.0f, 45.0f, 60.0f };
    protected float[] colorGainList = { 1.0f, 1.5f, 2.0f };
    int targetSpeedIndex = 0;

    public static IController controller;
    public IHelmetHandle helmetHandle;

    // Start is called before the first frame update
    void Start()
    {
        pos = new Posture();
        method = 0.0f;
        lastAngle = 0.0f;
        q = new Queue();
        plane = GameObject.Find("Plane");
        //plane.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/Diffuse"));
        print(plane);
        METHOD = GameObject.Find("methodText");
        SPEED = GameObject.Find("speedText");
        POS = GameObject.Find("posText");
        tDown = false;
        tUp = false;
        count = 0;
        for (int i = 0; i < qLength; ++i)
        {
            q.Enqueue(0.0f);
            qLast = 0.0f;
        }

        IControllerHandle controllerHandle = HvrApi.GetControllerHandle();
        if (controllerHandle == null)
        {
            Debug.Log("No Controller Found");
            return;
        }
        int[] indices = controllerHandle.GetValidIndices();

        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("android platform");
            controller = controllerHandle.GetControllerByIndex(indices[1]);
        }
        else
        {
            Debug.Log("stimulator");
            controller = controllerHandle.GetControllerByIndex(indices[1]);
            
        }
        if (controller == null)
        {
            return;
        }
        helmetHandle = HvrApi.GetHelmetHandle();
        if (helmetHandle == null)
        {
            Debug.Log("No Helmet Found");
            return;
        }
        /*
        helmetHandle.SetPositionLock(true);
        helmetHandle.SetPoseLock(true);
        */
        

        Material skyboxmat = Resources.Load(RESOURCES_SKYBOX_PATH) as Material;
        if (skyboxmat != null) {
            bool ret = HVRCamCore.UseSkyBox (true, skyboxmat);
            if (ret){
                Debug.Log ("Materials load succeeded!");
            } else{
                Debug.Log ("Materials load failed!");
            }
        } else {
            Debug.Log ("Material not loaded.");
        }
        

        /*m_HelmetHandle = HvrApi.GetHelmetHandle ();
        if (m_HelmetHandle != null)
        {
            Debug.Log("GetHelmetHandle Success!");
        } else {
            Debug.Log("HelmetHandle is null.");
        }*/

        m_RenderHandle = HvrApi.GetRenderHandle();
        if (m_RenderHandle != null)
        {
            Debug.Log("GetRenderHandle Success!");
        }
        else
        {
            Debug.Log("RenderHandle is null.");
        }
    }

    bool IsInteger(float m)
    {
        return m == 0.0f || m == 1.0f || m == 2.0f || m == 3.0f || m == 4.0f;
    }

    void UpdateMethod()
    {
        bool swipeUp = controller != null ? controller.IsTouchpadSwipeUp() : false;
        bool swipeDown = controller != null ? controller.IsTouchpadSwipeDown() : false;
        bool swipeLeft = controller != null ? controller.IsTouchpadSwipeLeft() : false;
        bool swipeRight = controller != null ? controller.IsTouchpadSwipeRight() : false;
        bool triggerPressed = controller != null ? controller.IsButtonPressed(ButtonType.ButtonTrigger) : false;
        if (!lastUpdate)
        {
            if (swipeUp)
            {
                method = method + 0.1f;
                if (method >= 5.0f) method = 0.0f;
            }
            else if (swipeDown)
            {
                method = method - 0.1f;
                if (method < 0) method = 0.0f;
            }
            else if (swipeLeft)
            {
                method = (int)method - 1.0f;
                if (method <= 0.0f) method = 0;
            }
            else if (swipeRight)
            {
                method = (int)method + 1.0f;
            }
            else if (triggerPressed)
            {
                controller.StartVibrate(1, 1000);
                targetSpeedIndex += 1;
                if (targetSpeedIndex == 3) targetSpeedIndex = 0;
            }
        }
        
        lastUpdate = swipeUp || swipeDown || swipeLeft || swipeRight || triggerPressed;
    }
    private Color getColor(float target_speed, float real_speed)
    {
        float offset = (target_speed - real_speed) / 26.0f;
        // color = Color.FromArgb((int)offset*255.0, (int)0 - offset*255.0, 0);
        Color color;
        if (Mathf.Abs(offset) < 0.5f)
        {
            color = new Color((offset * 2.0f), 1.0f, 0.0f);
           // Debug.Log("offset " + offset + " ycolor " + (offset * 2.0f));
        }
        else if(offset < 0)
        {
            color = new Color(1.0f, (1.0f - (Mathf.Abs(offset) - 0.5f) * 2.0f), 0.0f);
          //  Debug.Log("offset " + offset + " rcolor " + (255.0 - (offset - 0.5f) * 255.0f * 2.0f));
        } 
        else
        {
            color = new Color(1.0f, (1.0f - (Mathf.Abs(offset) - 0.5f) * 2.0f), (offset * 2.0f));
           // Debug.Log("offset " + offset + " rcolor " + (255.0 - (offset - 0.5f) * 255.0f * 2.0f));
        }
        color.a = 0.5f;
        // Color color = new Color((int)(offset*255.0f), (int)(255.0 - offset*255.0f), 0);
        // Debug.Log(target_speed);
        // Debug.Log(real_speed);
        // Debug.Log("offset " + offset + " color " + (int)(offset*255.0f));
        return color;
    }

    float CalSpeed()
    {
        float qFront = (float)q.Dequeue();
        float cam_y = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.y;
        q.Enqueue(cam_y);
        qLast = cam_y;
        if(qFront < 30.0f && qLast > 330.0f) {
            qLast = qLast - 360.0f;
        } else if (qFront > 330.0f && qLast < 30.0f) {
            qFront = qFront - 360.0f;
        }
        return System.Math.Abs(qLast - qFront) * 100/ qLength;
    }


    int Direction() {
        float thres = 1.0f;
        if(q.Count < qLength) return 0;
        float qFront = (float)q.Peek();
        // deal with dangling case
        if(qFront < 30.0f && qLast > 330.0f) {
            float tempLast = qLast - 360.0f;
            if(Mathf.Abs(qFront - tempLast) < thres) return 0;
            return -1; //turn left
        } else if (qFront > 330.0f && qLast < 30.0f) {
            float tempFront = qFront - 360.0f;
            if(Mathf.Abs(tempFront - qLast) < thres) return 0;
            return 1; //turn right
        }
        //normal case
        if(Mathf.Abs(qFront - qLast) < thres) return 0; 
        if(qLast - qFront > thres) return 1; //turn right
        else if(qFront - qLast > thres) return -1; //turn left
        return 1;
    }

    bool JudgeTurnBack() {
        if(direction == 0) return false;
        else if(direction > 0) {
            if(qLast > 180.0f) turnBack = true;
            else turnBack = false;
        } else if(direction < 0) {
            if(qLast < 180.0f) turnBack = true;
            else turnBack = false;
        }
        return turnBack;
    }

    float CalAmpliFactor(float vel, float lower_bound, float upper_bound, float max_comfort) {
        vel = Mathf.Abs(vel);
        float factor;
        if(0.0f <= vel && vel < speedList[0]) {
            factor = 2.95f;
        } else if(vel < speedList[1]) {
            factor = 2.55f + (speedList[1] - vel) / (speedList[1] - speedList[0]) * (2.95f - 2.55f);
        } else if(vel < speedList[2]) {
            factor = 2.22f + (speedList[2] - vel) / (speedList[2] - speedList[1]) * (2.55f - 2.22f);
        } else {
            return 2.22f;
        }
        return factor;
    }

    void updatePanel()
    {
        //METHOD.GetComponent<Text>().text = method.ToString(direction + " tb " + turnBack);
        Debug.Log("qfront " + q.Peek() + " qlast " + qLast + " " + Direction() + " tb " + turnBack);
        if (count == 0)
        {
            METHOD.GetComponent<Text>().text = method.ToString("0.0");
            SPEED.GetComponent<Text>().text = speed.ToString("0.00") + " " + speedList[targetSpeedIndex];
            
            
        }
        count += 1;
        if (count == qLength) count = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMethod();
        speed = CalSpeed();
        direction = Direction();
        turnBack = JudgeTurnBack();
        // Debug.Log("Direction " + direction + " tb " + isTurningBack);
        //Debug.Log("Current Head Angle is " + HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.y);
        plane.GetComponent<Renderer>().material.color = getColor(speedList[targetSpeedIndex], speed);
        

        updatePanel();
        if (speed < 180.0f)
        {
            // Debug.Log("Speed is " + speed);
        }
        //Debug.Log("Current Method is " + method);
        cam_y = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.y;
        if (method == -1.0f || method == 0.0f)
        {
            cam.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (method < 5)
        {
            if (IsInteger(method))
            {
                Vector3 v3 = new Vector3(0, method * cam_y, 0);
                cam_y = (method+1) * cam_y % 360f;
                cam.transform.localRotation = Quaternion.Euler(v3);
            }
            else
            {
                if (cam_y <= 180.0f)
                {
                    Vector3 v3 = new Vector3(0, method * cam_y, 0);
                    cam_y = (method+1) * cam_y;
                    cam.transform.localRotation = Quaternion.Euler(v3);
                }
                else
                {
                    Vector3 v3 = new Vector3(0, method * (cam_y - 360.0f), 0);
                    cam_y = 360f - (method+1) * (360f - cam_y);
                    cam.transform.localRotation = Quaternion.Euler(v3);
                }
            }
        }
        else // method == 5, our method
        {
            Debug.Log("in method 5");
            return;
            // implement our method
        }


        //int ret = helmetHandle.GetPosture(ref pos);
        /*
        if (ret == 0)
        {

            Quaternion quatDat;
            Vector3 posDat;
            quatDat = pos.rotation; //获取旋转姿态信息
            posDat = pos.position; //获取位置信息
            Debug.Log("Using Pos: " + quatDat + " " + posDat);
        }
        */
        /*Posture pos = new Posture ();
        int ret = m_HelmetHandle.GetPosture (ref pos);
        if (ret == 0) {
            Vector3 quatEuler = pos.rotation.eulerAngles;
            Debug.Log(quatEuler.y);
            quatEuler = new Vector3(pos.rotation.eulerAngles.x, pos.rotation.eulerAngles.y * 2, pos.rotation.eulerAngles.z);
            HVRLayoutCore.m_CamCtrObj.transform.rotation = Quaternion.Euler(quatEuler);
            Debug.Log(HVRLayoutCore.m_CamCtrObj.transform.rotation.eulerAngles.y);
        } else {
　　      Debug.Log ("Get VR glass posture failed!");
        }*/

        /*
      m_RenderHandle.GetRenderStatics(ref renderStatics);
　　  submitFrameRate = (int)renderStatics.SubmitFrameRate; //提交帧率
　　  renderFrameRate = (int)renderStatics.RenderFrameRate; //渲染帧率
        POS.GetComponent<Text>().text = renderFrameRate + " " + submitFrameRate;
        Debug.Log(renderStatics.RenderFrameRate);

        Debug.Log(Time.deltaTime);
        */


        
    }
}
