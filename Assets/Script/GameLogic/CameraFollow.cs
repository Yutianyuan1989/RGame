using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour 
{

    public Transform target;
    private Vector3 offset;

    public Rect mapRect;

    void Start()
    {
        //设置相对偏移
        offset = target.position - this.transform.position;
    }

    void LateUpdate()
    {
        Vector3 v3 = target.position - offset;
        v3.x = Mathf.Clamp(v3.x,mapRect.xMin,mapRect.xMax);
        v3.y = Mathf.Clamp(v3.y, mapRect.yMin, mapRect.yMax);
 
        this.transform.position = v3;
    }
}
