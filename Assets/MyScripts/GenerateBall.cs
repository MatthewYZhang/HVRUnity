using HVRCORE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenerateBall : MonoBehaviour
{
    public GameObject obj;
    public GameObject cam;
    int num;
    public GameObject[] objList;
    float cam_x;
    float cam_y;
    float cam_z;
    float obj_x;
    float obj_y;

    // Start is called before the first frame update
    void Start()
    {
        obj.transform.position = new Vector3(3, 1, 3);

        // 随机产生多个球 count
        /*
        num = Random.Range(11, 14);
        objList = new GameObject[num];
        for (int i = 0; i < num; ++i)
        {
            Vector2 p = Random.insideUnitCircle * 5;
            Vector2 pos = p.normalized * (5 + p.magnitude);
            float randHeight = Random.Range(-3f, 3f);
            Vector3 pos2 = new Vector3(pos.x, randHeight, pos.y);
            
            objList[i] = Instantiate(obj, pos2, Quaternion.identity);
        }
        */
    }

    Vector3 GenerateNewPos()
    {
        Vector2 p = Random.insideUnitCircle * 5;
        Vector2 pos = p.normalized * (5 + p.magnitude);
        float randHeight = Random.Range(-3f, 3f);
        Vector3 pos2 = new Vector3(pos.x, randHeight, pos.y);
        return pos2;
    }

    // Update is called once per frame
    void Update()
    {
        cam_x = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.x;
        cam_y = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.y;
        
        cam_z = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.z;
        Debug.Log("cam_y: " + cam_y + " " + cam_x + " " + cam_z);
        Debug.Log("SkyBoxCam_y: " + LoadSkybox.cam_y);
        cam_x = cam_x > 180f ? Mathf.Sin((360f - cam_x) * Mathf.PI / 180f) : -Mathf.Sin((cam_x) * Mathf.PI / 180f);
        cam_y = LoadSkybox.cam_y > 180f ? Mathf.Sin((LoadSkybox.cam_y - 360f) * Mathf.PI / (180)) : Mathf.Sin(LoadSkybox.cam_y * Mathf.PI / (180));
        cam_z = LoadSkybox.cam_y > 180f ? Mathf.Cos((LoadSkybox.cam_y - 360f) * Mathf.PI / (180)) : Mathf.Cos(LoadSkybox.cam_y * Mathf.PI / (180));
        Debug.Log("cam_y: " + cam_y + " " + cam_x + " " + cam_z);
        Debug.Log("obj at: " + obj.transform.position.normalized);
        Vector3 dir = new Vector3(cam_y, cam_x, cam_z);

        //Debug.Log(dir);
        Ray ray = new Ray(cam.transform.position, dir);
        RaycastHit hit;
        Debug.Log("ray! " + ray);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("HIT!!!");
            GameObject tar = hit.collider.gameObject;
            tar.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f);
            bool triggerPressed = LoadSkybox.controller != null ? LoadSkybox.controller.IsButtonPressed(ButtonType.ButtonTrigger) : false;
            if (triggerPressed)
            {
                tar.transform.position = GenerateNewPos();
            }
        }
        else
        {
            obj.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f);
        }
        Debug.Log("obj Euler angles:" + obj.transform.localPosition);
        
        
        // 如果对准了小球，就变色
        
        

        // 对准小球同时按键，就消除

        //
    }
}
