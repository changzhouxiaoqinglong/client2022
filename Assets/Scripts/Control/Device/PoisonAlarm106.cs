using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAlarm106 : PoisonAlarm
{
    protected override void ReportCurDrugData()
    {
        //Debug.LogError("发送106毒剂信息");
        //浓度
        float dentity = HarmAreaMgr.GetPosDrugDentity(car.GetPosition());
        DrugVarData drugVarData = HarmAreaMgr.GetPosDrugData(car.GetPosition());
        //if (drugVarData != null)
        //{
        //    UnityEngine.Debug.LogError("get毒数据浓度为:  " + dentity);
        //}
        //else
        //    UnityEngine.Debug.LogError("没有毒数据");

        DefenseReportDrugDataModel model = new DefenseReportDrugDataModel()
        {
            Type = drugVarData != null ? drugVarData.Type : PoisonType.NO_POISON,
            Dentity = dentity,
        };
        //Debug.LogError("发送106毒剂信息");
        //发给设备管理软件   之前协议是105  现在改为102协议号了
        NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model), NetProtocolCode.SEND_DRUG_DATA, NetManager.GetInstance().CurDeviceForward);
       // NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model), NetProtocolCode.SEND_DRUG_DATA, NetManager.GetInstance().SameMachineAllSeats);
      //  NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model.reportData), NetProtocolCode.SEND_DRUG_DATA, NetManager.GetInstance().SameMachineAllSeats);
    }

  
}
