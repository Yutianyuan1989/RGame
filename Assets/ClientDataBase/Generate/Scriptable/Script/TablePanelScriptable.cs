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
public class DictionaryTablePanel : SerializableDictionary<string, TablePanel> 
{
}

public sealed class TablePanelScriptable : ScriptableDictionaryObject<string, TablePanel>
{
    private static string m_gameTableName = "Panel";
	
	[UnityEngine.SerializeField]
	private DictionaryTablePanel m_serializedTableDict = new DictionaryTablePanel();

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
						string[] exportFieldNames = {"Name","IsSingleton","Group","UsePreload","PushStack","HideModel"};
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

				TablePanel table = new TablePanel();
				string fieldName = string.Empty;
                string trimedStr = string.Empty;
                string typename = string.Empty;

				try
                {
				var fieldIndex = -1;
					{
						fieldName = "Name";                        
                        typename = "string";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 0);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.Name = string.IsNullOrEmpty(trimedStr) ? string.Empty : trimedStr;
					}
					{
						fieldName = "IsSingleton";                        
                        typename = "bool";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 1);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.IsSingleton = !string.IsNullOrEmpty(trimedStr) && Utility.TypeRelate.StringToBool(trimedStr);
					}
					{
						fieldName = "Group";                        
                        typename = "int";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 2);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.Group = string.IsNullOrEmpty(trimedStr) ? default(int) : (int)Convert.ChangeType(trimedStr, typeof(int));
					}
					{
						fieldName = "UsePreload";                        
                        typename = "bool";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 3);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.UsePreload = !string.IsNullOrEmpty(trimedStr) && Utility.TypeRelate.StringToBool(trimedStr);
					}
					{
						fieldName = "PushStack";                        
                        typename = "bool";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 4);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.PushStack = !string.IsNullOrEmpty(trimedStr) && Utility.TypeRelate.StringToBool(trimedStr);
					}
					{
						fieldName = "HideModel";                        
                        typename = "bool";    
                        fieldIndex = GetFieldIndex(fieldNames, fieldName, 5);    
                        trimedStr = splitStr[fieldIndex].Trim();
						table.HideModel = !string.IsNullOrEmpty(trimedStr) && Utility.TypeRelate.StringToBool(trimedStr);
					}

				
				    if (!m_tableDict.ContainsKey(table.Name))
					{
						m_tableDict.Add(table.Name, table);
					}
					else
					{
						Debug.LogError(string.Format("#TableCheck# key confict in table({0}) line({1}) key value({2}) ", m_gameTableName,  index + 1, table.Name));
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
                    Debug.LogError(string.Format("#TableCheck# key value{3} : {0}  \n<b>{1}</b> \n\nStackTrace:{2}", e.Message, obvious, e.StackTrace, table.Name));
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
	
	public static TablePanelScriptable Get()
    {
        TablePanelScriptable t = ClientDataBaseManager.Instance.GetLoadedTable<TablePanelScriptable>();
        if (t == null)
        {
            ScriptableObjectBase scriptable = ClientDataBaseManager.Instance.LoadTable(m_gameTableName);
            ClientDataBaseManager.Instance.Register(typeof(TablePanelScriptable), scriptable);
			return ClientDataBaseManager.Instance.GetLoadedTable<TablePanelScriptable>();
        }
		
		return t;        
    }
	
	public override void Validate()
	{
		ClientDataChecker.Instance.Check(this, m_tableDict.Values, "Name");
	}
	
	public override SerializableDictionary<string, TablePanel> GetAllData()
    {
        return (SerializableDictionary<string, TablePanel>)m_serializedTableDict;
    }
}