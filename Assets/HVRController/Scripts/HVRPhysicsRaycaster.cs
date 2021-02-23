using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HVRCORE;

public class HVRPhysicsRaycaster : HVRRaycasterBase
{
    protected const int NO_EVENT_MASK_SET = -1;

    private Camera cachedEventCamera;

    [SerializeField] protected LayerMask raycasterEventMask = NO_EVENT_MASK_SET;

    protected override void Awake()
    {
        base.Awake();
        switch (transform.parent.name)
        {
            case "HVRRightController":
                controllerIndex = ControllerIndex.RIGHT_CONTROLLER;
                break;
            case "HVRLeftController":
                controllerIndex = ControllerIndex.LEFT_CONTROLLER;
                break;
            default:
                controllerIndex = ControllerIndex.NONE;
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        HVRInputModule.AddRaycaster(this);
    }

    protected override void OnDisable()
    {
        HVRInputModule.RemoveRaycaster(this);
    }

    public override Camera eventCamera
    {
        get
        {
            if (cachedEventCamera == null)
            {
                cachedEventCamera = GetComponent<Camera>();
            }
            if (cachedEventCamera == null) {
            }
            return cachedEventCamera!= null ? cachedEventCamera: Camera.main;
        }
    }

    public LayerMask eventMask
    {
        get { return raycasterEventMask; }
        set { raycasterEventMask = value; }
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (eventCamera == null)
        {
            return;
        }
        var ray = GetRay();
        var dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

        var hits = Physics.RaycastAll(ray, dist, eventMask);

        if (hits.Length > 1)
        {
            Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));
        }

        if (hits.Length != 0)
        {
            for (int b = 0, bmax = hits.Length; b < bmax; ++b)
            {
                var result = new RaycastResult
                {
                    gameObject = hits[b].collider.gameObject,
                    module = this,
                    distance = hits[b].distance,
                    worldPosition = hits[b].point,
                    worldNormal = hits[b].normal,
                    screenPosition = eventData.position,
                    index = resultAppendList.Count,
                    sortingLayer = 0,
                    sortingOrder = 0
                };
                resultAppendList.Add(result);
            }
        }
    }
}