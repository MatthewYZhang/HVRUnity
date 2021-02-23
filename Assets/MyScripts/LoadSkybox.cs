using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HVRCORE;

public class LoadSkybox : MonoBehaviour
{
    public const string RESOURCES_SKYBOX_PATH = "Skybox/Skybox";
    //private IHelmetHandle m_HelmetHandle = null;
    //private IRenderHandle m_RenderHandle = null;
    //private RenderStatistics renderStatics;
    public GameObject cam;
    public int method = 1;
    // method 0: simple version(1x)
    // method 1: 2x
    // method 2: 3x
    // method 3: 4x
    // method 4: our method

    // Start is called before the first frame update
    void Start()
    {
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

        /*m_RenderHandle = HvrApi.GetRenderHandle();
        if (m_RenderHandle != null)
        {
            Debug.Log("GetRenderHandle Success!");
        }
        else
        {
            Debug.Log("RenderHandle is null.");
        }*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (method == -1 || method == 0) return;
        if (method < 5)
        {
            float cam_y = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.y;
            Debug.Log(cam_y + ";");
            Vector3 v3 = new Vector3(0, 2 * cam_y, 0);
            cam.transform.localRotation = Quaternion.Euler(v3);
        }
        else
        {
            return;
            // implement our method
        }
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

        /*m_RenderHandle.GetRenderStatics(ref renderStatics);
　　  int submiteFrameRate = (int)renderStatics.SubmitFrameRate; //提交帧率
　　  int renderFrameRate = (int)renderStatics.RenderFrameRate; //渲染帧率
        Debug.Log(renderStatics.RenderFrameRate);

        Debug.Log(Time.deltaTime);*/
    }
}
