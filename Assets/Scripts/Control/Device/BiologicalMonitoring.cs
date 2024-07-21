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
        if (TaskMgr.GetInstance().curTaskData.CheckType != HarmAreaType.BIOLOGY)
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
                ReportCurBiologyData();
            }
        }
    }


   

    /// <summary>
    /// 上报当前生物信息
    /// </summary>
    protected virtual void ReportCurBiologyData()
    {
        
        //浓度
        float dentity = HarmAreaMgr.GetPosBiologyDentity(car.GetPosition());
        Debug.LogError("当前生物信息：" + dentity);

        //发给设备管理软件

        SetBIOLOGYModel model = new SetBIOLOGYModel()
        {
            Biomass = dentity,
        };
        //发给设备
        NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model), NetProtocolCode.SEND_BIOLOGY_BIOMASS, NetManager.GetInstance().CurDeviceForward);


    }
}
