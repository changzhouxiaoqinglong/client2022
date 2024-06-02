using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiologicalMonitoring : DeviceBase
{
    /// <summary>
    /// 计时器
    /// </summary>
    private float checkTimer = 0;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        //化学训练才起作用
        if (TaskMgr.GetInstance().curTaskData.CheckType != HarmAreaType.DRUG)
        {
            return;
        }
        CountBiologyData();


    }

    /// <summary>
    /// 计算上报生物区域信息
    /// </summary>
    private void CountBiologyData()
    {
        if (car.IsSelfCar())
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= AppConstant.Biology_CHECK_OFFTIME)
            {
                checkTimer = 0;
                ReportCurDrugData();
            }
        }
    }

    /// <summary>
    /// 上报当前化学信息
    /// </summary>
    protected virtual void ReportCurDrugData()
    {
        //print("上报当前化学信息");
        //浓度
        float dentity = HarmAreaMgr.GetPosDrugDentity(car.GetPosition());
        DrugVarData drugVarData = HarmAreaMgr.GetPosDrugData(car.GetPosition());

        //if (drugVarData != null)
        //    print("get毒数据浓度为:  "+ dentity);
        //else
        //    print("没有毒数据");

        //毒数据
        ExPoisonData exPoisonData = null;
        if (drugVarData != null)
        {
            exPoisonData = ExPoisonDataMgr.GetInstance().GetDataById(drugVarData.Type);
        }
        ReportDrugDataModel model = new ReportDrugDataModel()
        {
            Id = drugVarData != null ? drugVarData.Id : 0,
            Type = drugVarData != null ? drugVarData.Type : PoisonType.NO_POISON,
            Dentity = dentity,
            Degree = exPoisonData != null ? exPoisonData.GetdDegreeByDentity(dentity) : DrugDegree.NONE,
            DType = exPoisonData != null ? exPoisonData.DType : DrugDType.NONE,
        };
        //发给设备管理软件
        NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model), NetProtocolCode.SEND_DRUG_DATA, NetManager.GetInstance().SameMachineAllSeats);
    }
}
