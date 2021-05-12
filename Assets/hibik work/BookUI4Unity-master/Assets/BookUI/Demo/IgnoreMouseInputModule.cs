using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Processing mouse/toutch inputs are omitted from StandAloneInputModule
/// </summary>
public class IgnoreMouseInputModule : BaseInputModule
{
    private float m_PrevActionTime;
    private Vector2 m_LastMoveVector;
    private Vector2 movement = Vector2.zero;
    private int m_ConsecutiveMoveCount = 0;

    private GameObject m_CurrentFocusedGameObject;

    protected IgnoreMouseInputModule() { }

    // 自身が作成した変数
    //========================================
    [SerializeField] private GameObject bookL;
    private Animator bookLAnim;

    [SerializeField] private int pageNum;      // 現在のページ　※開始するページを入力する
    [SerializeField] private int maxPageNum;   // ページの最大値

    private bool nextPageFlg = false;       // true:次のページへ進む
    private bool backPageFlg = false;       // true:前のページに戻る

    private bool allBackFlg = false;        // 一番最初のページに戻る
    private int pageCntInit = 10;
    private int pageCnt = 20;
    private int bookCloseCnt = 90;
    private bool bookCloseFlg = false;

    [SerializeField] private AudioSource pageSeSource;
    //========================================

    [SerializeField]
    private string m_HorizontalAxis = "Horizontal";

    /// <summary>
    /// Name of the vertical axis for movement (if axis events are used).
    /// </summary>
    [SerializeField]
    private string m_VerticalAxis = "Vertical";

    /// <summary>
    /// Name of the submit button.
    /// </summary>
    [SerializeField]
    private string m_SubmitButton = "Submit";

    /// <summary>
    /// Name of the submit button.
    /// </summary>
    [SerializeField]
    private string m_CancelButton = "Cancel";

    [SerializeField]
    private float m_InputActionsPerSecond = 10;

    [SerializeField]
    private float m_RepeatDelay = 0.5f;

    [SerializeField]
    private bool m_ForceModuleActive;

    public bool forceModuleActive
    {
        get { return m_ForceModuleActive; }
        set { m_ForceModuleActive = value; }
    }

    public float inputActionsPerSecond
    {
        get { return m_InputActionsPerSecond; }
        set { m_InputActionsPerSecond = value; }
    }

    public float repeatDelay
    {
        get { return m_RepeatDelay; }
        set { m_RepeatDelay = value; }
    }

    /// <summary>
    /// Name of the horizontal axis for movement (if axis events are used).
    /// </summary>
    public string horizontalAxis
    {
        get { return m_HorizontalAxis; }
        set { m_HorizontalAxis = value; }
    }

    /// <summary>
    /// Name of the vertical axis for movement (if axis events are used).
    /// </summary>
    public string verticalAxis
    {
        get { return m_VerticalAxis; }
        set { m_VerticalAxis = value; }
    }

    public string submitButton
    {
        get { return m_SubmitButton; }
        set { m_SubmitButton = value; }
    }

    public string cancelButton
    {
        get { return m_CancelButton; }
        set { m_CancelButton = value; }
    }

    private bool ShouldIgnoreEventsOnNoFocus()
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Windows:
            case OperatingSystemFamily.Linux:
            case OperatingSystemFamily.MacOSX:
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isRemoteConnected)
                    return false;
