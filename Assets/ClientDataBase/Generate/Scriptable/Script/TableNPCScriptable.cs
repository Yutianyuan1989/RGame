/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ClientDataBase;

using LumenWorks.Framework.IO.Csv;

//using MobaGame.FixedMath;

using UnityEngine;

[Serializable]
public class DictionaryTableNPC : SerializableDictionary<int, TableNPC> 
{
}

public sealed class TableNPCScriptable : ScriptableDictionaryObject<int, TableNPC>
{
    private static string m_gameTableName = "NPC";
	
	[UnityEngine.SerializeField]
	private DictionaryTableNPC m_serializedTableDict = new DictionaryTableNPC();

    public override bool LoadGameTable(string text)
    {
        m_tableDict.Clear();

        StringReader reader = null;
        int index = 0;

        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError(string.Format("#TableCheck# Can't found GameTable txt file in because text is empty"));
            return false;
        }

        reader = new StringReader(text);
		
        List<string> fieldNames = null;
        if (reader != null)
        {
            CsvReader csvReader = new CsvReader(reader, false);
            foreach (string[] splitStr in csvReader)
            {
                if (index <= 1)
				{
				    if (index == 1)
				    {
				        // 注意解决字段有空格的问题
                        fieldNames = splitStr.Select(a => Utility.TypeRelate.CorrectFieldName(a)).ToList();
#if MGAME_DEBUG
						string[] exportFieldNames = {"ID","Name","MapID","PosX","PosY","Dialog"};
						foreach (string t in exportFieldNames)
						{
						    if (!fieldNames.Contains(t))
						    {
						        Debug.LogError(string.Format("#TableCheck# 加载表格失败,字段名[{0}]在表格[{1}]中找不到，请确证csv与脚本一一对应", t, GetTableName()));
						        return false;
						    }
						}
#endif
				    }

					index++;
					continue;
				}

				TableNPC table = new TableNPC();
				string fieldName = string.Empty;
                string trimedStr = string.Empty;
                string typename = string.Empty;

				try
                {
				var fieldIndex = -1;
					{
						fieldName = "ID";                        
                        typename = "int";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 0);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.ID = string.IsNullOrEmpty(trimedStr) ? default(int) : (int)Convert.ChangeType(trimedStr, typeof(int));
					}
					{
						fieldName = "Name";                        
                        typename = "string";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 1);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.Name = string.IsNullOrEmpty(trimedStr) ? string.Empty : trimedStr;
					}
					{
						fieldName = "MapID";                        
                        typename = "int";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 2);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.MapID = string.IsNullOrEmpty(trimedStr) ? default(int) : (int)Convert.ChangeType(trimedStr, typeof(int));
					}
					{
						fieldName = "PosX";                        
                        typename = "int";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 3);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.PosX = string.IsNullOrEmpty(trimedStr) ? default(int) : (int)Convert.ChangeType(trimedStr, typeof(int));
					}
					{
						fieldName = "PosY";                        
                        typename = "int";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 4);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.PosY = string.IsNullOrEmpty(trimedStr) ? default(int) : (int)Convert.ChangeType(trimedStr, typeof(int));
					}
					{
                        fieldName = "Dialog";                        
                        typename = "int[]";  
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 5);   
						string[] splitArray = splitStr[fieldIndex].Split(';', ',');
						int len = splitArray.Length;		

                        table.Dialog = new int[len];
                      
						for (int i = 0; i < len; i++)
						{
                            trimedStr = splitArray[i].Trim();
							table.Dialog[i] = string.IsNullOrEmpty(trimedStr) ? default(int) : (int)Convert.ChangeType(trimedStr, typeof(int));
						}
					}

				
				    if (!m_tableDict.ContainsKey(table.ID))
					{
						m_tableDict.Add(table.ID, table);
					}
					else
					{
						Debug.LogError(string.Format("#TableCheck# key confict in table({0}) line({1}) key value({2}) ", m_gameTableName,  index + 1, table.ID));
					}		
                
				}
				catch (Exception e)
                {
                    string obvious = string.Empty;
                    string[] log = e.StackTrace.Split('\n');
                    for (int i = 0; i < log.Length; i++)
                    {
                        if (log[i].IndexOf(this.GetType().Name) > 0)
                        {
                            obvious = log[i];
                            break;
                        }
                    }
					Debug.LogError(string.Format("#TableCheck# LoadGameTable error in table({2}) line({1}) field({0}) : when {3} convert to type({4})", fieldName, index+1,m_gameTableName, trimedStr, typename));
                    Debug.LogError(string.Format("#TableCheck# key value{3} : {0}  \n<b>{1}</b> \n\nStackTrace:{2}", e.Message, obvious, e.StackTrace, table.ID));
                }
				finally
                {
                    index++;
                }
            }

            reader.Close();
        }
		return true;
	}
	
	public override string GetTableName()
	{
		return m_gameTableName;
	}
	
	public static TableNPCScriptable Get()
    {
        TableNPCScriptable t = ClientDataBaseManager.Instance.GetLoadedTable<TableNPCScriptable>();
        if (t == null)
        {
            ScriptableObjectBase scriptable = ClientDataBaseManager.Instance.LoadTable(m_gameTableName);
            ClientDataBaseManager.Instance.Register(typeof(TableNPCScriptable), scriptable);
			return ClientDataBaseManager.Instance.GetLoadedTable<TableNPCScriptable>();
        }
		
		return t;        
    }
	
	public override void Validate()
	{
		ClientDataChecker.Instance.Check(this, m_tableDict.Values, "ID");
	}
	
	public override SerializableDictionary<int, TableNPC> GetAllData()
    {
        return (SerializableDictionary<int, TableNPC>)m_serializedTableDict;
    }
}