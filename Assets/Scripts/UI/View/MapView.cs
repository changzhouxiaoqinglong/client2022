using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView :  ViewBase<MapViewModel>
{
    /// <summary>
    /// 关闭地图按钮
    /// </summary>
    private ButtonBase closeMapBtn;
    


    protected override void Awake()
    {
        base.Awake();
        closeMapBtn = transform.Find("closeMapBtn").GetComponent<ButtonBase>();
        closeMapBtn.RegistClick(OnClickCloseMapBtn);
        NetManager.GetInstance().AddNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.OpenMap, OnGetCloseMap);

    }
    private void OnGetCloseMap(IEventParam param)
    {
        print("OnGetCloseMap");
        UIMgr.GetInstance().CloseView(ViewType.MapView);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        NetManager.GetInstance().RemoveNetMsgEventListener(ServerType.GuideServer, NetProtocolCode.OpenMap, OnGetCloseMap);
    }

    private void OnClickCloseMapBtn(GameObject obj)
    {
        UIMgr.GetInstance().CloseView(ViewType.MapView);
    }
}