#endif
                return true;
            default:
                return false;
        }
    }

    public override bool ShouldActivateModule()
    {
        if (!base.ShouldActivateModule())
            return false;

        var shouldActivate = m_ForceModuleActive;
        shouldActivate |= input.GetButtonDown(m_SubmitButton);
        shouldActivate |= input.GetButtonDown(m_CancelButton);
        shouldActivate |= !Mathf.Approximately(input.GetAxisRaw(m_HorizontalAxis), 0.0f);
        shouldActivate |= !Mathf.Approximately(input.GetAxisRaw(m_VerticalAxis), 0.0f);

        return shouldActivate;
    }

    public override void ActivateModule()
    {
        if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
            return;

        base.ActivateModule();

        var toSelect = eventSystem.currentSelectedGameObject;
        if (toSelect == null)
            toSelect = eventSystem.firstSelectedGameObject;

        eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
    }

    public override void Process()
    {
        if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
            return;

        bool usedEvent = SendUpdateEventToSelectedObject();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }
    }

    /// <summary>
    /// Process submit keys.
    /// </summary>
    protected bool SendSubmitEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        if (input.GetButtonDown(m_SubmitButton))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

        if (input.GetButtonDown(m_CancelButton))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
        return data.used;
    }

    private Vector2 GetRawMoveVector()
    {
        Vector2 move = Vector2.zero;
        move.x = input.GetAxisRaw(m_HorizontalAxis);
        move.y = input.GetAxisRaw(m_VerticalAxis);

        if (input.GetButtonDown(m_HorizontalAxis))
        {
            if (move.x < 0)
                move.x = -1f;
            if (move.x > 0)
                move.x = 1f;
        }
        if (input.GetButtonDown(m_VerticalAxis))
        {
            if (move.y < 0)
                move.y = -1f;
            if (move.y > 0)
                move.y = 1f;
        }
        return move;
    }

    /// <summary>
    /// Process keyboard events.
    /// </summary>
    protected bool SendMoveEventToSelectedObject()
    {
        float time = Time.unscaledTime;

        // 編集箇所
        //========================================
        //Vector2 movement = GetRawMoveVector();
        movement = Vector2.zero;
        if (pageNum < maxPageNum && nextPageFlg) PageChange("next");
        if (pageNum > 0 && backPageFlg) PageChange("back");
        if (allBackFlg) PageChange("back");
        //========================================

        if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
        {
            m_ConsecutiveMoveCount = 0;
            return false;
        }

        // If user pressed key again, always allow event
        bool allow = input.GetButtonDown(m_HorizontalAxis) || input.GetButtonDown(m_VerticalAxis);
        allow = false;
        bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);
        if (!allow)
        {
            // Otherwise, user held down key or axis.
            // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
            if (similarDir && m_ConsecutiveMoveCount == 1)
                allow = (time > m_PrevActionTime + m_RepeatDelay);
            // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
            else
                allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond);
        }
        if (!allow)
            return false;

        // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
        var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);

        if (axisEventData.moveDir != MoveDirection.None)
        {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            if (!similarDir)
                m_ConsecutiveMoveCount = 0;
            m_ConsecutiveMoveCount++;
            m_PrevActionTime = time;
            m_LastMoveVector = movement;
        }
        else
        {
            m_ConsecutiveMoveCount = 0;
        }

        return axisEventData.used;
    }

    protected bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    protected GameObject GetCurrentFocusedGameObject()
    {
        return m_CurrentFocusedGameObject;
    }

    // 自身が作成した関数
    //========================================
    public int GetPageNum()
    {
        return pageNum;
    }

    public void NextPage()
    {
        nextPageFlg = true;
    }

    public void BackPage()
    {
        backPageFlg = true;
    }

    public void AllBackPage()
    {
        allBackFlg = true;
    }

    public bool GetAllBackFlg()
    {
        return allBackFlg;
    }

    // ページ変更　next:次のページ　back:前のページ
    public void PageChange(string str)
    {
        //Debug.Log(str);
        nextPageFlg = false;
        backPageFlg = false;

        switch (str)
        {
            case "next":
                if (pageNum < maxPageNum)
                {
                    pageNum++;
                    movement.x++;
                    pageSeSource.Play();
                }
                break;

            case "back":
                if (allBackFlg && pageNum <= 1)
                {
                    if (bookCloseCnt == 60)
                    {
                        bookLAnim = bookL.GetComponent<Animator>();
                        bookLAnim.SetBool("isAnim", false);
                    }
                    if (bookCloseCnt == 0) bookCloseFlg = true;
                    else bookCloseCnt--;
                }

                if (allBackFlg && pageCnt == 0)
                {
                    pageNum--;
                    movement.x--;
                    pageCnt = pageCntInit;
                }
                else pageCnt--;


                if (!allBackFlg && pageNum > 1)
                {
                    pageNum--;
                    movement.x--;
                    pageSeSource.Play();
                }
                break;
        }
    }

    public bool GetBookCloseFlg()
    {
        return bookCloseFlg;
    }
    //========================================
}
