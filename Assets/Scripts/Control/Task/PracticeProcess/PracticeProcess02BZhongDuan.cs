using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeProcess02BZhongDuan : PracticeProcessBase
{
    public override void Init(int taskId)
    {
        base.Init(taskId);
        NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.IT_OP, OnGetZhongDuanOpMsg);


    }



    private void OnGetZhongDuanOpMsg(IEventParam param)
    {
        //已完成所有步骤
        if (IsFinish()) return;

        if (param is TcpReceiveEvParam tcpReceiveEvParam)
        {
            InformationTerminalOpModel model = JsonTool.ToObject<InformationTerminalOpModel>(tcpReceiveEvParam.netData.Msg);
            switch (model.Type)
            {
                case InformationTerminalOpType.OpenClose:
                    DoProcess(model.Operate == OperateDevice.OPEN ? InformationTerminalId.IT_OPEN_02B : InformationTerminalId.IT_CLOSE_02B);
                    break;

            }
        }
    }

    public override void End()
    {
        base.End();
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.IT_OP, OnGetZhongDuanOpMsg);

    }
}
