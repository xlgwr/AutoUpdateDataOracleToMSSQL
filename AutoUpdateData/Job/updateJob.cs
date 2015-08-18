using AutoUpdateData.Core.dal;
using log4net;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace AutoUpdateData.Service.Job
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class updateJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(Quartz.IJobExecutionContext context)
        {
            logger.DebugFormat("执行更新任务!!!!!!!!!!!!!!!");
            AutoUpdateData.jobflag("Is Runing,Next Time:"+context.NextFireTimeUtc.Value.DateTime);
            //get
            if (AutoUpdateData._tableList.Count > 0)
            {
                logger.Debug("执行数据获取任务!!!!!!!!!!!!!!!");
                var tmpBathc = AutoUpdateData._txt1batchNum;
                AutoUpdateData._dsList.Clear();
                foreach (var item in AutoUpdateData._tableList)
                {
                    if (item.Contains('|'))
                    {
                        var td = item.Split('|');
                        var tmpwhere = td[1] + ">=to_date(:gxsj,'yyyy-MM-dd HH24:mi:ss')";
                        OracleParameter[] parameters = { new OracleParameter(":gxsj", OracleDbType.Varchar2, 10) };
                        parameters[0].Value = DateTime.Now.AddHours(-3).ToString("yyyy-MM-dd HH") + ":00:00";

                        var tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, tmpBathc, parameters);
                        tmpds.DataSetName = td[0].Trim();
                        AutoUpdateData._dsList.Add(tmpds);
                    }
                    else
                    {
                        var tmpds = OracleDal.GetData(item.Trim(), "", tmpBathc);
                        tmpds.DataSetName = item.Trim();
                        AutoUpdateData._dsList.Add(tmpds);
                    }
                }
            }
            else
            {
                logger.Error("no Table,Please check Set.ini,and add Table.");
            }

            var strSQLinsert = new List<String>();
            if (AutoUpdateData._dsList.Count > 0)
            {
                foreach (var item in AutoUpdateData._dsList)
                {
                    var tmpcolDS = OracleDal.GetTableColumns(item.DataSetName);

                    foreach (DataRow row in item.Tables[0].Rows)
                    {
                        var tmpinstall = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, row);
                        if (!string.IsNullOrEmpty(tmpinstall))
                        {
                            strSQLinsert.Add(tmpinstall);
                        }
                    }
                }
            }
            var dd = DbHelperSQL.ExecuteSqlTran(strSQLinsert);
            if (dd > 0)
            {
                logger.DebugFormat("Update Count:{0}", dd);
            }
        }
    }
}