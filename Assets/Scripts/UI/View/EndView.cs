
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 结束训练界面
/// </summary>
public class EndView : ViewBase<EndViewModel>
{
    private ButtonBase okBtn;

    protected override void Awake()
    {
        base.Awake();
        okBtn = transform.Find("Content/okBtn").GetComponent<ButtonBase>();
        okBtn.RegistClick(OnClickOk);
    }

    private void OnClickOk(GameObject obj)
    {
        if (Record.GetInstance().isUpLoading)
        {
            UIMgr.GetInstance().ShowToast("正在上传回放视频，请稍后!");
            return;
        }
        UIMgr.GetInstance().CloseAllViews();
        ExSceneData endScene = SceneExDataMgr.GetInstance().GetDataById(SceneConstant.ID_LOGIN_SCENE);
        SceneManager.LoadScene(endScene.SceneName);
        

        //return;


        //UIMgr.GetInstance().CloseAllViews();
        //ExSceneData endScene = SceneExDataMgr.GetInstance().GetDataById(SceneConstant.END_SCENE);
        //SceneManager.LoadScene(endScene.SceneName);
    }
}
