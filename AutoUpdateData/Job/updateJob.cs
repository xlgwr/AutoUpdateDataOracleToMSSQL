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
            if (AutoUpdateData._isUploading)
            {
                logger.DebugFormat("***************************上个job还在上传中。。。 {0}", context.PreviousFireTimeUtc.Value.DateTime);
                return;
            }
            AutoUpdateData._isUploading = true;

            logger.DebugFormat("执行更新任务!!!!!!!!!!!!!!!");
            AutoUpdateData.jobflag("Is Runing,Next Time:" + context.NextFireTimeUtc.Value.DateTime);
            try
            {
                //get
                if (AutoUpdateData._tableList.Count > 0)
                {
                    logger.Debug("执行数据获取任务!!!!!!!!!!!!!!!");
                    var tmpBatch = AutoUpdateData._txt1batchNum;
                    AutoUpdateData._dsList.Clear();
                    foreach (var item in AutoUpdateData._tableList)
                    {
                        if (item.Key.Contains('|'))
                        {
                            var td = item.Key.Split('|');
                            var tmpwhere = "";
                            int preNum = 0;
                            var tmpds = new DataSet();

                            switch (item.Value)
                            {
                                case 1:
                                    break;
                                case 3:
                                    //key: table|where|order by
                                    tmpwhere = td[1] + ">=to_date(:gxsj,'yyyy-MM-dd HH24:mi:ss')";
                                    OracleParameter[] parameters = { new OracleParameter(":gxsj", OracleDbType.Varchar2, 10) };
                                    parameters[0].Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";// HH

                                    var tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                    if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                    {
                                        preNum = 0;
                                    }

                                    var allCount = OracleDal.GetCount(td[0].Trim(), tmpwhere, parameters);
                                    logger.DebugFormat("*********已上传：{0} ,Oracle 现在有数据：{1}.", preNum, allCount);

                                    if (preNum >= allCount)
                                    {
                                        logger.DebugFormat("*********(已上传数) {0} >= {1} (Oracle 现在有数据),无需更新.", preNum, allCount);
                                    }
                                    else
                                    {
                                        var tmptoUpdate = (allCount - preNum);
                                        logger.DebugFormat("*********需更新数.", tmptoUpdate);
                                        tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, td[2], preNum, tmptoUpdate, parameters);
                                        tmpds.DataSetName = td[0].Trim();
                                    }


                                    break;
                                default:
                                    break;
                            }
                            if (tmpds.Tables.Count > 0)
                            {
                                AutoUpdateData._dsList.Add(tmpds);
                            }

                        }
                        else
                        {
                            var tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", item.Key.Trim());
                            int preNum = 0;
                            if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                            {
                                preNum = 0;
                            }

                            var allCount = OracleDal.GetCount(item.Key, "");

                            logger.DebugFormat("*********已上传：{0} ,Oracle 现在有数据：{1}.", preNum, allCount);

                            if (preNum >= allCount)
                            {
                                logger.DebugFormat("*********(已上传数) {0} >= {1} (Oracle 现在有数据),无需更新.", preNum, allCount);
                            }
                            else
                            {

                                var tmptoUpdate = (allCount - preNum);
                                logger.DebugFormat("*********需更新数:{0}", tmptoUpdate);

                                var tmpds = OracleDal.GetData(item.Key, "", "", preNum, tmptoUpdate);
                                tmpds.DataSetName = item.Key;
                                AutoUpdateData._dsList.Add(tmpds);
                            }
                        }
                    }
                }
                else
                {
                    logger.Error("no Table,Please check Set.ini,and add Table.");
                }


                if (AutoUpdateData._dsList.Count > 0)
                {
                    foreach (var item in AutoUpdateData._dsList)
                    {


                        //get colmuns
                        var tmpcolDS = OracleDal.GetTableColumns(item.DataSetName);

                        //get Allcount
                        var allCount = item.Tables[0].Rows.Count;

                        //gen sql

                        if (allCount <= AutoUpdateData._txt1batchNum)
                        {
                            //init str
                            var strSQLinsert = new List<String>();

                            foreach (DataRow row in item.Tables[0].Rows)
                            {
                                var tmpinstall = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, row);
                                if (!string.IsNullOrEmpty(tmpinstall))
                                {
                                    strSQLinsert.Add(tmpinstall);
                                }
                            }
                            //upload to mssql 
                            updateToMSSQL(item, strSQLinsert);

                        }
                        else
                        {
                            var nextdiffCount = 0;

                            var tmpCount = 1;
                            do
                            {
                                //init str
                                var strSQLinsert = new List<String>();
                                for (int i = nextdiffCount; i < allCount; i++)
                                {
                                    var tmpinstall = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, item.Tables[0].Rows[i]);
                                    if (!string.IsNullOrEmpty(tmpinstall))
                                    {
                                        strSQLinsert.Add(tmpinstall);
                                    }

                                    if (i + 1 % AutoUpdateData._txt1batchNum == 0)
                                    {
                                        nextdiffCount += AutoUpdateData._txt1batchNum;
                                        break;
                                    }
                                }

                                //upload to mssql 
                                updateToMSSQL(item, strSQLinsert);

                                
                                tmpCount++;

                            } while (nextdiffCount < allCount);

                        }



                    }
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                AutoUpdateData._isUploading = false;
            }

        }

        private static void updateToMSSQL(DataSet item, List<string> strSQLinsert)
        {
            var dd = DbHelperSQL.ExecuteSqlTran(strSQLinsert);

            if (dd > 0)
            {
                //save upload count for each;
                var tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", item.DataSetName);
                int getNum = 0;
                if (!int.TryParse(tmpTableTakeDataNum, out getNum))
                {
                    getNum = 0;
                }
                int tmpcountNum = getNum + strSQLinsert.Count;
                AutoUpdateData._iniToday.IniWriteValue("TableTakeDataNum", item.DataSetName, tmpcountNum.ToString());

                logger.DebugFormat("Update Success Count:{0}", dd);
            }
            else
            {
                logger.DebugFormat("Update Fails Count:{0}", dd);
            }
        }
    }
}