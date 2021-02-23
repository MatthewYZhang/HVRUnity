using UnityEngine;
using UnityEngine.EventSystems;
using HVRCORE;
using System.Collections;
using System.Collections.Generic;

public class HVRInputModule : BaseInputModule
{
    private static string TAG = "HVRInputModule";
    private Camera m_UiCamera;
    private static Vector3 m_MousePosition;
    private static float m_MoveMagnitude = 3.0f;

    /// <summary>
    /// The pointer whose button is clicked will be set as active. Only one active pointer at the same time.
    /// </summary>
    public static HVRLinePointer activePointer;

    public static List<HVRLinePointer> m_LinePointers = new List<HVRLinePointer>();

    public static List<HVRRaycasterBase> m_AddedRaycastersCache = new List<HVRRaycasterBase>();

    public static List<HVRRaycasterBase> m_RemovedRaycastersCache = new List<HVRRaycasterBase>();

    /// <summary>
    /// Add pointer from HVRLinePointer. Generally only add tow instances, left and right, of HVRLinePointer.
    /// </summary>
    /// <param name="point"></param>
    public static void AddLinePoint(HVRLinePointer point)
    {
        if (!m_LinePointers.Contains(point))
            m_LinePointers.Add(point);

        if (activePointer == null)
            activePointer = point;
    }

    public static void RemoveLinePoint(HVRLinePointer point)
    {
        if (m_LinePointers.Contains(point))
            m_LinePointers.Remove(point);

        if (activePointer == point)
            activePointer = m_LinePointers.Count>0? m_LinePointers[0]:null;
    }

    public static void AddRaycaster(HVRRaycasterBase raycast)
    {
        m_AddedRaycastersCache.Add(raycast);
    }

    public static void RemoveRaycaster(HVRRaycasterBase raycast)
    {
        m_RemovedRaycastersCache.Add(raycast);
    }

    /// <summary>
    /// Every raycaster must specify a pointer to response.
    /// </summary>
    /// <param name="raycast"></param>
    /// <param name="isAdd"></param>
    public void DispatchRaycaster(HVRRaycasterBase raycast,bool isAdd)
    {
        foreach (var pointer in m_LinePointers) {
            if (pointer.controllerIndex == raycast.controllerIndex) {
                if (isAdd) {
                    pointer.AddRaycaster(raycast);
                } else {
                    pointer.RemoveRaycaster(raycast);
                }             
                break;
            }
        }
    }

    public override void Process()
    {
        if (m_AddedRaycastersCache.Count > 0)
        {
            List<HVRRaycasterBase> tmp = new List<HVRRaycasterBase>(m_AddedRaycastersCache);
            foreach (var raycast in tmp)
            {
                DispatchRaycaster(raycast, true);
                m_AddedRaycastersCache.Remove(raycast);
            }
        }

        if (m_RemovedRaycastersCache.Count > 0)
        {
            List<HVRRaycasterBase> tmp = new List<HVRRaycasterBase>(m_RemovedRaycastersCache); ;
            foreach (var raycast in tmp)
            {
                DispatchRaycaster(raycast, false);
                m_RemovedRaycastersCache.Remove(raycast);
            }
        }

        StartCoroutine(DelayProcess());
    }

    private IEnumerator DelayProcess() {
        yield return null;
        if (HVRController.m_RightEventCamera == null)
        {
            yield return null;
        }
        processPointers(m_LinePointers);
        UpdateCurrentObject(activePointer);
    }

