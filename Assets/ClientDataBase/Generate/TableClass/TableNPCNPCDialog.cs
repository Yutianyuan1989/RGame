/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using ClientDataBase;
//using MobaGame.FixedMath;

[System.Serializable]
public sealed partial class TableNPCNPCDialog : TableClassBase
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
    public string Speaker { get { return _Speaker; } set { _Speaker = value; } }
	[SerializeField] 
	private string _Speaker;

	/// <summary>
    /// S
    /// </summary>
    public string Icon { get { return _Icon; } set { _Icon = value; } }
	[SerializeField] 
	private string _Icon;

	/// <summary>
    /// S
    /// </summary>
    public string content { get { return _content; } set { _content = value; } }
	[SerializeField] 
	private string _content;


}