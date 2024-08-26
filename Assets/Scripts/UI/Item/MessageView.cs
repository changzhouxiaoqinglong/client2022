
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 信息展示界面
/// </summary>
public class MessageView : MonoBehaviour
{
    /// <summary>
    /// 标签
    /// </summary>
    public MessageTitle[] titles;

    /// <summary>
    /// 当前选中的标签
    /// </summary>
    private MessageTitle curSelect;

    /// <summary>
    /// 流程标签
    /// </summary>
    private MessageTitle processTitle;

    private Transform basicInfo;
    /// <summary>
    /// 显示底部操作信息按钮
    /// </summary>
    private bool isMaxInfo = true;

    /// <summary>
    /// 底部信息框max
    /// </summary>
    private ButtonBase MaxInfoBtn;

    /// <summary>
    /// 底部信息框min
    /// </summary>
    private ButtonBase MinInfoBtn;
    private Text tasktype;
    private void Awake()
    {
        titles = GetComponentsInChildren<MessageTitle>();
     //   tasktype= transform.Find("tasktype").GetComponent<Text>();
        processTitle = transform.Find("提示信息框/Scroll View").GetComponent<MessageTitle>();
        basicInfo = transform.Find("底部信息框/BasicInformation/Viewport/Content");
        // EventDispatcher.GetInstance().AddEventListener(EventNameList.CLICK_MSG_TITLE, OnClickTitle);
        if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType == CheckTypeConst.PRACTICE)
            EventDispatcher.GetInstance().AddEventListener(EventNameList.REF_SHOW_TASK_LOG, RefreshLog);
    }

    private void Start()
    {
        /*
		#region old
		//初始选中任务描述标签
		SelectTitle(MsgTitleType.TaskDesc);
        InitTaskDesc();
        //刷新日志
        RefreshLog(null);
        //初始化流程
        InitProcessUI();
        #endregion
        */

        #region 新版修改
        //初始选中任务描述标签
       // SelectTitle(MsgTitleType.TaskDesc);
        //  InitTaskDesc();任务说明不要了
        InitBasicInformation();
        //刷新日志
        RefreshLog(null);
        //初始化流程，就是提示信息
        InitProcessUI();//

        MaxInfoBtn = transform.Find("底部信息框/BtnMax").GetComponent<ButtonBase>();
        MaxInfoBtn.RegistClick(OnClickMaxInfoBtn);
        MinInfoBtn = transform.Find("底部信息框/BtnMin").GetComponent<ButtonBase>();
        MinInfoBtn.RegistClick(OnClickMinInfoBtn);

        #endregion
    }

    /// <summary>
    /// 初始化任务描述
    /// </summary>
    public void InitTaskDesc()
    {
        GetTitleByType(MsgTitleType.TaskDesc).SetText(NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.TaskDesc);
    }

    void OnGetBeiDouData(IEventParam param)
	{
        if (param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            BeiDouModel model = JsonTool.ToObject<BeiDouModel>(tcpReceiveEvParam.netData.Msg);           
            basicInfo.Find("currentpos/value").GetComponent<Text>().text = "经度：" + model.Longicude + "，纬度：" + model.Latitude + "，高程："+model.Elevation.ToString("F2");
        }
    }

    /// <summary>
    /// 初始化训练流程
    /// </summary>
    private void InitProcessUI()
    {
        print("初始化训练流程_CheckType"+ NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType);
        print("初始化训练流程_TaskType" + NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.TaskType);
        if(TaskMgr.GetInstance().curTaskData.Type == TaskType.Tactic)//战术训练
		{
            processTitle.transform.parent.gameObject.SetActive(false);

            NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.BEIDOU_DATA, OnGetBeiDouData);


            
            return;
        }

        //单击和考核没有基础训练
        if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType != CheckTypeConst.PRACTICE)
        {
            processTitle.transform.parent.gameObject.SetActive(false);
            //processTitle.SetText("当前为考核模式不显示具体操作信息");
            return;
        }

  //      if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.TaskType == 3)
		//{
  //          processTitle.transform.parent.gameObject.SetActive(false);         
  //          //processTitle.SetText("当前为考核模式不显示具体操作信息");
  //          return;
  //      }

            int taskId = TaskMgr.GetInstance().curTaskData.Id;
        //本次训练流程数据
        List<ExPracticeProcess> processList = ExPracticeProcessMgr.GetInstance().GetProcessByTaskId(taskId);
        if (processList.Count > 0)
        {
            processTitle.SetText(ExPracticeProcessMgr.GetInstance().GetProcessDesc(taskId));
        }
        else
        {
            processTitle.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    ///  初始化基本信息
    /// </summary>
    private void InitBasicInformation()
	{
        basicInfo.Find("tasktype/value").GetComponent<Text>().text = TaskMgr.GetInstance().curTaskData.Desc;
        //基本操作不下显示风向风速温度湿度
        if (TaskMgr.GetInstance().curTaskData.Type != TaskType.Tactic)
        {
            return;
        }
        //float WindDirOff = 5;
        //float WindSpOff = 0.3f;
        var weather = NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Wearth;
        ////风向浮动
        //float windDir = weather.GetWindDir();
        //windDir += WindDirOff * Random.Range(-1f, 1f);
        //windDir = Mathf.Clamp(windDir, 0, 359);

        ////风速浮动
        //float windSp = weather.GetWindSp();
        //windSp += WindSpOff * Random.Range(-1f, 1f);
        //windSp = windSp < 0 ? 0 : windSp;



        // basicInfo.Find("tasktype/value")?.GetComponent<Text>().text=;

        //      tasktype = transform.Find("tasktype").GetComponent<Text>();
        //      tasktype.gameObject.SetActive(true);
        //      if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType == CheckTypeConst.EXAMINE)
        //{
        //          tasktype.text = "考核模式";
        //      }
        //      else if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType == CheckTypeConst.PRACTICE)
        //      {
        //          tasktype.text = "训练模式";
        //      }

        
        if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType == CheckTypeConst.EXAMINE)
        {
            transform.Find("kaohe").gameObject.SetActive(true);
        }
        else if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType == CheckTypeConst.PRACTICE)
        {
            transform.Find("xunlian").gameObject.SetActive(true);
        }
        else if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType == CheckTypeConst.PK)
        {
            transform.Find("biwu").gameObject.SetActive(true);
        }


        basicInfo.Find("weather/value").GetComponent<Text>().text = weather.GetDes() + ","+ "温度" +weather.Temperate+","+ "湿度" + weather.Humidity + ","+ GetWindDir()+",风速"+weather.GetWindSp();

    }

	private void LateUpdate()
	{
        if (SceneMgr.GetInstance().curScene is Train3DSceneCtrBase scene3D)
        {
            Vector3 lation = scene3D.terrainChangeMgr.gisPointMgr.GetGisPos(scene3D.miniMapMgr.MiniMapCamera.GetPoint());
            //高程
            float elevat = scene3D.terrainChangeMgr.GetEvelationByPos(scene3D.miniMapMgr.MiniMapCamera.GetPoint());
            if (lation != null)
            {
                basicInfo.Find("currentpos/value").GetComponent<Text>().text = "经度：" + lation.x + "，纬度：" + lation.y + "，高程：" + elevat.ToString("F2");
            }
        }
        else
		{
            if (TaskMgr.GetInstance().curTaskData.Type == TaskType.Tactic)
            {
               // basicInfo.Find("currentpos/value").GetComponent<Text>().text = "经度：" + lation.y + "，纬度：" + lation.x;
            }
        }
    }

	/// <summary>
	/// 获取当前风向
	/// </summary>
	/// <returns></returns>
	private string GetWindDir() {
        string windDir = "";
        Wearth curWearth = NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Wearth;
        //switch (curWearth.WindDir) {
        //    case 0:
        //        windDir = "北风";
        //        break;
        //    case 1:
        //        windDir = "东北风";
        //        break;
        //    case 2:
        //        windDir = "东风";
        //        break;
        //    case 3:
        //        windDir = "东南风";
        //        break;
        //    case 4:
        //        windDir = "南风";
        //        break;
        //    case 5:
        //        windDir = "西南风";
        //        break;
        //    case 6:
        //        windDir = "西风";
        //        break;
        //    case 7:
        //        windDir = "西北风";
        //        break;
        //    default:
        //        break;
        //}

        //if (curWearth.WindDir == 0 || curWearth.WindDir == 360)
        //{
        //    windDir = "北风";
        //}
        //else if (curWearth.WindDir > 0 && curWearth.WindDir < 90)
        //{
        //    windDir = "东北风";
        //}
        //else if (curWearth.WindDir == 90)
        //{
        //    windDir = "东风";
        //}
        //else if (curWearth.WindDir > 90 && curWearth.WindDir < 180)
        //{
        //    windDir = "东南风";
        //}
        //else if (curWearth.WindDir == 180)
        //{
        //    windDir = "南风";
        //}
        //else if (curWearth.WindDir > 180 && curWearth.WindDir < 270)
        //{
        //    windDir = "西南风";
        //}
        //else if (curWearth.WindDir == 270)
        //{
        //    windDir = "西风";
        //}
        //else if (curWearth.WindDir > 270 && curWearth.WindDir < 360)
        //{
        //    wind//}Dir = "西北风";


        return " 风向：" + curWearth.WindDir.ToString() + "°";
        return windDir;
    }

    /// <summary>
    /// 刷新日志
    /// </summary>
    private void RefreshLog(IEventParam param)
    {
        if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.CheckType != CheckTypeConst.PRACTICE)
		{
            GetTitleByType(MsgTitleType.Log).SetText("当前为考核模式不显示具体操作信息");
            return;
        }
        GetTitleByType(MsgTitleType.Log).SetText(TaskMgr.GetInstance().curTaskCtr.GetTrainLog());
    }

    private void SelectTitle(MsgTitleType type)
    {
        MessageTitle title = GetTitleByType(type);
        if (title)
        {
            SelectTitle(title);
        }
    }

    /// <summary>
    /// 选择标签
    /// </summary>
    private void SelectTitle(MessageTitle title)
    {
        if (curSelect != title)
        {
            curSelect?.SetShow(false);
            curSelect = title;
            curSelect.SetShow(true);
        }
    }

    /// <summary>
    /// 根据类型找到对应的标签
    /// </summary>
    private MessageTitle GetTitleByType(MsgTitleType type)
    {
        foreach (MessageTitle title in titles)
        {
            if (title.curType == type)
            {
                return title;
            }
        }
        return null;
    }

    /// <summary>
    /// 点击标签
    /// </summary>
    private void OnClickTitle(IEventParam param)
    {     
        if (param is ClickMsgTitleEvParam titleParam)
        {
            SelectTitle(titleParam.type);
        }
    }

    private void OnClickMaxInfoBtn(GameObject obj)
    {
        if (isMaxInfo) return;
        isMaxInfo = !isMaxInfo;
        MaxInfoBtn.transform.Find("select").gameObject.SetActive(isMaxInfo);
        MaxInfoBtn.transform.Find("defult").gameObject.SetActive(!isMaxInfo);
        MinInfoBtn.transform.Find("select").gameObject.SetActive(!isMaxInfo);
        MinInfoBtn.transform.Find("defult").gameObject.SetActive(isMaxInfo);
        if (isMaxInfo)
        {
            transform.Find("底部信息框").GetComponent<RectTransform>().DOAnchorPosY(151, 0.5f);
        }
    }

    private void OnClickMinInfoBtn(GameObject obj)
    {
        if (!isMaxInfo) return;
        isMaxInfo = !isMaxInfo;
        MaxInfoBtn.transform.Find("select").gameObject.SetActive(isMaxInfo);
        MaxInfoBtn.transform.Find("defult").gameObject.SetActive(!isMaxInfo);
        MinInfoBtn.transform.Find("select").gameObject.SetActive(!isMaxInfo);
        MinInfoBtn.transform.Find("defult").gameObject.SetActive(isMaxInfo);
        if (!isMaxInfo)
        {
            transform.Find("底部信息框").GetComponent<RectTransform>().DOAnchorPosY(-80, 0.5f);
        }
    }

    private void OnDestroy()
    {
      //  EventDispatcher.GetInstance().RemoveEventListener(EventNameList.CLICK_MSG_TITLE, OnClickTitle);
        EventDispatcher.GetInstance().RemoveEventListener(EventNameList.REF_SHOW_TASK_LOG, RefreshLog);
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.BEIDOU_DATA, OnGetBeiDouData);
    }
}
