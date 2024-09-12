using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceReportDetect : ViewBase<ChoiceConfirmViewModel>
{
    //选择侦察类型界面
    private Transform choiceTypeView;
    //上传侦察结果界面
    private Transform reportView;

    //选择侦察类型是按钮
    private ButtonBase choiceTypeYesBtn;
    //选择侦察类型否按钮
    private ButtonBase choiceTypeNoBtn;
    //上传侦察报告是按钮
    private ButtonBase reportYesBtn;
    //上传侦察报告否按钮
    private ButtonBase reportNoBtn;
    //上报侦察界面文本框
    private InputField reportInput;

    /// <summary>
    /// 毒剂信息
    /// </summary>
    private ReportDrugDataModel model;

    protected override void Awake()
    {
        base.Awake();
        //选择侦察类型界面
        choiceTypeView = transform.Find("ChoiceType");
        choiceTypeYesBtn = choiceTypeView.Find("Content/yesBtn").GetComponent<ButtonBase>();
        choiceTypeNoBtn = choiceTypeView.Find("Content/noBtn").GetComponent<ButtonBase>();
        choiceTypeYesBtn.RegistClick(OnChoiceTypeYesBtn);
        choiceTypeNoBtn.RegistClick(OnChoiceTypeNoBtn);
        //上报界面
        reportView = transform.Find("Report");
        reportYesBtn = reportView.Find("Content/yesBtn").GetComponent<ButtonBase>();
        reportNoBtn = reportView.Find("Content/noBtn").GetComponent<ButtonBase>();
        reportInput = reportView.Find("Content/InputField").GetComponent<InputField>();
        reportYesBtn.RegistClick(OnReportYesBtn);
        reportNoBtn.RegistClick(OnReportNoBtn);

        //choiceTypeView.gameObject.SetActive(true);
       // reportView.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        this.DelayInvoke(0, () =>
        {
            NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.SEND_DRUG_DATA, OnGetDrugDataMsg);

            
        });
        //this.DelayInvoke(1f, () =>
        //{

        //    reportInput.text = GetReportStr("qianjie");
          
        //});
        
    }

    private void OnGetDrugDataMsg(IEventParam param)
    {
        if(param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            model = JsonTool.ToObject<ReportDrugDataModel>(tcpReceiveEvParam.netData.Msg);
            print("获得Drug数据" + model.ToString());
        }
    }

    /// <summary>
    /// 1 前界 2后界 3临机 4综合
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetReportStr(string typeName) {
        string reportStr = string.Empty;
        //辐射
        if (TaskMgr.GetInstance().curTaskData.CheckType == HarmAreaType.NUCLEAR)
        {
            switch (typeName)
            {
                case ChoiceReportConstant.CHOICE_REPORT_QIANJIE:
                    reportStr = "时间：" + TaskMgr.GetInstance().curTaskCtr.trainDateMgr.GetCurDateStr() + "\r\n" +
                            "前界：经纬度坐标(xxxx)\r\n" +
                            "标志：位置(xxxx)\r\n" +
                            "辐射剂量率：(xx)uGy/h\r\n" +
                            "风向：" + GetWindDir() + "    风速：" + GetWindSpeed() + "\r\n" +
                            "温湿度：" + GetWSD();
                    break;
                case ChoiceReportConstant.CHOICE_REPORT_HOUJIE:
                    reportStr = "时间：" + TaskMgr.GetInstance().curTaskCtr.trainDateMgr.GetCurDateStr() + "\r\n" +
                            "后界：经纬度坐标(xxxx)\r\n" +
                            "标志：位置(xxxx)\r\n" +
                            "辐射剂量率：(xx)uGy/h\r\n" +
                            "风向：" + GetWindDir() + "    风速：" + GetWindSpeed() + "\r\n" +
                            "温湿度：" + GetWSD();
                    break;
                case ChoiceReportConstant.CHOICE_REPORT_ZONGHE:
                    reportStr = "时间：" + TaskMgr.GetInstance().curTaskCtr.trainDateMgr.GetCurDateStr() + "\r\n" +
                            "前后界：经纬度坐标(xxxx)\r\n" +
                            "标志：位置(xxxx)\r\n" +
                            "辐射剂量率：(xx)uGy/h\r\n" +
                            "道路两边情况：弹坑 / 征候数量（xx）、位置（xx）、种类（xx）、浓度（xx）等\r\n" +
                            "风向：" + GetWindDir() + "    风速：" + GetWindSpeed() + "\r\n" +
                            "温湿度：" + GetWSD()+ "\r\n" +
                            "突发及处置情况：文档描述\r\n" +
                            "目前状态：包括人员伤亡情况、防护状态、自消情况、物资消耗情况等。\r\n" +
                            "相关建议及后续任务请示：有关部队防护、通过时的状态等方面的建议，后续工作请示等。";
                    break;
                case ChoiceReportConstant.CHOICE_REPORT_LINJI:
                    reportStr = "报告信息没有固定格式，可用开放式文档，有什么情况报告什么情况。";
                    break;
                default:
                    break;
            }

        }
        //化学
        else if (TaskMgr.GetInstance().curTaskData.CheckType == HarmAreaType.DRUG)
        {
            switch (typeName)
            {
                case ChoiceReportConstant.CHOICE_REPORT_QIANJIE:
                    reportStr = "时间：" + TaskMgr.GetInstance().curTaskCtr.trainDateMgr.GetCurDateStr() + "\r\n" +
                        "前界：经纬度坐标(xxxx)\r\n" +
                        "标志：位置(xxxx)\r\n" +
                        "种类：(xx)毒剂\r\n" +
                        "浓度：" + model.Dentity + "微克每升\r\n" +
                        "风向：" + GetWindDir() + "    风速：" + GetWindSpeed() + "\r\n" +
                        "温湿度：" + GetWSD();
                    break;
                case ChoiceReportConstant.CHOICE_REPORT_HOUJIE:
                    reportStr = "时间：" + TaskMgr.GetInstance().curTaskCtr.trainDateMgr.GetCurDateStr() + "\r\n" +
                        "后界：经纬度坐标(xxxx)\r\n" +
                        "标志：位置(xxxx)\r\n" +
                        "种类：(xx)毒剂\r\n" +
                        "浓度："+ model.Dentity +  "微克每升\r\n" +
                        "道路两边情况：弹坑数量（xx）、位置(xx)、种类（xx）、浓度(xx)等\r\n" +
                        "风向：" + GetWindDir() + "    风速：" + GetWindSpeed() + "\r\n" +
                        "温湿度：" + GetWSD();
                    break;
                case ChoiceReportConstant.CHOICE_REPORT_ZONGHE:
                    reportStr = "时间：" + TaskMgr.GetInstance().curTaskCtr.trainDateMgr.GetCurDateStr() + "\r\n" +
                        "前后界：经纬度坐标(xxxx)\r\n" +
                        "标志：位置(xxxx)\r\n" +
                        "种类：(xx)毒剂\r\n" +
                        "浓度：" + model.Dentity + "微克每升\r\n" +
                        "道路两边情况：弹坑数量（xx）、位置(xx)、种类（xx）、浓度(xx)等\r\n" +
                        "风向：" + GetWindDir() + "    风速：" + GetWindSpeed() + "\r\n" +
                        "温湿度：" + GetWSD() + "\r\n" +
                        "突发及处置情况：文档描述\r\n" +
                        "目前状态：包括人员伤亡情况、防护状态、自消情况、物资消耗情况等。\r\n" +
                        "相关建议及后续任务请示：有关部队防护、通过时的状态等方面的建议，后续工作请示等。";
                    break;
                case ChoiceReportConstant.CHOICE_REPORT_LINJI:
                    reportStr = "报告信息没有固定格式，可用开放式文档，有什么情况报告什么情况。";
                    break;
                default:
                    break;
            }
        }
        return reportStr;
    }

    
    /// <summary>
    /// 获取当前风向
    /// </summary>
    /// <returns></returns>
    private string GetWindDir() {
        string windDir = "";
        Wearth curWearth = NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Wearth;

        //      if(curWearth.WindDir==0|| curWearth.WindDir == 360)
        //{
        //          windDir = "北风";
        //      }
        //      else if (curWearth.WindDir > 0 && curWearth.WindDir < 90)
        //      {
        //          windDir = "东北风";
        //      }
        //      else if (curWearth.WindDir == 90)
        //      {
        //          windDir = "东风";
        //      }
        //      else if (curWearth.WindDir > 90 && curWearth.WindDir < 180)
        //      {
        //          windDir = "东南风";
        //      }
        //      else if (curWearth.WindDir == 180)
        //      {
        //          windDir = "南风";
        //      }
        //      else if (curWearth.WindDir > 180 && curWearth.WindDir < 270)
        //      {
        //          windDir = "西南风";
        //      }
        //      else if (curWearth.WindDir == 270)
        //      {
        //          windDir = "西风";
        //      }
        //      else if (curWearth.WindDir > 270 && curWearth.WindDir < 360)
        //      {
        //          windDir = "西北风";
        //      }

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
        return " 风向："+curWearth.WindDir.ToString()+"°";
        return windDir;
    }

    /// <summary>
    /// 获取风速
    /// </summary>
    /// <returns></returns>
    string GetWindSpeed() {
        string tempStr = "";
        Wearth curWearth = NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Wearth;
        tempStr = curWearth.WindSp.ToString() + "米/秒";
        return tempStr;
    }

    /// <summary>
    /// 获取温度湿度
    /// </summary>
    /// <returns></returns>
    string GetWSD() {
        string tempStr = "";
        Wearth curWearth = NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Wearth;
        tempStr = curWearth.Temperate.ToString() + "摄氏度,"+ curWearth.Humidity.ToString()+ "RH";
        return tempStr;
    }

    //选择侦察类型是按钮
    private void OnChoiceTypeYesBtn(GameObject obj)
    {
        Transform toggleGroup = choiceTypeView.Find("Content/ToggleGroup");
        for(int i = 0; i < toggleGroup.childCount; i++)
        {
            Toggle to = toggleGroup.GetChild(i).GetComponent<Toggle>();
            if (to.isOn == true)
            {
                choiceTypeView.gameObject.SetActive(false);
                reportView.gameObject.SetActive(true);
                reportInput.text = GetReportStr(to.name);
                break;
            }
            else
            {
                continue;
            }
        }
    }
    //选择侦察类型否按钮
    private void OnChoiceTypeNoBtn(GameObject obj)
    {
        UIMgr.GetInstance().CloseView(ViewType.ChoiceReportDetect);
    }
    //上传侦察报告是按钮
    private void OnReportYesBtn(GameObject obj)
    {
        //EventDispatcher.GetInstance().DispatchEvent(EventNameList.REPORT_DETECT_RES);

        //最终结果在前面加上车号
        string resStr = $"车{AppConfig.MACHINE_ID}:\n" + reportInput.text;
        DetectResModel reportModel = new DetectResModel()
        {
            Result = resStr.Trim(),
        };
        //车上所有人 包括自己
        List<ForwardModel> forwardModels = new List<ForwardModel>();
        forwardModels.AddRange(NetManager.GetInstance().SameMachineSeatsExDevice);
        forwardModels.Add(new ForwardModel()
        {
            MachineId = AppConfig.MACHINE_ID,
            SeatId = AppConfig.SEAT_ID,
        });
        NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(reportModel), NetProtocolCode.REPORT_DETECT_RES, forwardModels);
        UIMgr.GetInstance().ShowToast("上报成功");

        UIMgr.GetInstance().CloseView(ViewType.ChoiceReportDetect);
    }
    //上传侦察报告否按钮
    private void OnReportNoBtn(GameObject obj)
    {
        UIMgr.GetInstance().CloseView(ViewType.ChoiceReportDetect);
        
    }

    bool init = false;
	private void Update()
	{
		if(!init)
		{
            if (TaskMgr.GetInstance().curTaskData.CheckType == HarmAreaType.NUCLEAR)
			{
                reportInput.text = GetReportStr("qianjie");
                init = true;
            }
            else if (TaskMgr.GetInstance().curTaskData.CheckType == HarmAreaType.DRUG)
			{
                if (model != null)
                {
                    reportInput.text = GetReportStr("qianjie");
                    init = true;
                }
            }



        }
           
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.SEND_DRUG_DATA, OnGetDrugDataMsg);

    }
}
