using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MapView :  ViewBase<MapViewModel>
{
    /// <summary>
    /// 关闭地图按钮
    /// </summary>
    private ButtonBase closeMapBtn;
    Transform deletemeum;
    ButtonBase cancelBtn;
    ButtonBase confirmBtn;
    


    protected override void Awake()
    {
        base.Awake();
        closeMapBtn = transform.Find("closeMapBtn").GetComponent<ButtonBase>();
        closeMapBtn.RegistClick(OnClickCloseMapBtn);
        deletemeum= transform.Find("Delete");
        confirmBtn = transform.Find("Delete/Panel/true").GetComponent<ButtonBase>();
        cancelBtn = transform.Find("Delete/Panel/false").GetComponent<ButtonBase>();
        confirmBtn.RegistClick(OnClickConfirm);
        cancelBtn.RegistClick(OnClickCancel);
        NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.CloseMap, OnGetCloseMap);

    }
    private void OnGetCloseMap(IEventParam param)
    {
        print("OnGetCloseMap");
        UIMgr.GetInstance().CloseView(ViewType.MapView);
    }

    void OnClickConfirm(GameObject obj)
    {
        if (SceneMgr.GetInstance().curScene is Train3DSceneCtrBase scene3D)
        {

            transform.parent.GetComponentInChildren<MaxMapControl>().Delete();

        }
        deletemeum.gameObject.SetActive(false);
        
    }

    void OnClickCancel(GameObject obj)
	{
        deletemeum.gameObject.SetActive(false);

    }

    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.CloseMap, OnGetCloseMap);
    }

    private void OnClickCloseMapBtn(GameObject obj)
    {
        UIMgr.GetInstance().CloseView(ViewType.MapView);
    }

    public Action<GameObject> Actiondelect;
}
