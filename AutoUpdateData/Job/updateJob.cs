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
            _is1 = false;

            logger.DebugFormat("执行更新任务!!!!!!!!!!!!!!!");
            AutoUpdateData.jobflag("Is Runing,Next Time:" + context.NextFireTimeUtc.Value.DateTime);
            try
            {
                //init even tInitIniToday
                AutoUpdateData.tInitIniToday(DateTime.Now.ToString("yyyyMMdd"));
                //get
                if (AutoUpdateData._tableList.Count > 0)
                {
                    logger.Debug("执行数据获取任务!!!!!!!!!!!!!!!");
                    var tmpBatch = AutoUpdateData._txt1batchNum;
                    AutoUpdateData._dsList.Clear();
                    foreach (var item in AutoUpdateData._tableList)
                    {
                        try
                        {
                            if (item.Key.Contains('|'))
                            {
                                var td = item.Key.Split('|');
                                var tmpcontarct = AutoUpdateData._CONTRACT;
                                var tmpwhere = " CONTRACT='" + tmpcontarct + "'";

                                var tmpTableTakeDataNum = "";
                                int preNum = 0;
                                var allCount = 0;

                                var tmpKeyname = td[0].Trim() + "_KEY";
                                var tmpKeyLast = td[0].Trim() + "." + td[1].Trim();

                                var tmpds = new DataSet();
                                //father and son
                                var isSon = false;
                                var tmpdsForFatherSon = new DataSet();

                                tmpds.DataSetName = td[0].Trim();

                                switch (item.Value)
                                {
                                    case 1:
                                        //key: 0 table | where 1 | order by 2 from config file
                                        tmpwhere += " and " + td[1].Trim() + "='" + AutoUpdateData._PRIME_COMMODITY + "' ";

                                        //pre update number
                                        tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                        if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                        {
                                            preNum = 0;
                                        }
                                        //get all count form oracle db

                                        allCount = OracleDal.GetCount(td[0].Trim(), tmpwhere);
                                        logger.DebugFormat("*********Table: {0},已上传：{1} ,Oracle 现在有数据：{2}.当日：{3}", td[0], preNum, allCount, DateTime.Now.ToString("yyyyMMdd"));

                                        if (preNum >= allCount)
                                        {
                                            logger.DebugFormat("*********(已上传数) {0} >= {1} (Oracle 现在有数据),无需更新.", preNum, allCount);
                                        }
                                        else
                                        {
                                            var tmptoUpdate = (allCount - preNum);
                                            logger.DebugFormat("*********需更新数:{0}.", tmptoUpdate);
                                            var tmporderby = td[2].Trim();
                                            if (td[2].Trim().ToLower().Equals("no"))
                                            {
                                                tmporderby = "";
                                            }
                                            tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate);

                                        }
                                        tmpds.DataSetName = td[0].Trim();
                                        break;
                                    case 2:
                                        //key: 0 table | add Id 2 | order by 3 from config file
                                        //get the last ID
                                        var tmpTRANSACTION_ID = AutoUpdateData._iniToday.IniReadValue("TableKeyLastValue", tmpKeyLast);
                                        double lastPreId = 0;

                                        if (!double.TryParse(tmpTRANSACTION_ID, out lastPreId))
                                        {
                                            lastPreId = 0;
                                        }

                                        // get last where
                                        tmpwhere += " and " + td[1].Trim() + ">'" + lastPreId + "' ";

                                        //pre update number
                                        tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                        if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                        {
                                            preNum = 0;
                                        }

                                        //get all count form oracle db

                                        allCount = OracleDal.GetCount(td[0].Trim(), tmpwhere);
                                        logger.DebugFormat("*********Table: {0},已上传：{1} ,Oracle 现在有数据：{2}.当日：{3}", td[0], preNum, allCount, DateTime.Now.ToString("yyyyMMdd"));

                                        if (preNum >= allCount)
                                        {
                                            logger.DebugFormat("*********(已上传数) {0} >= {1} (Oracle 现在有数据),无需更新.", preNum, allCount);
                                        }
                                        else
                                        {
                                            var tmptoUpdate = (allCount - preNum);
                                            logger.DebugFormat("*********需更新数:{0}.", tmptoUpdate);
                                            var tmporderby = td[2].Trim();
                                            if (td[2].Trim().ToLower().Equals("no"))
                                            {
                                                tmporderby = "";
                                            }
                                            tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate);

                                        }

                                        tmpds.DataSetName = td[0].Trim();
                                        break;
                                    case 3:
                                        //key: 0 table|1 where|2 order by
                                        //get per last datetime

                                        var tmpLastWhere = AutoUpdateData._iniToday.IniReadValue("TableKeyLastValue", tmpKeyLast);

                                        var tmpLastWhereDateTime = DateTime.Now;
                                        if (!DateTime.TryParse(tmpLastWhere, out tmpLastWhereDateTime))
                                        {
                                            tmpLastWhereDateTime = DateTime.Now;
                                            AutoUpdateData._iniToday.IniWriteValue("TableKeyLastValue", tmpKeyLast, tmpLastWhereDateTime.ToString());
                                        }

                                        tmpwhere += " and " + td[1] + ">=to_date(:gxsj,'yyyy-MM-dd HH24:mi:ss')";
                                        OracleParameter[] parameters3 = { new OracleParameter(":gxsj", OracleDbType.Varchar2, 10) };
                                        //no time
                                        // parameters[0].Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";// HH
                                        parameters3[0].Value = tmpLastWhereDateTime.ToString("yyyy-MM-dd") + " 00:00:00";// HH

                                        //pre update number
                                        tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                        if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                        {
                                            preNum = 0;
                                        }

                                        allCount = OracleDal.GetCount(td[0].Trim(), tmpwhere, parameters3);
                                        logger.DebugFormat("*********Table: {0},已上传：{1} ,Oracle 现在有数据：{2}.当日：{3}", td[0], preNum, allCount, DateTime.Now.ToString("yyyyMMdd"));

                                        if (preNum >= allCount)
                                        {
                                            logger.DebugFormat("*********(已上传数) {0} >= {1} (Oracle 现在有数据),无需更新.", preNum, allCount);
                                        }
                                        else
                                        {
                                            var tmptoUpdate = (allCount - preNum);
                                            logger.DebugFormat("*********需更新数:{0}.", tmptoUpdate);
                                            var tmporderby = td[2].Trim();
                                            if (td[2].Trim().ToLower().Equals("no"))
                                            {
                                                tmporderby = "";
                                            }
                                            tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate, parameters3);

                                        }

                                        tmpds.DataSetName = td[0].Trim();
                                        break;
                                    case 4:
                                        //key: P|where|C get P：父，C: 子 根据P的Key得到C.的数据。
                                        //get the last ID
                                        var tmpORG_START = AutoUpdateData._iniToday.IniReadValue("TableKeyLastValue", tmpKeyLast);
                                        DateTime tmpORG_START_DATE = DateTime.Now;

                                        if (!DateTime.TryParse(tmpORG_START, out tmpORG_START_DATE))
                                        {
                                            tmpORG_START_DATE = DateTime.Now;
                                        }
                                        // set tmpwhere
                                        tmpwhere += " and " + td[1] + ">=to_date(:gxsj,'yyyy-MM-dd HH24:mi:ss')";
                                        OracleParameter[] parameters4 = { new OracleParameter(":gxsj", OracleDbType.Varchar2, 10) };
                                        //no time
                                        // parameters[0].Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";// HH
                                        parameters4[0].Value = tmpORG_START_DATE.ToString("yyyy-MM-dd") + " 00:00:00";// HH

                                        //pre update number
                                        tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                        if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                        {
                                            preNum = 0;
                                        }

                                        //get all count form oracle db

                                        allCount = OracleDal.GetCount(td[0].Trim(), tmpwhere, parameters4);
                                        logger.DebugFormat("*********Table: {0},已上传：{1} ,Oracle 现在有数据：{2}.当日：{3}", td[0], preNum, allCount, DateTime.Now.ToString("yyyyMMdd"));

                                        if (preNum >= allCount)
                                        {
                                            logger.DebugFormat("*********(已上传数) {0} >= {1} (Oracle 现在有数据),无需更新.", preNum, allCount);
                                        }
                                        else
                                        {
                                            var tmptoUpdate = (allCount - preNum);
                                            logger.DebugFormat("*********需更新数:{0}.", tmptoUpdate);
                                            var tmporderby = td[2].Trim();
                                            if (td[2].Trim().ToLower().Equals("no"))
                                            {
                                                tmporderby = "";
                                            }
                                            tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate, parameters4);

                                        }

                                        tmpds.DataSetName = td[0].Trim();
                                        //to Get C
                                        if (!AutoUpdateData._tableKeyList.ContainsKey(tmpKeyname))
                                        {
                                            isSon = false;
                                            logger.DebugFormat("*************************表：{0} 没有设置主键，无法更新子表:{1}。", td[0], td[3]);
                                        }
                                        else
                                        {
                                            //var tmpkeyValue = AutoUpdateData._tableKeyList[tmpKeyname];
                                            //var tmpkeys = tmpkeyValue.Split(',');
                                            //logger.DebugFormat("*************************表：{0} 的主键：{1}。", td[0], tmpkeyValue);
                                            isSon = true;
                                        }
                                        break;
                                    default:

                                        tmpds.DataSetName = td[0].Trim();
                                        break;
                                }
                                //**************************同步表
                                StartToMSSQL(tmpds, tmpKeyLast);

                                //for father and son
                                if (isSon)
                                {
                                    if (tmpds.Tables.Count > 0)
                                    {
                                        var tmpkeyValue = AutoUpdateData._tableKeyList[tmpKeyname];
                                        var tmpkeys = tmpkeyValue.Split(',');
                                        if (tmpkeys.Count() <= 0)
                                        {
                                            logger.DebugFormat("*************************表：{0} 的主键 为空：{1}。", td[0], tmpkeyValue);
                                        }
                                        else
                                        {
                                            logger.DebugFormat("*************************表：{0} 的主键：{1}。", td[0], tmpkeyValue);
                                            logger.DebugFormat("*************************开始更新表：{0}，子表：{1}。", td[0], td[3]);

                                            foreach (DataRow p in tmpds.Tables[0].Rows)
                                            {
                                                try
                                                {
                                                    var tmpsonwhere = "SELECT * FROM " + td[3].Trim() + " where ";
                                                    if (tmpkeys.Count() > 0)
                                                    {
                                                        if (tmpkeys.Count() == 1)
                                                        {
                                                            tmpsonwhere += tmpkeys[0] + "='" + p[tmpkeys[0]].ToString().Trim() + "'";
                                                        }
                                                        else
                                                        {
                                                            for (int i = 0; i < tmpkeys.Count(); i++)
                                                            {
                                                                if (i == 0)
                                                                {
                                                                    tmpsonwhere += tmpkeys[0] + "='" + p[tmpkeys[0]].ToString().Trim() + "'";
                                                                }
                                                                else
                                                                {
                                                                    tmpsonwhere += " and " + tmpkeys[i] + "='" + p[tmpkeys[i]].ToString().Trim() + "'";
                                                                }
                                                            }
                                                        }

                                                    }
                                                    //get data
                                                    var tmpSon = OracleDal.Query(tmpsonwhere);
                                                    tmpSon.DataSetName = td[3].Trim();

                                                    //**************************同步子表
                                                    StartToMSSQL(tmpSon, "");
                                                }
                                                catch (Exception ex)
                                                {
                                                    logger.DebugFormat("****************************更新主表：{0},记录：{1}  -->的子表失败。{2}", td[0], (p[0].ToString() + "," + p[1].ToString() + "," + p[2].ToString()), ex.Message);
                                                    continue;
                                                }


                                            }

                                        }

                                    }
                                    else
                                    {
                                        logger.DebugFormat("*************************表：{0} 的数据 为空。", td[0]);
                                    }

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

                                logger.DebugFormat("*********Table: {0},已上传：{1} ,Oracle 现在有数据：{2}.", item.Key, preNum, allCount);

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

                                    StartToMSSQL(tmpds, "");

                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            logger.DebugFormat("{0}表同步有问题，开始同步下个表。Error:{1}.", item.Key, ex.Message);
                            continue;
                        }

                    }
                }
                else
                {
                    logger.Error("no Table,Please check Set.ini,and add Table.");
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
        public void StartToMSSQL(DataSet item, string setLastValue)
        {

            if (item.Tables.Count <= 0)
            {
                logger.DebugFormat("开始同步表：{0},表中无任何记录。", item.DataSetName);
                return;
            }
            logger.DebugFormat("开始同步表：{0}，更新条数为：{1}。更新方式：{2}", item.DataSetName, item.Tables[0].Rows.Count,AutoUpdateData._updatemode);
            try
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
                        //update mode
                        updateMode(strSQLinsert, tmpcolDS, item, row);
                        //gen SQL all
                        var tmpinstall = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, row);
                        if (!string.IsNullOrEmpty(tmpinstall))
                        {
                            strSQLinsert.Add(tmpinstall);
                        }
                    }
                    //upload to mssql 
                    updateToMSSQL(item, strSQLinsert, setLastValue, "Upload ");

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
                            nextdiffCount++;

                            //update mode
                            updateMode(strSQLinsert, tmpcolDS, item, item.Tables[0].Rows[i]);
                            //gen sql by batch
                            var tmpinstall = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, item.Tables[0].Rows[i]);
                            if (!string.IsNullOrEmpty(tmpinstall))
                            {
                                strSQLinsert.Add(tmpinstall);
                            }

                            if (nextdiffCount % AutoUpdateData._txt1batchNum == 0)
                            {
                                break;
                            }
                        }

                        //upload to mssql 
                        updateToMSSQL(item, strSQLinsert, setLastValue, "Upload ");


                        tmpCount++;

                    } while (nextdiffCount < allCount);

                }
            }
            catch (Exception ex)
            {
                logger.DebugFormat("*************{0}:更新出现问题，继续同步下个表.error：{1}", item.DataSetName, ex.Message);
            }
        }
        public static void updateMode(List<String> strSQLinsert, DataSet tmpcolDS, DataSet item, DataRow row)
        {
            //get sql update mode
            //1-删除后再追加 2-直接更新
            if (AutoUpdateData._updatemode.StartsWith("1-"))
            {
                _is1 = true;
                //get all key for table
                var tmpKeyname = item.DataSetName + "_KEY";
                if (!AutoUpdateData._tableKeyList.ContainsKey(tmpKeyname))
                {
                    logger.DebugFormat("1*****************表：{0},无主键，无法：1-删除后再追加。", item.DataSetName);
                }
                else
                {
                    var tmpkeyValue = AutoUpdateData._tableKeyList[tmpKeyname];
                    var tmpkeys = tmpkeyValue.Split(',');
                    if (tmpkeys.Count() <= 0)
                    {
                        logger.DebugFormat("2*****************表：{0},无主键，无法：1-删除后再追加。", item.DataSetName);
                    }
                    else
                    {
                        var tmpDeleteForSQL = OracleDal.getSQLColumnsForDeleteByKeys(tmpcolDS, item.DataSetName, tmpkeys, row);

                        if (!string.IsNullOrEmpty(tmpDeleteForSQL))
                        {
                            strSQLinsert.Add(tmpDeleteForSQL);
                        }
                    }
                }

            }
            else if (AutoUpdateData._updatemode.StartsWith("2-"))
            {
               
            }
        }
        private static void updateToMSSQL(DataSet item, List<string> strSQLinsert, string setLastValue, string msg)
        {
            try
            {

                var dd = DbHelperSQL.ExecuteSqlTran(strSQLinsert,_is1);

                if (dd > 0)
                {
                    //save upload count for each;
                    var tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", item.DataSetName);
                    int getNum = 0;
                    if (!int.TryParse(tmpTableTakeDataNum, out getNum))
                    {
                        getNum = 0;
                    }
                    var tmpcount = strSQLinsert.Count;
                    if (_is1)
                    {
                        tmpcount = tmpcount / 2;
                    }
                    int tmpcountNum = getNum + tmpcount;

                    AutoUpdateData._iniToday.IniWriteValue("TableTakeDataNum", item.DataSetName, tmpcountNum.ToString());


                    if (!string.IsNullOrEmpty(setLastValue))
                    {
                        var count = item.Tables[0].Rows.Count - 1;
                        var tmpkey = setLastValue.Split('.');
                        var lastvalue = item.Tables[0].Rows[count][tmpkey[1].ToString()];
                        AutoUpdateData._iniToday.IniWriteValue("TableKeyLastValue", setLastValue, lastvalue.ToString());
                    }
                    logger.DebugFormat(msg + " Table:[{0}],Success Count:{1}", item.DataSetName, dd);
                }
                else
                {
                    logger.DebugFormat(msg + " Fails Table:[{0}],Success Count:{1}", item.DataSetName, dd);
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public static bool _is1 { get; set; }
    }
}