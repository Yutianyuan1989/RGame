using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : Singleton<TaskManager>
{
    private int m_CurTaskID;
    private int m_CurTaskStep;
    public override void Init()
    {
        base.Init();
        if (RegisterDataManager.HasKey("TaskID"))
        {
            m_CurTaskID = RegisterDataManager.GetInt("TaskID");
        }
        else
        {
            m_CurTaskID = 1;
        }
        if (RegisterDataManager.HasKey("TaskStep"))
        {
            m_CurTaskStep = RegisterDataManager.GetInt("TaskStep");
        }
        else
        {
            m_CurTaskStep = 1;
        }
    }

    public int GetCurStepID()
    {
        var data = TableTaskScriptable.Get().GetData(m_CurTaskID);
        if (data != null)
        {
            int id = data.TaskChain[m_CurTaskStep];
            Debug.Log("ididid" + id);
            return id;
        }
        else
        {
            Debug.LogError("任务ID错误" + m_CurTaskID);
            return 0;
        }
    }


    public void UpdateTask()
    {

    }

}
