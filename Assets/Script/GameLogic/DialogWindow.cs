using Engine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MoleMole;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour {

    public GameObject TriggerObj;
    public TypewriterEffect TypewriterText;
    public GameObject Node_Dialog;
    public Text Text_Dialog;
    public Image Image_LeftSide;
    public Image Image_RightSide;
    public Sprite[] Sprte_speaker;
    public float DialogDuration = 0.2f;

    private int npcID;
    private int startDialog;
    private int endDialog;
    private int curDialog;
    private bool playingDialog = false;

    void OnEnable()
    {
        EventEngine.Instance().AddEventListener((int)GameEventID.ON_NPC_INTERACTIVE, OnTriggerNPC);
        EventEngine.Instance().AddEventListener((int)GameEventID.ON_NPC_INTERDEACTIVE, OnTriggerExitNPC);
    }

    void OnDisable()
    {
        EventEngine.Instance().RemoveAllEventListener((int)GameEventID.ON_NPC_INTERACTIVE);
        EventEngine.Instance().RemoveAllEventListener((int)GameEventID.ON_NPC_INTERDEACTIVE);
    }

    private void OnTriggerExitNPC(int nEventID, object param)
    {
        TriggerObj.SetActive(false);
    }

    private void OnTriggerNPC(int nEventID, object param)
    {
        npcID = (int)param;
        TriggerObj.SetActive(true);
        //检查NPC身上有没有对应的任务 有的话显示任务
        int cStepId = TaskManager.instance.GetCurStepID();
        int en = cStepId / 10000;
        if (en == (int)ETaskSubFuncType.TYPE_TALK)
        {
            var data = TableTaskTaskDialogScriptable.Get().GetData(cStepId);
            //是不是这个NPC
            if (data != null)
            {
                if (data.NPC == npcID)
                {

                }
            }
        }
    }

    //触发对话
    public void TriggerNPCDialog()
    {
        var data = TableNPCScriptable.Get().GetData(npcID);
        if (data != null)
        {
            if (data.Dialog.Length == 1)
            {
                startDialog = endDialog = data.Dialog[0];
            }
            else
            {
                startDialog = data.Dialog[0];
                curDialog = startDialog;
                endDialog = data.Dialog[1];
            }
            Node_Dialog.gameObject.SetActive(true);
            SetDialogInfo(curDialog);
            TypewriterText.myEvent.AddListener(new UnityAction(this.OnDialogFinished));
        }       
    }

    private void SetDialogInfo(int dialogId)
    {
        var data = TableNPCNPCDialogScriptable.Get().GetData(dialogId);
        if (data != null)
        {
            Text_Dialog.text = data.content;
            TypewriterText.RestartRead();
            if (data.Speaker != "我")
            {
                Image_LeftSide.gameObject.SetActive(false);
                Image_RightSide.gameObject.SetActive(true);
                Image_RightSide.sprite = Sprte_speaker[1];
            }
            else
            {
                Image_LeftSide.gameObject.SetActive(true);
                Image_RightSide.gameObject.SetActive(false);
                Image_LeftSide.sprite = Sprte_speaker[0];
            }
        }
    }

    private void OnDialogFinished()
    {
        Invoke("PlayNextDialog", DialogDuration);
        
    }

    void PlayNextDialog()
    {
        if (curDialog < endDialog)
        {
            curDialog++;
            SetDialogInfo(curDialog);
        }
        else
        {
            Node_Dialog.gameObject.SetActive(false);
        }
    }

}
