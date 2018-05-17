#if UNITY_EDITOR
using System;
using System.IO;
using System.Xml.Serialization;

/// <summary>
/// Xml序列化与反序列化
/// </summary>
public class XmlUtil
{

    /// ------------------------------ 反序列化  -----------------------------------
    /// <summary>
    /// The deserialize from xml.
    /// </summary>
    public static object DeserializeFromXml(Type type, string xmlPath)
    {
        using (var file = new FileStream(xmlPath, FileMode.Open))
        {
            return Deserialize(type, file);
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="xml">XML字符串</param>
    /// <returns></returns>
    public static object Deserialize(Type type, string xml)
    {
        try
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(type);
                return xmldes.Deserialize(sr);
            }
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="type"> </param>
    /// <param name="stream"> The stream </param>
    /// <returns> </returns>
    public static object Deserialize(Type type, Stream stream)
    {
        XmlSerializer xmldes = new XmlSerializer(type);
        return xmldes.Deserialize(stream);
    }

    /// ------------------------------ 序列化  -----------------------------------
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static string Serializer(Type type, object obj)
    {
        var stream = new MemoryStream();
        XmlSerializer xml = new XmlSerializer(type);

        // 序列化对象
        xml.Serialize(stream, obj);
        stream.Position = 0;
        StreamReader sr = new StreamReader(stream);
        string str = sr.ReadToEnd();

        sr.Dispose();
        stream.Dispose();

        return str;
    }

    public static void SerializerToXml(Type type,  object sourceObj, string filePath, string xmlRootName)
    {
        if (!string.IsNullOrEmpty(filePath) && sourceObj != null)
        {
            type = type != null ? type : sourceObj.GetType();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                XmlSerializer xmlSerializer = string.IsNullOrEmpty(xmlRootName) ?
                                                                           new XmlSerializer(type) :
                                                                           new XmlSerializer(type, new XmlRootAttribute(xmlRootName));
                xmlSerializer.Serialize(writer, sourceObj);
            }
        }
    }
}
#endif