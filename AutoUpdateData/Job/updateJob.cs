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

            //get sql update mode
            //1-删除后再追加 2-直接更新
            if (AutoUpdateData._updatemode.StartsWith("1"))
            {
                _is1 = true;
            }
            else if (AutoUpdateData._updatemode.StartsWith("2"))
            {
                _is1 = true;
            }
            else
            {
                _is1 = false;
            }

            logger.DebugFormat("执行更新任务!!!!!!!!!!!!!!!");
            AutoUpdateData.jobflag("Is Runing, Next Exec Job Time:" + context.NextFireTimeUtc.Value.DateTime);
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

                                //test
                                
                                switch (item.Value)
                                {
                                    case 1:
                                        //key: 0 table | where 1 | order by 2 from config file
                                        var tmpPC = AutoUpdateData._PRIME_COMMODITY.Split(',');
                                        if (tmpPC.Count() <= 0)
                                        {
                                            logger.ErrorFormat("*********Table: {0} 设置 PRIME_COMMODITY 有问题，value:{1}", td[0], AutoUpdateData._PRIME_COMMODITY);
                                            continue;
                                        }
                                        if (tmpPC.Count() == 1)
                                        {
                                            tmpwhere += " and " + td[1].Trim() + "='" + AutoUpdateData._PRIME_COMMODITY + "' ";

                                        }
                                        else
                                        {
                                            //in('1','2')
                                            var tOr = "";

                                            for (int i = 0; i < tmpPC.Count(); i++)
                                            {
                                                if (i == 0)
                                                {
                                                    tOr = "'" + tmpPC[i].Trim() + "'";
                                                }
                                                else
                                                {
                                                    tOr += ",'" + tmpPC[i].Trim() + "'";
                                                }
                                            }

                                            tmpwhere += " and " + td[1].Trim() + " in(" + tOr + ")";
                                        }
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
                                        //key: 0 table | add Id 2 | order by 3 |datefrom config file
                                        //get the last ID

                                        //get from SQL by id;
                                        //var tmpTRANSACTION_ID = AutoUpdateData._iniToday.IniReadValue("TableKeyLastValue", tmpKeyLast);
                                        var tmpTRANSACTION_ID = DbHelperSQL.GetDMaxID(td[1], td[0]);

                                        // get last where
                                        tmpwhere += " and " + td[1].Trim() + ">='" + tmpTRANSACTION_ID + "' ";

                                        //pre update number
                                        tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                        if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                        {
                                            preNum = 0;
                                        }

                                        //get all count form oracle db

                                        OracleParameter[] parameters2 = { new OracleParameter(":gxsj", OracleDbType.Varchar2, 10) };
                                        parameters2[0].Value = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd") + " 00:00:00";// HH   

                                        if (tmpTRANSACTION_ID == 1)
                                        {
                                            logger.DebugFormat("******************************{0} 初始更新，加限时间-3天。", td[0]);
                                            tmpwhere += " and " + td[3] + ">=to_date(:gxsj,'yyyy-MM-dd HH24:mi:ss')";
                                            allCount = OracleDal.GetCount(td[0].Trim(), tmpwhere, parameters2);
                                        }
                                        else
                                        {
                                            allCount = OracleDal.GetCount(td[0].Trim(), tmpwhere);
                                        }

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
                                            if (tmpTRANSACTION_ID == 1)
                                            {
                                                tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate, parameters2);
                                            }
                                            else
                                            {
                                                tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate);
                                            }


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

                                        parameters3[0].Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";// HH       

                                        //parameters3[0].Value = tmpLastWhereDateTime.ToString("yyyy-MM-dd") + " 00:00:00";// HH

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
                                        parameters4[0].Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";// HH
                                        // parameters4[0].Value = tmpORG_START_DATE.ToString("yyyy-MM-dd") + " 00:00:00";// HH

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
                                OracleDal.StartToMSSQL(_is1, false, tmpds, tmpKeyLast);

                                //for father and son
                                if (isSon)
                                {
                                    var tmpallSonCount = 0;
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
                                                string sonTmpsonwhere = "";
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
                                                    //
                                                    sonTmpsonwhere = tmpsonwhere;
                                                    //get data
                                                    var tmpSon = OracleDal.Query(tmpsonwhere);
                                                    tmpSon.DataSetName = td[3].Trim();

                                                    //**************************同步子表
                                                    tmpallSonCount += OracleDal.StartToMSSQL(_is1, true, tmpSon, "");
                                                }
                                                catch (Exception ex)
                                                {
                                                    logger.ErrorFormat("****************************更新主表：{0},记录：{1}  -->的子表失败。{2}", td[0], (p[0].ToString() + "," + p[1].ToString() + "," + p[2].ToString()), ex.Message);
                                                    OracleDal.ilog(td[3].Trim(), allCount, AutoUpdateData._CONTRACT + ",Fail", "AutoUpdateOracleMSSQL: SQL:" + sonTmpsonwhere + " Fail. Error:" + ex.Message, AutoUpdateData._ipAddMac);

                                                    continue;
                                                }


                                            }
                                            OracleDal.ilog(td[3].Trim(), tmpallSonCount, AutoUpdateData._CONTRACT + ",Success", "AutoUpdateOracleMSSQL:Update Count:" + tmpallSonCount + " Success.", AutoUpdateData._ipAddMac);


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

                                    OracleDal.StartToMSSQL(_is1, false, tmpds, "");

                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            AutoUpdateData.jobflag("Error:" + ex.Message);
                            logger.ErrorFormat("{0}表同步有问题，开始同步下个表。Error:{1}.", item.Key, ex.Message);
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
                AutoUpdateData.jobflag("Error:" + ex.Message);
                logger.Error(ex);
            }
            finally
            {
                AutoUpdateData._isUploading = false;
            }

        }
        //1-删除后再追加 2-直接更新
        public static bool _is1 { get; set; }
    }
}