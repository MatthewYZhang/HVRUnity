using HVRCORE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GenerateBall : MonoBehaviour
{
    public GameObject base_ball;
    public GameObject base_cube;
    public GameObject base_cylinder;
    public GameObject cam;
    int num;
    public GameObject[] objList;
    private GameObject[] objType;
    // 0: ball, 1: cube, 2: cylinder
    public int[] objCnt;
    float cam_x;
    float cam_y;
    float cam_z;
    float obj_x;
    float obj_y;

    // Start is called before the first frame update
    void Start()
    {
        objCnt = new int[3];
        objType = new GameObject[3];
        base_ball.transform.position = new Vector3(5, 0, 0);
        base_cube.transform.position = new Vector3(0, 0, -5);
        base_cylinder.transform.position = new Vector3(0, 0, 5);
        objCnt[0] = 1; objCnt[1] = 1; objCnt[2] = 1;
        
        objType[0] = base_ball; objType[1] = base_cube; objType[2] = base_cylinder;
        
        // 随机产生多个球 count
        num = Random.Range(11, 14);
        objList = new GameObject[num+3];
        objList[0] = base_ball; objList[1] = base_cube; objList[2] = base_cylinder;

        for (int i = 3; i < num+3; ++i)
        {
            int type;
            do
            {
                type = Random.Range(0, 3);
            } while (objCnt[type] + 1 > 7);
            objCnt[type] += 1;

            Vector3 pos2 = GenerateNewPos(i);
            objList[i] = Instantiate(objType[type], pos2, Quaternion.identity);
        }
        for (int i = 0; i < num + 3; ++i)
        {
            objList[i].GetComponent<Renderer>().material.color = new Color(0.5f, 0.7f, 1f);
        }
        LoadSkybox.SPEED.GetComponent<Text>().text = objCnt[0] * 47 + " " + objCnt[1] * 37 + " " + objCnt[2] * 17;
    }

    Vector3 GenerateNewPos(int i)
    {
        Vector2 p = Random.insideUnitCircle * 1;
        Vector2 pos = p.normalized * (1 + p.magnitude);
        float randHeight = Random.Range(-3f, 3f);
        float angle = 2 * i * Mathf.PI / num;
        float x = Mathf.Sin(angle) * 5;
        float z = Mathf.Cos(angle) * 5;
        Vector3 pos2 = new Vector3(pos.x + x, randHeight, pos.y + z);
        return pos2;
    }

    // Update is called once per frame
    void Update()
    {
        cam_x = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.x;
        cam_y = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.y;
        
        cam_z = HVRLayoutCore.m_CamCtrObj.transform.localRotation.eulerAngles.z;
        Debug.Log("cam_y: " + cam_y + " " + cam_x + " " + cam_z);
        Debug.Log("SkyBoxCam_y: " + LoadSkybox.viewAngle);
        cam_x = cam_x > 180f ? Mathf.Sin((360f - cam_x) * Mathf.PI / 180f) : -Mathf.Sin((cam_x) * Mathf.PI / 180f);
        cam_y = LoadSkybox.viewAngle > 180f ? Mathf.Sin((LoadSkybox.viewAngle - 360f) * Mathf.PI / (180)) : Mathf.Sin(LoadSkybox.viewAngle * Mathf.PI / (180));
        cam_z = LoadSkybox.viewAngle > 180f ? Mathf.Cos((LoadSkybox.viewAngle - 360f) * Mathf.PI / (180)) : Mathf.Cos(LoadSkybox.viewAngle * Mathf.PI / (180));
        Debug.Log("cam_y: " + cam_y + " " + cam_x + " " + cam_z);
        
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
                //tar.transform.position = GenerateNewPos();
            }
        }
        else
        {
            for (int i = 0; i < num+3; ++i)
            {
                objList[i].GetComponent<Renderer>().material.color = new Color(0.5f, 0.7f, 1f);
            }
            
        }
        
        
        // 如果对准了小球，就变色
        
        

        // 对准小球同时按键，就消除

        //
    }
}
