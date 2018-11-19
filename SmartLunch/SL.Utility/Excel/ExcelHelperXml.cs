using System;
using System.Collections;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SL.Utility
{
    /// <summary>
    /// 配置文件类
    /// Create By setyg
    /// 2015年1月29日13:24:49
    /// </summary>
   public class ExcelHelperXml
   {
       public static string ConfigFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Config\csvimp.xml";

        public ExcelHelperXml()
        {
            
        }
        public ExcelHelperXml(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }
        public IList ReadXml(string itemName, string xmlPath, ref string noEmpty)
        {
            IList list = new ArrayList();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNodeList xmlNodeList = xmlDoc.SelectNodes("descendant::Items/Item[@Name='" + itemName + "']");
            if (xmlNodeList != null && xmlNodeList.Count > 0)
            {
                XmlNode rootNode = xmlNodeList[0];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Element)
                        continue;
                    ExcelHelperItem item = new ExcelHelperItem();
                    item.FieldName = GetPropertyValue(node, "Name");
                    item.Default_Value = GetPropertyValue(node, "default_value");
                    if (GetPropertyValue(node, "seq").Trim() != "")
                    {
                        item.Seq = Convert.ToInt32(GetPropertyValue(node, "seq"));
                    }
                    item.Type = GetPropertyValue(node, "type");
                    item.Length = GetPropertyValue(node, "length");
                    item.Memo = GetPropertyValue(node, "memo");
                    if (GetPropertyValue(node, "empty").ToUpper() == "TRUE")
                    {
                        item.Empty = true;
                    }
                    else
                    {
                        item.Empty = false;
                    }
                    list.Add(item);
                }
                if (rootNode.Attributes["NoEmptyCol"] != null)
                {
                    noEmpty = rootNode.Attributes["NoEmptyCol"].Value;
                }
            }

            return list;
        }


        /// <summary>
        /// 获取XmlNode属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="proName"></param>
        /// <returns></returns>
        protected string GetPropertyValue(XmlNode obj, string proName)
        {
            if (obj.Attributes[proName] != null)
            {
                return obj.Attributes[proName].Value;
            }
            foreach (XmlNode node1 in obj.ChildNodes)
            {
                if (node1.Name == proName)
                {
                    return node1.InnerText;
                }
            }
            return "";
        }


        /// <summary>
        /// 程序运行目录
        /// </summary>
        ///  /// <summary>
        /// 得到XML节点的值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public string GetXmlValue(string nodeName)
        {
//            XmlDocument xml = new XmlDocument();
//            string strReturn = "";
//            xml.Load(ConfigFilePath);
//            XmlElement root = xml.DocumentElement;
//            XmlNode node = null;
//            for (int i = 0; i < root.ChildNodes.Count; i++)
//            {
//                node = root.ChildNodes[i];
//                if (node.NodeType != XmlNodeType.Comment && node.Name.Equals(nodeName))
//                {
//                    strReturn = node.InnerText;
//                    break;
//                }
//            }
//            return strReturn;

            string strReturn = "";
            XElement xElement=XElement.Load(ConfigFilePath);
            var text = (from t in xElement.Elements("Item")                    
                       .Where(w => w.Attribute("Name").Value == nodeName)  
                       select new
                       {
                           HasHead = t.Attribute("HasHead").Value   
                       }).ToList();
           
            strReturn = text[0].HasHead;
            return strReturn;
        }

        public int GetXmlErrorInfoColValue(string nodeName)
        {
            XElement xElement=XElement.Load(ConfigFilePath);
            var text = (from t in xElement.Elements("Item")
                       .Where(w => w.Attribute("Name").Value == nodeName)
                        select new
                        {
                            ErrorInfoCol = t.Attribute("ErrorInfoCol").Value
                        }).ToList();

            int retValue = 0;
            if (int.TryParse(text[0].ErrorInfoCol, out retValue))
            {
                return retValue;
            }
            else
            {
                return 0;
            }
        }
   }
}
