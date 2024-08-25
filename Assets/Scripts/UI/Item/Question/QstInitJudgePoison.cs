using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QstInitJudgePoison : SelectQuestionBase
{
    /// <summary>
    /// 显色图片
    /// </summary>
    private Image craterImage;

    /// <summary>
    /// 显色图片
    /// </summary>
    private Image bigCraterImage;

    /// <summary>
    /// 显色图片按钮组件
    /// </summary>
    private ButtonBase craterBtn;

    /// <summary>
    /// 大显色图片按钮组件
    /// </summary>
    private ButtonBase bigCraterBtn;

    /// <summary>
    /// 检测类型
    /// </summary>
    private QstPoisonParam param;

    public Transform content;
    protected override void Awake()
    {
        base.Awake();
        craterImage = transform.Find("CraterImage").GetComponent<Image>();
        bigCraterImage = transform.Find("BigCraterImage").GetComponent<Image>();
        craterBtn = transform.Find("CraterImage").GetComponent<ButtonBase>();
        craterBtn.RegistClick(BigCraterImageShow);
        bigCraterBtn = transform.Find("BigCraterImage").GetComponent<ButtonBase>();
        bigCraterBtn.RegistClick(BigCraterImageHide);
        content = transform.Find("Scroll View/Viewport/Content");
       for(int i=0;i<3;i++)
		{
            ButtonBase btn = content.GetChild(i).GetComponent<ButtonBase>();
            btn.RegistClick(BigCraterImageShow);
        }
    }

    protected override void Start()
    {
        base.Start();
        param = JsonTool.ToObject<QstPoisonParam>(qstResult.Param);
        InitImage();
        InitDrugPoisonLog();
    }

    /// <summary>
    /// 大显色图片显示
    /// </summary>
    private void BigCraterImageShow(GameObject obj)
    {
        bigCraterImage.sprite = obj.GetComponent<Image>().sprite;
        bigCraterImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// 大显色图片隐藏
    /// </summary>
    private void BigCraterImageHide(GameObject obj)
    {
        bigCraterImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 初始化图片
    /// </summary>
    private void InitImage()
    {
        print("初始化图片"+qstConfig.TargetId);
        ExPoisonData data = ExPoisonDataMgr.GetInstance().GetDataById(qstConfig.TargetId);
        string path = data.GetPathByCheckType(param.CheckType);
        print(path);
        if(path!=null)
		{
            var texs = Resources.LoadAll<Sprite>(path);
            for (int i = 0; i < 3; i++)
            {
                //   print(texs[i]);
                content.GetChild(i).GetComponent<Image>().sprite = texs[i];
            }
        }
        else//无毒
		{
            //Prefabs/Sprite/PoisonCheckType/ShaLin
            var tex = Resources.Load<Sprite>("Prefabs/Sprite/PoisonCheckType/WDAir");

            transform.Find("Scroll View").gameObject.SetActive(false);
            craterImage.gameObject.SetActive(true);
            craterImage.sprite = tex;
        }
        

        //craterImage.sprite = Resources.Load<Sprite>(path);
        //bigCraterImage.sprite = Resources.Load<Sprite>(path);
    }

    private void InitDrugPoisonLog()
    {
        (SceneMgr.GetInstance().curScene as TrainSceneCtrBase).virtualCar.SetQstDrugPoisonLog(SetQstDrugPoisonLog(param.pos, param.CheckType, qstConfig.TargetId));
    }

    /// <summary>
    /// 设置日志数据
    /// </summary>
    private QstDrugPoisonLog SetQstDrugPoisonLog(CustVect2 pos, int checkType, int poisonType)
    {
        QstDrugPoisonLog tempLog = new QstDrugPoisonLog();
        tempLog.GisPos = pos;
        tempLog.CheckType = checkType;
        tempLog.PoisonType = poisonType;
        tempLog.TubeCount = 0;
        return tempLog;
    }

}
