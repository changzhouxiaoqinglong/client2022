using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeProcess102ZhongDuan : PracticeProcessBase
{
    public override void Init(int taskId)
    {
        base.Init(taskId);
        NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.IT_OP_102, OnGetZhongDuanOpMsg);


    }



    private void OnGetZhongDuanOpMsg(IEventParam param)
    {
        //已完成所有步骤
        if (IsFinish()) return;

        if (param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            InformationTerminalOpModel102 model = JsonTool.ToObject<InformationTerminalOpModel102>(tcpReceiveEvParam.netData.Msg);
            switch (model.Type)
            {
                case InformationTerminalOpType102.OpenClose:
                    DoProcess(model.Operate == OperateDevice.OPEN ? InformationTerminalId102.IT_OPEN_102 : InformationTerminalId102.IT_CLOSE_102);
                    break;

            }
        }
    }

    public override void End()
    {
        base.End();
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.IT_OP_102, OnGetZhongDuanOpMsg);

    }
}
