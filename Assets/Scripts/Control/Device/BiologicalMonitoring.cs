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
        if (TaskMgr.GetInstance().curTaskData.CheckType != HarmAreaType.BIOLOGY)
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
                ReportCurBiologyData();
            }
        }
    }


   

    /// <summary>
    /// �ϱ���ǰ������Ϣ
    /// </summary>
    protected virtual void ReportCurBiologyData()
    {
        
        //Ũ��
        float dentity = HarmAreaMgr.GetPosBiologyDentity(car.GetPosition());
        Debug.LogError("��ǰ������Ϣ��" + dentity);

        //�����豸�������

        SetBIOLOGYModel model = new SetBIOLOGYModel()
        {
            Biomass = dentity,
        };
        //�����豸
        NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model), NetProtocolCode.SEND_BIOLOGY_BIOMASS, NetManager.GetInstance().CurDeviceForward);


    }
}
