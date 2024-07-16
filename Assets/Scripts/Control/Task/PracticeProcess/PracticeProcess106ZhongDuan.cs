using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeProcess106ZhongDuan : PracticeProcessBase
{
    public override void Init(int taskId)
    {
        base.Init(taskId);
        NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.IT_OP_106, OnGetZhongDuanOpMsg);


    }



    private void OnGetZhongDuanOpMsg(IEventParam param)
    {
        //已完成所有步骤
        if (IsFinish()) return;

        if (param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            InformationTerminalOpModel106 model = JsonTool.ToObject<InformationTerminalOpModel106>(tcpReceiveEvParam.netData.Msg);
            switch (model.Type)
            {
                case InformationTerminalOpType106.OpenClose:
                    DoProcess(model.Operate == OperateDevice.OPEN ? InformationTerminalId106.IT_OPEN_106 : InformationTerminalId106.IT_CLOSE_106);
                    break;

            }
        }
    }

    public override void End()
    {
        base.End();
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.IT_OP_106, OnGetZhongDuanOpMsg);

    }
}
