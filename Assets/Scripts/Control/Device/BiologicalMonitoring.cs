using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiologicalMonitoring : DeviceBase
{
    /// <summary>
    /// ��ʱ��
    /// </summary>
    private float checkTimer = 0;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        //��ѧѵ����������
        if (TaskMgr.GetInstance().curTaskData.CheckType != HarmAreaType.DRUG)
        {
            return;
        }
        CountBiologyData();


    }

    /// <summary>
    /// �����ϱ�����������Ϣ
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
    /// �ϱ���ǰ��ѧ��Ϣ
    /// </summary>
    protected virtual void ReportCurDrugData()
    {
        //print("�ϱ���ǰ��ѧ��Ϣ");
        //Ũ��
        float dentity = HarmAreaMgr.GetPosDrugDentity(car.GetPosition());
        DrugVarData drugVarData = HarmAreaMgr.GetPosDrugData(car.GetPosition());

        //if (drugVarData != null)
        //    print("get������Ũ��Ϊ:  "+ dentity);
        //else
        //    print("û�ж�����");

        //������
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
        //�����豸�������
        NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model), NetProtocolCode.SEND_DRUG_DATA, NetManager.GetInstance().SameMachineAllSeats);
    }
}
