using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;

namespace QICore.Modes
{
    interface IConstrat
    {
        void GetUserInfo();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetUserName(string id);
       
    }
    public class Class1: IConstrat
    {
        public ModelEntity GetModels(string jsonPath)
        {
            ModelEntity model =null;
           
            model = new ModelEntity();
            model.NameSpace = "PR.WINPLANT";
            model.OutPath = "c:\\";
            model.Models = new System.Collections.Generic.List<Model>();
            Column co = new Column();
            co.Name = "UserName";

            var parmList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>("");
            var comment = string.Empty;
            var parmString = string.Empty;
            comment += " /// <summary>";
            comment += " ///";
            comment += " /// <summary>";
            if (parmList != null && parmList.Count > 0)
            {              
                foreach (var parm in parmList)
                {
                    var name = parm.Key.Split(',').Length>1? parm.Key.Split(',')[0]: parm.Key;
                    var dataType = parm.Key.Split(',').Length > 1 ? parm.Key.Split(',')[1] :"string";
                    var desc = parm.Value;
                   
                    comment += $"<param name=\"{name}\">{desc}</param>";
                    parmString += $"{dataType} {name},";
                }
            }
            comment += " ///<returns></returns>";
            parmString = parmString.TrimEnd(',');
            model.Models.Add(new Model() { TableName = "UserInfo", Columns = new System.Collections.Generic.List<Column>() { co } });
            var json=Newtonsoft.Json.JsonConvert.SerializeObject(model);
            //读取json文件  
            using (StreamReader sr = new StreamReader(jsonPath,System.Text.Encoding.Default))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    //构建Json.net的读取流  
                    JsonReader reader = new JsonTextReader(sr);
                    //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                    model = serializer.Deserialize<ModelEntity>(reader);                  
                }
                catch (Exception ex)
                {                   
                    ex.Message.ToString();
                }
            }
            return model;
        }

        public void GetUserInfo()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUserName(string id)
        {
            throw new NotImplementedException();
        }
    }
}