    private void processPointers(List<HVRLinePointer> pointers)
    {
        m_UiCamera = HVRController.m_RightEventCamera.GetComponent<Camera>();
        foreach (var pointer in pointers)
        {
            if (pointer.gameObject.activeInHierarchy && pointer.enabled)
            {
                RaycastResult raycastResult = GetRaycastResult(pointer);
                pointer.m_LastGazeObj = pointer.m_NowGazeObj;
                pointer.m_NowGazeObj = raycastResult.gameObject;

                if (pointer.m_PointerEventData == null)
                {
                   continue;
                }

                Vector2 pointerPosition;
                pointerPosition.x = m_UiCamera.pixelWidth / 2;
                pointerPosition.y = m_UiCamera.pixelHeight / 2;
                pointer.m_PointerEventData.Reset();
                pointer.m_PointerEventData.pointerCurrentRaycast = raycastResult;
                pointer.m_PointerEventData.position = pointerPosition;

                UpdateAnchorPos(pointer, raycastResult.worldPosition);

                if (Application.platform == RuntimePlatform.Android)
                {
                    if (IsRightControllerButtonDown() && pointer.controllerIndex == ControllerIndex.RIGHT_CONTROLLER)
                    {
                        this.HandleTrigger(pointer);
                        activePointer = pointer;
                    }
                    else if (IsRightControllerButtonUp() && pointer.controllerIndex == ControllerIndex.RIGHT_CONTROLLER)
                    {
                        this.HandlePendingClick(pointer);
                        activePointer = pointer;
                    }

                    if (IsLeftControllerButtonDown() && pointer.controllerIndex == ControllerIndex.LEFT_CONTROLLER)
                    {
                        this.HandleTrigger(pointer);
                        activePointer = pointer;
                    }
                    else if (IsLeftControllerButtonUp() && pointer.controllerIndex == ControllerIndex.LEFT_CONTROLLER)
                    {
                        this.HandlePendingClick(pointer);
                        activePointer = pointer;
                    }
                }
                else if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        this.HandleTrigger(pointer);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        this.HandlePendingClick(pointer);
                    }
                }

                ProcessDrag(pointer);
                ProcessHover(pointer);
                ProcessMove(pointer);
            }
        }
    }


    private  RaycastResult GetRaycastResult(HVRLinePointer pointer)
    {
        List<RaycastResult> raycastResultCache = new List<RaycastResult>();
        pointer.RaycastAll(raycastResultCache); 
        raycastResultCache.Sort((o1,o2)=>{
            if (o1.sortingLayer < o2.sortingLayer) { return 1; }
            if (o1.sortingLayer > o2.sortingLayer) { return -1; }
            if (o1.sortingOrder < o2.sortingOrder) { return 1; }
            if (o1.sortingOrder > o2.sortingOrder) { return -1; }
            if (o1.depth < o2.depth) { return 1; }
            if (o1.depth > o2.depth) { return -1; }
            if (o1.distance > o2.distance) { return 1; }
            if (o1.distance < o2.distance) { return -1; }
            if (o1.index > o2.index) { return 1; }
            if (o1.index < o2.index) { return -1; }
            return 0;
        });
        return FindFirstRaycast(raycastResultCache); 
    }


    private bool IsLeftControllerButtonDown()
    {
        if (HVRController.m_LeftController == null || !HVRController.m_LeftController.IsAvailable())
        {
            return false;
        }
        if ((HVRController.m_LeftController.IsButtonDown(ButtonType.ButtonConfirm) || HVRController.m_LeftController.IsButtonDown(ButtonType.ButtonTrigger)))
        {
            return true;
        }
        return false;
    }

    private bool IsLeftControllerButtonUp()
    {
        if (HVRController.m_LeftController == null || !HVRController.m_LeftController.IsAvailable())
        {
            return false;
        }
        if ((HVRController.m_LeftController.IsButtonUp(ButtonType.ButtonConfirm) || HVRController.m_LeftController.IsButtonUp(ButtonType.ButtonTrigger)))
        {
            return true;
        }
        return false;
    }
    private bool IsRightControllerButtonDown()
    {
        if (HVRController.m_RightController == null || !HVRController.m_RightController.IsAvailable())
        {
            return false;
        }
        if ((HVRController.m_RightController.IsButtonDown(ButtonType.ButtonConfirm) || HVRController.m_RightController.IsButtonDown(ButtonType.ButtonTrigger)))
        {
            return true;
        }
        return false;
    }

    private bool IsRightControllerButtonUp()
    {
        if (HVRController.m_RightController == null || !HVRController.m_RightController.IsAvailable())
        {
            return false;
        }
        if ((HVRController.m_RightController.IsButtonUp(ButtonType.ButtonConfirm) || HVRController.m_RightController.IsButtonUp(ButtonType.ButtonTrigger)))
        {
            return true;
        }
        return false;
    }

    private void UpdateCurrentObject(HVRLinePointer pointer)
    {
        if (pointer == null) {
            return;
        }
        GameObject target = pointer.m_PointerEventData.pointerCurrentRaycast.gameObject;

        HandlePointerExitAndEnter(pointer.m_PointerEventData, target); //Controller sending enter and exit events when a new enter target is found.

        GameObject select = ExecuteEvents.GetEventHandler<ISelectHandler>(target);

        if (select == eventSystem.currentSelectedGameObject)
        {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
        }
        else
        {
            eventSystem.SetSelectedGameObject(null, pointer.m_PointerEventData);
        }
    }

    private void UpdateAnchorPos(HVRLinePointer pointer, Vector3 intersectionPosition)
    {
        if (pointer == null)
        {
            return;
        }
        GameObject currentGazeObject = pointer.m_NowGazeObj;
        GameObject previousGazedObject = pointer.m_LastGazeObj;

        // Hack here,use remote to control pointer
        if (currentGazeObject == previousGazedObject)
        {         
            if (currentGazeObject != null)
            {
                pointer.OnLineHover(intersectionPosition, true);
            }
            
            try
            {
                if (previousGazedObject.ToString().Equals("null"))
                {
                    pointer.OnLineExit(intersectionPosition, false);
                }
            }
            catch (System.NullReferenceException ex)
            {
            }
            
        }
        else
        {
            if (previousGazedObject != null)
            {
                pointer.OnLineExit(intersectionPosition, false);
            }

            if (currentGazeObject != null)
            {
                pointer.OnLineEnter(intersectionPosition, true);
            }
        }
    }

    private void HandlePendingClick(HVRLinePointer pointer)
    {
        if (!pointer.m_PointerEventData.eligibleForClick)
        {
            return;
        }

        GameObject mhitObject = pointer.m_PointerEventData.pointerCurrentRaycast.gameObject;

        if (pointer.m_CurrentDragging)
        {
            ExecuteEvents.Execute(pointer.m_CurrentDragging, pointer.m_PointerEventData, ExecuteEvents.endDragHandler);

            ExecuteEvents.ExecuteHierarchy(pointer.m_CurrentDragging, pointer.m_PointerEventData, ExecuteEvents.dropHandler);

            pointer.m_PointerEventData.pointerDrag = null;
            pointer.m_CurrentDragging = null;
        }

        if (pointer.m_ClickedDownObj)
        {
            m_MousePosition -= Input.mousePosition;
            if (Application.platform == RuntimePlatform.Android)
            {
                if (pointer != null)
                {
                    if (pointer.controllerIndex == ControllerIndex.RIGHT_CONTROLLER)
                    {
                        HVRController.m_RightController.GetTouchpadTouchPos(ref pointer.m_TouchUpPos);
                    }
                    else
                    {
                        HVRController.m_LeftController.GetTouchpadTouchPos(ref pointer.m_TouchUpPos);
                    }
                }

                pointer.m_DifferPos = pointer.m_TouchDownPos - pointer.m_TouchUpPos;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                pointer.m_DifferPos = Vector2.zero;
            }

            GameObject clickedUpObj = ExecuteEvents.ExecuteHierarchy(pointer.m_ClickedDownObj, pointer.m_PointerEventData, ExecuteEvents.pointerUpHandler);
            if (pointer.m_ClickedDownObj == clickedUpObj && (m_MousePosition.magnitude < m_MoveMagnitude) && (pointer.m_DifferPos.magnitude < 0.1f))
            {
                ExecuteEvents.Execute(pointer.m_ClickedDownObj, pointer.m_PointerEventData, ExecuteEvents.pointerClickHandler);
            }
            else
            {
                ExecuteEvents.Execute(pointer.m_ClickedDownObj, pointer.m_PointerEventData, ExecuteEvents.pointerUpHandler);
            }

            pointer.m_PointerEventData.pressPosition = Vector3.zero;
            pointer.m_PointerEventData.pointerPress = null;
            pointer.m_PointerEventData.rawPointerPress = null;
            pointer.m_PointerEventData.eligibleForClick = false;
            pointer.m_PointerEventData.clickCount = 0;
            pointer.m_ClickedDownObj = null;
            pointer.m_PointerEventData.useDragThreshold = false;
        }
    }

    private void HandleTrigger(HVRLinePointer pointer)
    {
        GameObject mtriggerObject = pointer.m_PointerEventData.pointerCurrentRaycast.gameObject;

        pointer.m_PointerEventData.pressPosition = pointer.m_PointerEventData.position;
        pointer.m_PointerEventData.pointerPressRaycast = pointer.m_PointerEventData.pointerCurrentRaycast;

        pointer.m_PointerEventData.pointerPress = ExecuteEvents.ExecuteHierarchy(mtriggerObject, pointer.m_PointerEventData, ExecuteEvents.pointerDownHandler)
            ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(mtriggerObject);

        if (Application.platform == RuntimePlatform.Android)
        {
            if (pointer.controllerIndex == ControllerIndex.RIGHT_CONTROLLER)
            {
                HVRController.m_RightController.GetTouchpadTouchPos(ref pointer.m_TouchDownPos);
            }
            else
            {
                HVRController.m_LeftController.GetTouchpadTouchPos(ref pointer.m_TouchDownPos);
            }
        }
        m_MousePosition = Input.mousePosition;
        pointer.m_ClickedDownObj = pointer.m_PointerEventData.pointerPress;
        pointer.m_PointerEventData.rawPointerPress = mtriggerObject;
        pointer.m_PointerEventData.eligibleForClick = true;
        pointer.m_PointerEventData.delta = Vector2.zero;
        pointer.m_PointerEventData.dragging = false;
        pointer.m_PointerEventData.useDragThreshold = true;
        pointer.m_PointerEventData.clickCount = 1;
        pointer.m_PointerEventData.clickTime = Time.unscaledTime;
    }

    private void ProcessDrag(HVRLinePointer pointer)
    {
        GameObject mDragObj = pointer.m_NowGazeObj;
        if (pointer.m_PointerEventData.useDragThreshold)
        {
            bool isBeginDrag = ExecuteEvents.Execute(pointer.m_ClickedDownObj, pointer.m_PointerEventData, ExecuteEvents.beginDragHandler);
            if (isBeginDrag)
            {
                pointer.m_PointerEventData.pointerDrag = pointer.m_ClickedDownObj;
                pointer.m_CurrentDragging = pointer.m_ClickedDownObj;
            }
        }
        if (pointer.m_CurrentDragging)
        {
            ExecuteEvents.Execute(pointer.m_CurrentDragging, pointer.m_PointerEventData, ExecuteEvents.dragHandler);
        }
    }

    private void ProcessHover(HVRLinePointer ponter)
    {
        GameObject mCurrentObj = ponter.m_PointerEventData.pointerCurrentRaycast.gameObject;

        if (mCurrentObj == ponter.m_LastGazeObj && mCurrentObj != null)
        {
            ExecuteEvents.ExecuteHierarchy(mCurrentObj, ponter.m_PointerEventData, HVRExecuteEventsExtension.pointerHoverHandler);
            ExecuteEvents.Execute(mCurrentObj, ponter.m_PointerEventData, HVRExecuteEventsExtension.pointerHoverHandler);
        }
    }

    private void ProcessMove(HVRLinePointer pointer)
    {
        if (pointer == null)
        {
            return;
        }
        IController controller = null;
        if (pointer.controllerIndex == ControllerIndex.RIGHT_CONTROLLER)
        {
            controller = HVRController.m_RightController;
        }
        else
        {
            controller = HVRController.m_LeftController;
        }
        if (controller == null || !controller.IsAvailable())
        {
            return;
        }
        GameObject nowGazedObj = pointer.m_PointerEventData.pointerCurrentRaycast.gameObject;
        GameObject selectedDownObj;
        if (nowGazedObj != null)
        {
            if (controller.IsTouchpadTouchDown())
            {
                selectedDownObj = null;
                selectedDownObj = ExecuteEvents.ExecuteHierarchy(nowGazedObj, pointer.m_PointerEventData, ExecuteEvents.selectHandler);
                eventSystem.SetSelectedGameObject(selectedDownObj);

            }
            else if (controller.IsTouchpadTouchUp())
            {
                selectedDownObj = null;
            }
            Vector2 touchPos = new Vector2(0.0f, 0.0f);
            if (eventSystem.currentSelectedGameObject)
            {
                controller.GetTouchpadTouchPos(ref touchPos);
                AxisEventData axisData = GetAxisEventData(touchPos.x - 0.5f, -(touchPos.y - 0.5f), 0.0f);
                axisData.moveDir = MoveDirection.None;

                if (controller.IsTouchpadSwipeLeft())
                {
                    axisData.moveDir = MoveDirection.Left;
                }
                else if (controller.IsTouchpadSwipeRight())
                {
                    axisData.moveDir = MoveDirection.Right;
                }
                else if (controller.IsTouchpadSwipeUp())
                {
                    axisData.moveDir = MoveDirection.Up;

                }
                else if (controller.IsTouchpadSwipeDown())
                {
                    axisData.moveDir = MoveDirection.Down;
                }

                if (axisData.moveDir != MoveDirection.None)
                {
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisData, ExecuteEvents.moveHandler);
                }

            }
        }
    }


    private Vector3 GetIntersectionPosition(Camera cam, RaycastResult raycastResult)
    {
        // Check for camera
        if (cam == null)
        {
            return Vector3.zero;
        }

        float intersectionDistance = raycastResult.distance + cam.nearClipPlane;
        Vector3 intersectionPosition = cam.transform.position + (cam.transform.forward * intersectionDistance);
        return intersectionPosition;
    }
}
