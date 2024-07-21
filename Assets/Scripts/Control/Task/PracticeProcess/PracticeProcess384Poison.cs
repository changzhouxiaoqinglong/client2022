using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PracticeProcess384Poison : PracticeProcessBase
{
    /// <summary>
    /// 记录开始进样时间
    /// </summary>
    private float startJinYangTime = 0;

    /// <summary>
    /// 进样最少时间
    /// </summary>
    private float jinYangMinTime = 5;
    public override void Init(int taskId)
    {
        base.Init(taskId);
        NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.POISON_ALARM_OP_384,OnGetPoisonAlarmMsg);
        NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.POISON_ALARM_WORK_TYPE_384,OnGetPoisonSetWorkModelMsg);
       // NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.RADIOME_OP_384, ONGetPoisonDFHMsg);
    }

    private void OnGetPoisonAlarmMsg(IEventParam param)
    {
        if (IsFinish()) return;

        if(param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            PoisonAlarmOp384Model model = JsonTool.ToObject<PoisonAlarmOp384Model>(tcpReceiveEvParam.netData.Msg);
            //if (model.Type == PoisonAlarmOp384Type.Error)
            //    UIMgr.GetInstance().ShowToast("请打开空气侦检探头");
            switch (model.Type)
            {
                case PoisonAlarmOp384Type.OpenStatus://开关机
                                                     // Logger.LogWarning(model.Operate.ToString());
                    if (model.Operate == OperateDevice.OPEN)
                    {
                        DoProcess(Poison384Id.POISON384_OPEN);
                    }
                    else if (model.Operate == OperateDevice.CLOSE)
                    {
                        DoProcess(Poison384Id.POISON384_CLOSE);
                    }
                    break;
                case PoisonAlarmOp384Type.Intake: //进气帽
                    // Logger.LogWarning(model.Operate.ToString());
                    if (model.Operate == OperateDevice.OPEN)
                    {
                        DoProcess(Poison384Id.POISON384_Intake_OPEN);
                    }
                    else if (model.Operate == OperateDevice.CLOSE)
                    {
                        DoProcess(Poison384Id.POISON384_Intake_CLOSE);
                    }
                    break;
                case PoisonAlarmOp384Type.JinYang:             
                   // DoProcess(model.Operate == OperateDevice.OPEN ? Poison384Id.POISON384_HEAD_OPEN : Poison384Id.POISON384_HEAD_CLOSE);

                     //开始进样
                    if (model.Operate == OperateDevice.OPEN)
                    {
                        //记下进样时间
                        startJinYangTime = Time.realtimeSinceStartup;
                        DoProcess(Poison384Id.POISON384_JINYANG);
                        WaitAlarm();
                    }
                    //else
                    //{
                    //    //结束进样
                    //    int jinIndex = GetProcessIndex(Poison384Id.POISON384_JINYANG);
                    //    //当前步骤在开始进样之后，才判断
                    //    if (curIndex >= jinIndex)
                    //    {
                    //        //计算进样时间
                    //        float time = Time.realtimeSinceStartup - startJinYangTime;
                    //        //时间充足
                    //        if (time > jinYangMinTime)
                    //        {
                    //            //进样结束
                    //            DoProcess(Poison384Id.POISON384_JINYANG_End);
                    //        }
                    //        else
                    //        {
                    //            //提示进样不足
                    //            EventDispatcher.GetInstance().DispatchEvent(EventNameList.PRACTICE_PROCESS_ERROR_TIP, new StringEvParam("进样时间不足！"));
                    //        }
                    //    }
                    //}


                    break;
               
                  
              
               
                //case PoisonAlarmOp384Type.Alarm://报警
                  
                //    if (model.Operate == OperateDevice.OPEN)
                //    {
                //        DoProcess(Poison384Id.POISON384_ALARM);
                //    }
                //    break;


               
				case PoisonAlarmOp384Type.JinYangEnd://进样结束 type改成5
					Debug.LogWarning("JinYangEnd");
					DoProcess(Poison384Id.POISON384_JINYANG_End);
					break;
				default:
                    break;
            }

        }
    }

    async void WaitAlarm()
    {
        await Task.Delay(3000);//进样结束 3秒后通知硬件报警
        DoProcess(Poison384Id.POISON384_ALARM);

        PoisonAlarmOp384Model model = new PoisonAlarmOp384Model
        {
            Type = PoisonAlarmOp384Type.Alarm,
            Operate = 1
        };

        NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model), NetProtocolCode.POISON_ALARM_OP_384, NetManager.GetInstance().CurDeviceForward);
    }

    private void ONGetPoisonDFHMsg(IEventParam param)
    {
        if (IsFinish()) return;
        if(param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            RadiomeOp384Model model = JsonTool.ToObject<RadiomeOp384Model>(tcpReceiveEvParam.netData.Msg);
            if(model.Type == RadiomOpType384.OpenClose)
            {
                if(model.Operate == OperateDevice.OPEN)
                {
                //    DoProcess(Poison384Id.POISON384_DFH_OPEN);
                }
            }
        }
    }


    private void OnGetPoisonSetWorkModelMsg(IEventParam param)
    {
        if (IsFinish()) return;

        if(param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            PoisonAlarmWorkType384Model model = JsonTool.ToObject<PoisonAlarmWorkType384Model>(tcpReceiveEvParam.netData.Msg);
           // if (model.Type == PoisonAlarmWorkType.UPDATE_MODEL)
           //     UIMgr.GetInstance().ShowToast("修改侦检模式，请将DFH控制盒开机");
            DoProcess(Poison384Id.POISON384_SET_WORK_MODEL);
        }
    }

    protected override void JumpToNext()
    {
        base.JumpToNext();
    }

    public override void End()
    {
        base.End();
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.POISON_ALARM_OP_384, OnGetPoisonAlarmMsg);
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.POISON_ALARM_WORK_TYPE_384, OnGetPoisonSetWorkModelMsg);
      //  NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.RADIOME_OP_384, ONGetPoisonDFHMsg);
    }
}
