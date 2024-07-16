using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeProcess384ZhongDuan : PracticeProcessBase
{
    public override void Init(int taskId)
    {
        base.Init(taskId);
        NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.IT_OP_384, OnGetZhongDuanOpMsg);


    }



    private void OnGetZhongDuanOpMsg(IEventParam param)
    {
        //已完成所有步骤
        if (IsFinish()) return;

        if (param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            InformationTerminalOpModel384 model = JsonTool.ToObject<InformationTerminalOpModel384>(tcpReceiveEvParam.netData.Msg);
            switch (model.Type)
            {
                case InformationTerminalOpType384.OpenClose:
                    DoProcess(model.Operate == OperateDevice.OPEN ? InformationTerminalId384.IT_OPEN_384 : InformationTerminalId384.IT_CLOSE_384);
                    break;

            }
        }
    }

    public override void End()
    {
        base.End();
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.IT_OP_384, OnGetZhongDuanOpMsg);

    }
}
