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
public class DictionaryTableTaskTaskSelectDialog : SerializableDictionary<int, TableTaskTaskSelectDialog> 
{
}

public sealed class TableTaskTaskSelectDialogScriptable : ScriptableDictionaryObject<int, TableTaskTaskSelectDialog>
{
    private static string m_gameTableName = "TaskTaskSelectDialog";
	
	[UnityEngine.SerializeField]
	private DictionaryTableTaskTaskSelectDialog m_serializedTableDict = new DictionaryTableTaskTaskSelectDialog();

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
						string[] exportFieldNames = {"ID","content","Dialog"};
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

				TableTaskTaskSelectDialog table = new TableTaskTaskSelectDialog();
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
						fieldName = "content";                        
                        typename = "string";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 1);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.content = string.IsNullOrEmpty(trimedStr) ? string.Empty : trimedStr;
					}
					{
                        fieldName = "Dialog";                        
                        typename = "int[]";  
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 2);   
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
	
	public static TableTaskTaskSelectDialogScriptable Get()
    {
        TableTaskTaskSelectDialogScriptable t = ClientDataBaseManager.Instance.GetLoadedTable<TableTaskTaskSelectDialogScriptable>();
        if (t == null)
        {
            ScriptableObjectBase scriptable = ClientDataBaseManager.Instance.LoadTable(m_gameTableName);
            ClientDataBaseManager.Instance.Register(typeof(TableTaskTaskSelectDialogScriptable), scriptable);
			return ClientDataBaseManager.Instance.GetLoadedTable<TableTaskTaskSelectDialogScriptable>();
        }
		
		return t;        
    }
	
	public override void Validate()
	{
		ClientDataChecker.Instance.Check(this, m_tableDict.Values, "ID");
	}
	
	public override SerializableDictionary<int, TableTaskTaskSelectDialog> GetAllData()
    {
        return (SerializableDictionary<int, TableTaskTaskSelectDialog>)m_serializedTableDict;
    }
}