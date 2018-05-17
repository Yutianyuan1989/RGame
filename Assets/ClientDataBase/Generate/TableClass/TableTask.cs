/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using ClientDataBase;
//using MobaGame.FixedMath;

[System.Serializable]
public sealed partial class TableTask : TableClassBase
{
	/// <summary>
    /// KN
    /// </summary>
    public int ID { get { return _ID; } set { _ID = value; } }
	[SerializeField] 
	private int _ID;

	/// <summary>
    /// S
    /// </summary>
    public string Name { get { return _Name; } set { _Name = value; } }
	[SerializeField] 
	private string _Name;

	/// <summary>
    /// AN
    /// </summary>
    public int[] TaskChain { get { return _TaskChain; } set { _TaskChain = value; } }
	[SerializeField] 
	private int[] _TaskChain;


}