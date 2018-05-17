﻿using Engine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollCircleJoystick : ScrollRect
{

	 // 半径
     private float _mRadius = 0f;

     // 距离
     private const float Dis = 0.5f;
 
     protected override void Start()
     {
         base.Start();

         // 能移动的半径 = 摇杆的宽 * Dis
        _mRadius = content.sizeDelta.x * Dis;
    }
 
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
             
         // 获取摇杆，根据锚点的位置。
       var contentPosition = content.anchoredPosition;

        // 判断摇杆的位置 是否大于 半径
        if (contentPosition.magnitude > _mRadius)
        {   
             // 设置摇杆最远的位置
            contentPosition = contentPosition.normalized * _mRadius;
            SetContentAnchoredPosition(contentPosition);
        }

        // 最后 v2.x/y 就跟 Input中的 Horizontal Vertical 获取的值一样 
        var v2 = content.anchoredPosition.normalized;
        if (Mathf.Abs(v2.x) <= Mathf.Abs(v2.y))
        {
            v2.x = 0f;
        }
        else 
        {
            v2.y = 0f;
        }
        EventEngine.Instance().DispatchEvent((int)GameEventID.PLAYERMOVE, v2);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnPointerUpOnPointerUp");
        content.localPosition = Vector3.zero;
        EventEngine.Instance().DispatchEvent((int)GameEventID.PLAYERMOVE, Vector2.zero);
    }
}
