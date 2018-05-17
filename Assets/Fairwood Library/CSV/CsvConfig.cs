
using System;
using System.Collections.Generic;
namespace Edu.CSV
{
    /// <summary>
    /// 一条配置文件
    /// </summary>
    public class CsvConfig : IEnumerable<CsvConfigField>
    {
        private readonly Dictionary<string,CsvConfigField> _namedFields = new Dictionary<string, CsvConfigField>();

        private readonly Dictionary<int,CsvConfigField> _indexedFields = new Dictionary<int, CsvConfigField>(); 

        /// <summary>
        /// maybe return null
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CsvConfigField this[int index]
        {
            get
            {
                if (_indexedFields.ContainsKey(index))
                {
                    return _indexedFields[index];
                }

                return null;
            }           
        }
        


        /// <summary>
        /// maybe return null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CsvConfigField this[string name]
        {
            get
            {
                if (_namedFields.ContainsKey(name))
                {
                    return _namedFields[name];
                }

                return null;
            }

        }

        /// <summary>
        /// Return whether is filed is exist or not
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool IsFieldExist(string fieldName)
        {
            return _namedFields.ContainsKey(fieldName);
        }

        public CsvConfig()
        {
            _namedFields.Clear();
            _indexedFields.Clear();
        }

        public bool AddField(CsvConfigField field)
        {
            if (_namedFields.ContainsKey(field.Name))
            {
                return false;
            }
            _namedFields.Add(field.Name,field);
            _indexedFields.Add(field.Sequence,field);
            return true;
        }

        public IEnumerator<CsvConfigField> GetEnumerator()
        {
            return _namedFields.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _namedFields.Values.GetEnumerator();
        }

        public CsvConfig Clone()
        {
            CsvConfig tempConfig = new CsvConfig();
            foreach (CsvConfigField field in _namedFields.Values)
            {
                tempConfig.AddField(field.Clone());
            }
            return tempConfig;
        }

        public void Set(string name, int obj)
        {
            if (_namedFields.ContainsKey(name))
            {
                _namedFields[name].Value.Value = obj;
            }
            else
            {
                CsvValue addValue = new CsvValue(CsvValue.CsvValueType.Int, obj);
                CsvConfigField addField = new CsvConfigField(_namedFields.Count , false , false , name , addValue);
                addField.Value.Value = obj;
                AddField(addField);
            }
        }

        public void Set(string name, string obj)
        {
            if (_namedFields.ContainsKey(name))
            {
                _namedFields[name].Value.Value = obj;
            }
            else
            {
                CsvValue addValue = new CsvValue(CsvValue.CsvValueType.String, obj);
                CsvConfigField addField = new CsvConfigField(_namedFields.Count, false, false, name, addValue);
                addField.Value.Value = obj;
                AddField(addField);
            }
        }

        public void Set(string name, bool obj)
        {
            if (_namedFields.ContainsKey(name))
            {
                _namedFields[name].Value.Value = obj;
            }
            else
            {
                CsvValue addValue = new CsvValue(CsvValue.CsvValueType.Bool, obj);
                CsvConfigField addField = new CsvConfigField(_namedFields.Count, false, false, name, addValue);
                addField.Value.Value = obj;
                AddField(addField);
            }
        }


        public void Set(string name, float obj)
        {
            if (_namedFields.ContainsKey(name))
            {
                _namedFields[name].Value.Value = obj;
            }
            else
            {
                CsvValue addValue = new CsvValue(CsvValue.CsvValueType.Float, obj);
                CsvConfigField addField = new CsvConfigField(_namedFields.Count, false, false, name, addValue);
                addField.Value.Value = obj;
                AddField(addField);
            }
        }

    }
}
