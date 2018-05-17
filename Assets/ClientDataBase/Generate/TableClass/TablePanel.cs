/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using ClientDataBase;
//using MobaGame.FixedMath;

[System.Serializable]
public sealed partial class TablePanel : TableClassBase
{
	/// <summary>
    /// KS
    /// </summary>
    public string Name { get { return _Name; } set { _Name = value; } }
	[SerializeField] 
	private string _Name;

	/// <summary>
    /// B
    /// </summary>
    public bool IsSingleton { get { return _IsSingleton; } set { _IsSingleton = value; } }
	[SerializeField] 
	private bool _IsSingleton;

	/// <summary>
    /// N
    /// </summary>
    public int Group { get { return _Group; } set { _Group = value; } }
	[SerializeField] 
	private int _Group;

	/// <summary>
    /// B
    /// </summary>
    public bool UsePreload { get { return _UsePreload; } set { _UsePreload = value; } }
	[SerializeField] 
	private bool _UsePreload;

	/// <summary>
    /// B
    /// </summary>
    public bool PushStack { get { return _PushStack; } set { _PushStack = value; } }
	[SerializeField] 
	private bool _PushStack;

	/// <summary>
    /// B
    /// </summary>
    public bool HideModel { get { return _HideModel; } set { _HideModel = value; } }
	[SerializeField] 
	private bool _HideModel;


}