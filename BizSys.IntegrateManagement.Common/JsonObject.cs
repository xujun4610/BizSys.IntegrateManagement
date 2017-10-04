using BizSys.IntegrateManagement.Entity.CallBack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Common
{
    public class JsonObject
    {
        #region 成功单据的回写

        /// <summary>
        /// 获取处理成功单据（主键为DocEntry）的回写json字符串
        /// </summary>
        /// <param name="ObjectCode">单据</param>
        /// <param name="DocEntry">主键值</param>
        /// <param name="B1DocEntry">B1单据号</param>
        /// <returns></returns>
        public static string GetCallBackJsonString(string ObjectCode, string DocEntry, string B1DocEntry, DateTime dateTime)
        {
            CallBackRootObject callBackRootObject = new CallBackRootObject()
            {
                ObjectId = ObjectCode,
                QueryParameters = new List<QueryParameters>(){
                                    new QueryParameters(){
                                Key="DocEntry",
                                Text=DocEntry
                                    }
                },
                Data = new List<Data>(){
                    new Data(){
                        Key="U_SBOSynchronization",
                        Text="Y"
                    },
                    new Data(){
                         Key="U_SBOCallbackDate",
                        Text=dateTime.ToString()
                    },
                     new Data(){
                         Key="U_SBOCallbackTime",
                         Text=Convert.ToString(dateTime.Hour * 100 + dateTime.Minute)
                    },
                     new Data(){
                         Key="U_SBOId",
                        Text=B1DocEntry
                    }

                }
            };
            return JsonConvert.SerializeObject(callBackRootObject);
            /*
             * {"ObjectId":"ObjectId",
                "QueryParameters":[
                        {"Key":"Key0","Text":"Text0"},
             *          {"Key":"Key1","Text":"Text1"}],
             *   "Data":[
             *          {"Key":"U_SBOSynchronization","Text":"XXXX"},
             *          {"Key":"U_SBOCallbackDate","Text":"YYYY-MM-DD"},
             *          {"Key":"U_SBOId","Text":"XXXX"}]}
            */



        }

        /// <summary>
        /// 获取处理成功单据（主键为DocEntry）的回写json字符串
        /// </summary>
        /// <param name="ObjectCode">单据</param>
        /// <param name="DocEntry">主键值</param>
        /// <param name="B1DocEntry">B1单据号</param>
        /// <returns></returns>
        public static string GetCallBackJsonString(int ObjectKey, string ObjectCode, string DocEntry, string B1DocEntry, DateTime dateTime)
        {
            CallBackRootObject callBackRootObject = new CallBackRootObject()
            {
                ObjectId = ObjectCode,
                QueryParameters = new List<QueryParameters>(){
                                    new QueryParameters(){
                                Key="DocEntry",
                                Text=DocEntry
                                    }
                },
                Data = new List<Data>(){
                    new Data(){
                        Key="U_SBOSynchronization",
                        Text="Y"
                    },
                    new Data(){
                         Key="U_SBOCallbackDate",
                        Text=dateTime.ToString()
                    },
                    new Data(){
                         Key="U_SBOCallbackTime",
                         Text=Convert.ToString(dateTime.Hour * 100 + dateTime.Minute)
                    },
                     new Data(){
                         Key="U_SBOId",
                        Text=ObjectKey + "_" + B1DocEntry
                     }

                }
            };
            return JsonConvert.SerializeObject(callBackRootObject);
            /*
             * {"ObjectId":"ObjectId",
                "QueryParameters":[
                        {"Key":"Key0","Text":"Text0"},
             *          {"Key":"Key1","Text":"Text1"}],
             *   "Data":[
             *          {"Key":"U_SBOSynchronization","Text":"XXXX"},
             *          {"Key":"U_SBOCallbackDate","Text":"YYYY-MM-DD"},
             *          {"Key":"U_SBOId","Text":"XXXX"}]}
            */



        }

        /// <summary>
        /// 获取处理成功单据的回写json字符串
        /// </summary>
        /// <param name="ObjectCode">单据</param>
        /// <param name="keyWord">关键字段名称</param>
        /// <param name="Value">关键字段值</param>
        /// <param name="B1DocEntry">B1单据号</param>
        /// <returns></returns>
        public static string GetCallBackJsonString(string ObjectCode, string KeyWord, string Value, string B1DocEntry, DateTime dateTime)
        {
            CallBackRootObject callBackRootObject = new CallBackRootObject()
            {
                ObjectId = ObjectCode,
                QueryParameters = new List<QueryParameters>(){
                                    new QueryParameters(){
                                Key=KeyWord,
                                Text=Value
                                    }
                },
                Data = new List<Data>(){
                    new Data(){
                        Key="U_SBOSynchronization",
                        Text="Y"
                    },
                    new Data(){
                        Key="U_SBOCallbackDate",
                        Text=dateTime.ToString()
                    },
                    new Data(){
                         Key="U_SBOCallbackTime",
                         Text=Convert.ToString(dateTime.Hour * 100 + dateTime.Minute)
                    },
                     new Data(){
                         Key="U_SBOId",
                        Text=B1DocEntry
                    }
                }
            };
            return JsonConvert.SerializeObject(callBackRootObject);
        }

        /// <summary>
        /// 获取处理成功单据的回写json字符串
        /// </summary>
        /// <param name="ObjectCode">单据</param>
        /// <param name="keyWord">关键字段名称</param>
        /// <param name="Value">关键字段值</param>
        /// <param name="B1DocEntry">B1单据号</param>
        /// <returns></returns>
        public static string GetCallBackJsonString(string ObjectCode, string KeyWord, string Value, string B1DocEntry, string IsSync, DateTime dateTime)
        {
            CallBackRootObject callBackRootObject = new CallBackRootObject()
            {
                ObjectId = ObjectCode,
                QueryParameters = new List<QueryParameters>(){
                                    new QueryParameters(){
                                Key=KeyWord,
                                Text=Value
                                    }
                },
                Data = new List<Data>(){
                    new Data(){
                        Key="U_SBOSynchronization",
                        Text=IsSync
                    },
                    new Data(){
                        Key="U_SBOCallbackDate",
                        Text=dateTime.ToString()
                    },
                    new Data(){
                         Key="U_SBOCallbackTime",
                         Text=Convert.ToString(dateTime.Hour * 100 + dateTime.Minute)
                    },
                     new Data(){
                         Key="U_SBOId",
                        Text=B1DocEntry
                    }
                }
            };
            return JsonConvert.SerializeObject(callBackRootObject);
        }
        /// <summary>
        /// 获取处理成功单据的回写json字符串
        /// </summary>
        /// <param name="ObjectCode">单据</param>
        /// <param name="Conditions">条件集合</param>
        /// <param name="ModifyList">修改字段集合</param>
        /// <param name="B1DocEntry">B1单据号</param>
        /// <returns></returns>
        public static string GetCallBackJsonString(string ObjectCode, Dictionary<string, string> Conditions, Dictionary<string, string> ModifyList, string B1DocEntry, DateTime dateTime)
        {
            CallBackRootObject callBackRootObject = new CallBackRootObject();
            callBackRootObject.QueryParameters = new List<QueryParameters>();
            callBackRootObject.ObjectId = ObjectCode;
            callBackRootObject.Data = new List<Data>(){
                    new Data(){
                        Key="U_SBOSynchronization",
                        Text="Y"
                    },
                    new Data(){
                        Key="U_SBOCallbackDate",
                        Text=dateTime.ToString()
                    },
                    new Data(){
                         Key="U_SBOCallbackTime",
                         Text=Convert.ToString(dateTime.Hour * 100 + dateTime.Minute)
                    },
                    new Data(){
                         Key="U_SBOId",
                        Text=B1DocEntry
                    }
            };
            foreach (var item in Conditions)
            {
                QueryParameters QueryParameter = new QueryParameters();
                QueryParameter.Key = item.Key;
                QueryParameter.Text = item.Value;
                callBackRootObject.QueryParameters.Add(QueryParameter);
            }
            foreach (var item in ModifyList)
            {
                Data data = new Data();
                data.Key = item.Key;
                data.Text = item.Value;
                callBackRootObject.Data.Add(data);
            }
            return JsonConvert.SerializeObject(callBackRootObject);
        }

        #endregion
        #region 失败单据的回写
        /// <summary>
        /// 获取处理失败单据的回写json字符串
        /// </summary>
        /// <param name="ObjectCode">单据</param>
        /// <param name="keyWord">关键字段名称</param>
        /// <param name="Value">关键字段值</param>
        /// <param name="B1DocEntry">B1单据号</param>
        /// <returns></returns>
        public static string GetErrorCallBackJsonString(string ObjectCode, string KeyWord, string Value, string ErrorMsg, DateTime dateTime)
        {
            CallBackRootObject callBackRootObject = new CallBackRootObject()
            {
                ObjectId = ObjectCode,
                QueryParameters = new List<QueryParameters>(){
                                    new QueryParameters(){
                                Key=KeyWord,
                                Text=Value
                                    }
                },
                Data = new List<Data>(){
                    new Data(){
                        Key="U_SBOSynchronization",
                        Text="N"
                    },
                    new Data(){
                        Key="U_SBOCallbackDate",
                        Text=dateTime.ToString()
                    },
                     new Data(){
                         Key="U_SBOCallbackTime",
                         Text=Convert.ToString(dateTime.Hour * 100 + dateTime.Minute)
                    },
                    // new Data(){
                    //     Key="U_SBOId",
                    //    Text=B1DocEntry
                    //}
                }
            };
            return JsonConvert.SerializeObject(callBackRootObject);
        }

        #endregion
    }
}
