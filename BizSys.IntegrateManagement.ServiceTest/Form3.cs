using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesOrder;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizSys.IntegrateManagement.ServiceTest
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void btnSO1_Click(object sender, EventArgs e)
        {
            //销售订单
            var concatedString = string.Concat(this.txtJSONStr.Lines);
            SalesOrderRootObject DesOrderName = JsonConvert.DeserializeObject<SalesOrderRootObject>(concatedString);
            if (DesOrderName.ResultObjects.Count == 0) { MessageBox.Show("没有记录，结束！"); return; }
            foreach (var item in DesOrderName.ResultObjects)
            {
                var documentResult = OmniChannelToSAP.Service.Document.SalesManagement.SalesOrder.CreateSalesOrder(item);
                if (documentResult.ResultValue == ResultType.True)
                {
                    string callBackJsonString = JsonObject.GetCallBackJsonString4MFT_B1Customer(item.ObjectCode, "DocEntry", item.DocEntry.ToString(), documentResult.DocEntry, System.DateTime.Now, documentResult.CallBackDataList);
                    //var ts = new Task<string>(() => { return BaseHttpClient.HttpCallBackAsync(callBackJsonString).Result; } );
                    //ts.Start();
                    var t = BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                    t.Start();
                    string callBackResultStr = t.Result;  //ts.Result;
                    Logger.Writer(string.Format("回写数据：{0}", callBackResultStr));

                    var callBackResult = JsonConvert.DeserializeObject<CallBackResult>(callBackResultStr);
                }
            }

        }
    }
}
