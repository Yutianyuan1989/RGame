/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using ClientDataBase;
//using MobaGame.FixedMath;

[System.Serializable]
public sealed partial class TableTaskTaskDialog : TableClassBase
{
	/// <summary>
    /// KN
    /// </summary>
    public int ID { get { return _ID; } set { _ID = value; } }
	[SerializeField] 
	private int _ID;

	/// <summary>
    /// N
    /// </summary>
    public int NPC { get { return _NPC; } set { _NPC = value; } }
	[SerializeField] 
	private int _NPC;

	/// <summary>
    /// AN
    /// </summary>
    public int[] Dialog { get { return _Dialog; } set { _Dialog = value; } }
	[SerializeField] 
	private int[] _Dialog;


}