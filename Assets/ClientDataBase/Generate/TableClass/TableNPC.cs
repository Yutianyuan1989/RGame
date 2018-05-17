/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using ClientDataBase;
//using MobaGame.FixedMath;

[System.Serializable]
public sealed partial class TableNPC : TableClassBase
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
    /// N
    /// </summary>
    public int MapID { get { return _MapID; } set { _MapID = value; } }
	[SerializeField] 
	private int _MapID;

	/// <summary>
    /// N
    /// </summary>
    public int PosX { get { return _PosX; } set { _PosX = value; } }
	[SerializeField] 
	private int _PosX;

	/// <summary>
    /// N
    /// </summary>
    public int PosY { get { return _PosY; } set { _PosY = value; } }
	[SerializeField] 
	private int _PosY;

	/// <summary>
    /// AN
    /// </summary>
    public int[] Dialog { get { return _Dialog; } set { _Dialog = value; } }
	[SerializeField] 
	private int[] _Dialog;


}