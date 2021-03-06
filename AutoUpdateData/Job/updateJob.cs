﻿using AutoUpdateData.Core.dal;
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

        public static string _typeOfTable { get; set; }
        public static string _sql { get; set; }
        public static string _time_start { get; set; }
        public static string _time_done { get; set; }

        public void Execute(Quartz.IJobExecutionContext context)
        {


            if (AutoUpdateData._isUploading)
            {
                logger.DebugFormat("***************************Previous job is In Upload. Please wait。。。 {0}", context.PreviousFireTimeUtc.Value.DateTime);
                AutoUpdateData.jobflag("P Please wait。。。revious job is In Upload:" + context.PreviousFireTimeUtc.Value.DateTime);
                return;
            }

            DataTable tblPicked;
            BssLineStock bssLineStock = new BssLineStock();

            AutoUpdateData._isUploading = true;
            //get sql update mode
            //1-删除后再追加 2-直接更新
            if (AutoUpdateData._updatemode.StartsWith("1"))
            {
                _is1 = true;
            }
            else if (AutoUpdateData._updatemode.StartsWith("2"))
            {
                _is1 = false;
            }
            else
            {
                _is1 = true;
            }

            logger.DebugFormat("执行更新任务!!!!!!!!!!!!!!!");
            AutoUpdateData.jobflag("Is Runing, Next Exec Job Time:" + context.NextFireTimeUtc.Value.DateTime);
            try
            {

                var _tmpOracleDBname = AutoUpdateData._DBOracle11DBname;
                //init even tInitIniToday
                AutoUpdateData.tInitIniToday(DateTime.Now.ToString("yyyyMMdd"));
                //get
                if (AutoUpdateData._tableList.Count > 0)
                {
                    logger.Debug("执行数据获取任务!!!!!!!!!!!!!!!");
                    var tmpBatch = AutoUpdateData._txt1batchNum;
                    AutoUpdateData._dsList.Clear();


                    var tmpwhereFirst = getInSql(AutoUpdateData._CONTRACT, "CONTRACT", false);
                    if (string.IsNullOrEmpty(tmpwhereFirst))
                    {
                        return;
                    }
                    var tmpwhere = tmpwhereFirst;

                    foreach (var item in AutoUpdateData._tableList)
                    {
                        try
                        {
                            //init attr
                            _typeOfTable = "0";
                            _time_start = DateTime.Now.ToString();
                            _time_done = DateTime.Now.ToString();
                            _sql = "select * from ";
                            tmpwhere = tmpwhereFirst;

                            if (item.Key.Contains('|'))
                            {
                                var td = item.Key.Split('|');

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
                                //if (item.Value != 3)
                                //{
                                //    continue;
                                //}

                                switch (item.Value)
                                {
                                    case 1:
                                        //key: 0 table | where 1 | order by 2  | type 3
                                        var tmpInsql = getInSql(AutoUpdateData._N_OBL_PART_TYPE, td[1].Trim(), true);


                                        if (string.IsNullOrEmpty(tmpInsql))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            tmpwhere += tmpInsql;
                                        }

                                        //pre update number
                                        tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                        if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                        {
                                            preNum = 0;
                                        }
                                        //get all count form oracle db

                                        allCount = OracleDal.GetCount(_tmpOracleDBname, td[0].Trim(), tmpwhere);
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

                                            tmpds = OracleDal.GetData(_tmpOracleDBname, td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate);

                                        }

                                        tmpds.DataSetName = td[0].Trim();
                                        _typeOfTable = td[3].Trim();
                                        break;
                                    case 2:

                                        //key: 0 table | add Id 1 | order by 2 | datefrom 3 | type 4
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
                                            _time_start = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd") + " 00:00:00";
                                            tmpwhere += " and " + td[3] + ">=to_date(:gxsj,'yyyy-MM-dd HH24:mi:ss')";
                                            allCount = OracleDal.GetCount(_tmpOracleDBname, td[0].Trim(), tmpwhere, parameters2);

                                        }
                                        else
                                        {
                                            allCount = OracleDal.GetCount(_tmpOracleDBname, td[0].Trim(), tmpwhere);
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
                                                tmpds = OracleDal.GetData(_tmpOracleDBname, td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate, parameters2);
                                            }
                                            else
                                            {
                                                tmpds = OracleDal.GetData(_tmpOracleDBname, td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate);
                                            }


                                        }

                                        tmpds.DataSetName = td[0].Trim();
                                        _typeOfTable = td[4].Trim();
                                        _time_done = OracleDal.getMaxCol(tmpds, td[3]).ToString();
                                        break;
                                    case 3:

                                        //key: 0 table|1 where|2 order by  | type 3
                                        //get per last datetime

                                        var tmpLastWhere = AutoUpdateData._iniToday.IniReadValue("TableKeyLastValue", tmpKeyLast);

                                        var tmpLastWhereDateTime = DateTime.Now;
                                        if (!DateTime.TryParse(tmpLastWhere, out tmpLastWhereDateTime))
                                        {
                                            tmpLastWhereDateTime = DateTime.Now;
                                            AutoUpdateData._iniToday.IniWriteValue("TableKeyLastValue", tmpKeyLast, tmpLastWhereDateTime.ToString());
                                        }
                                        tmpwhere += " and to_char(" + td[1] + ", 'yyyymmddHH24miss') >= :gxsj";
                                        OracleParameter[] parameters3 = { new OracleParameter(":gxsj", OracleDbType.Varchar2, 10) };
                                        //no time

                                        parameters3[0].Value = DateTime.Now.ToString("yyyyMMdd000000");

                                        //for N_AIS_SHOP_LIST_PICKED_ACT_TAB
                                        var tmpnewDateTime =DateTime.Now.ToString("yyyyMMdd000000");   
                                        if (td[0].Trim().ToUpper().Equals("N_AIS_SHOP_LIST_PICKED_ACT_TAB".ToUpper()))
                                        {
                                            tmpnewDateTime = DbHelperSQL.GetTableFieldDateTime("AISPICK_UPD_DATE", "M_CONTOROL", " [KEY-ID]='01'");
                                            parameters3[0].Value = tmpnewDateTime;
                                            logger.DebugFormat("**************{0},{1} change to new value:{2}.", td[0], td[1], tmpnewDateTime);
                                        }

                                        //pre update number
                                        tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                        if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                        {
                                            preNum = 0;
                                        }

                                        allCount = OracleDal.GetCount(_tmpOracleDBname, td[0].Trim(), tmpwhere, parameters3);
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
                                            tmpds = OracleDal.GetData(_tmpOracleDBname, td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate, parameters3);

                                        }

                                        tmpds.DataSetName = td[0].Trim();
                                        _typeOfTable = td[3].Trim();
                                        _time_done = OracleDal.getMaxCol(tmpds, td[1]).ToString();

                                        #region new update 2015-12-02
                                        if (tmpds.Tables.Count > 0 && td[0].Trim().ToUpper().Equals("N_AIS_SHOP_LIST_PICKED_ACT_TAB".ToUpper()))
                                        {
                                            logger.DebugFormat("*******开始更新{0}的相关表[M_PARTS_STOCK,M_LINE_PARTS_STOCK]", td[0].Trim());
                                            //AIS配膳リストピッキング実績-> AIS配膳リストピッキング実績
                                            tblPicked = tmpds.Tables[0];// bssLineStock.GetPickedActData();
                                            //更新ライン在庫マス数据
                                            if (bssLineStock.SetLinePartsStockData(tblPicked) <= 0)
                                            {
                                                //TODO： 更新失败
                                                logger.ErrorFormat("******* {0} 的相关表[M_PARTS_STOCK,M_LINE_PARTS_STOCK] 更新失败", td[0].Trim());
                                            }
                                            else
                                            {
                                                logger.DebugFormat("******* {0} 的相关表[M_PARTS_STOCK,M_LINE_PARTS_STOCK] 更新Success", td[0].Trim());

                                                var tmpExist = DbHelperSQL.GetCount("M_CONTOROL", "[KEY-ID]='01'");
                                                var tmpsql = "";
                                                if (tmpExist > 0)
                                                {
                                                    tmpsql = string.Format("UPDATE M_CONTOROL set [AISPICK_UPD_DATE]='{0}' where [KEY-ID]='01'", _time_done);
                                                }
                                                else
                                                {
                                                    tmpsql = string.Format("INSERT INTO M_CONTOROL([KEY-ID],[AISPICK_UPD_DATE])  VALUES('{0}','{1}')", "01", _time_done);
                                                }

                                                var tmpdd = DbHelperSQL.ExecuteSql(tmpsql);

                                                if (tmpdd > 0)
                                                {
                                                    logger.DebugFormat("*******Success M_CONTOROL 更新时间 AISPICK_UPD_DATE success", td[0].Trim());
                                                }
                                                else
                                                {
                                                    logger.DebugFormat("*******Error M_CONTOROL 更新时间 AISPICK_UPD_DATE Error", td[0].Trim());
                                                }

                                            }
                                        }


                                        #endregion

                                        break;
                                    case 4:
                                        //key: P 0|where 1| order by 2 | C 3 | type 4
                                        //get P：父，C: 子 根据P的Key得到C.的数据。
                                        //get the last ID

                                        // set tmpwhere
                                        tmpwhere += " and " + td[1] + ">=to_date(:gxsj,'yyyy-MM-dd HH24:mi:ss')";
                                        OracleParameter[] parameters4 = { new OracleParameter(":gxsj", OracleDbType.Varchar2, 10) };

                                        //set time
                                        parameters4[0].Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";// HH


                                        logger.DebugFormat("****TESTFKG:{0}，ORG_START_DATE:{1}   【0:の場合 PCの日付を抽出条件にする。1:の場合 環境ファイル内の日付（yyyy-mm-dd)を設定し、その日付を抽出条件にする】", AutoUpdateData._TESTFKG, AutoUpdateData._ORG_START_DATE);
                                        if (AutoUpdateData._TESTFKG.Equals("1"))
                                        {
                                            logger.DebugFormat("****使用配置文件中的日期。{0}", AutoUpdateData._ORG_START_DATE);
                                            parameters4[0].Value = AutoUpdateData._ORG_START_DATE + " 00:00:00";// HH
                                        }
                                        var trytmpDD = DateTime.Now;
                                        var chedkdate = DateTime.TryParse(parameters4[0].Value.ToString(), out trytmpDD);
                                        if (!chedkdate)
                                        {
                                            logger.DebugFormat("**配置文件提供ORG_START_DATE的值不符合要求(yyyy-MM-dd)，value:{0}.则使用当天日期。", AutoUpdateData._ORG_START_DATE, DateTime.Now.ToString("yyyy-MM-dd"));
                                            logger.ErrorFormat("**配置文件提供ORG_START_DATE的值不符合要求(yyyy-MM-dd)，value:{0}.则使用当天日期。", AutoUpdateData._ORG_START_DATE, DateTime.Now.ToString("yyyy-MM-dd"));
                                            parameters4[0].Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";// HH
                                        }

                                        // parameters4[0].Value = tmpORG_START_DATE.ToString("yyyy-MM-dd") + " 00:00:00";// HH

                                        //pre update number
                                        tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", td[0].Trim());

                                        if (!int.TryParse(tmpTableTakeDataNum, out preNum))
                                        {
                                            preNum = 0;
                                        }

                                        //get all count form oracle db

                                        allCount = OracleDal.GetCount(_tmpOracleDBname, td[0].Trim(), tmpwhere, parameters4);

                                        logger.DebugFormat("*********Table: {0},已上传：{1} ,Oracle 现在有数据：{2}.>=条件日期：{3}", td[0], preNum, allCount, trytmpDD.ToString("yyyy-MM-dd"));

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
                                            tmpds = OracleDal.GetData(_tmpOracleDBname, td[0].Trim(), tmpwhere, tmporderby, preNum, tmptoUpdate, parameters4);

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
                                        tmpds.DataSetName = td[0].Trim();
                                        _typeOfTable = td[4].Trim();
                                        _time_done = OracleDal.getMaxCol(tmpds, td[1]).ToString();
                                        break;
                                    default:

                                        tmpds.DataSetName = td[0].Trim();
                                        if (td[0].IndexOf('.') > 0)
                                        {
                                            tmpds.DataSetName = td[0].Trim().Split('.')[1];
                                        }
                                        break;
                                }
                                _sql += td[0].Trim() + " where " + tmpwhere;
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
                                            string sonTmpsonwhereAll = "";
                                            foreach (DataRow p in tmpds.Tables[0].Rows)
                                            {
                                                string sonTmpsonwhere = "";
                                                try
                                                {
                                                    var tmpsonwhere = "";

                                                    tmpsonwhere = "SELECT * FROM " + td[3].Trim() + " where ";

                                                    if (!string.IsNullOrEmpty(_tmpOracleDBname))
                                                    {
                                                        tmpsonwhere = "SELECT * FROM " + _tmpOracleDBname + "." + td[3].Trim() + " where ";
                                                    }
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
                                                    sonTmpsonwhereAll += sonTmpsonwhere + "\n";
                                                    //get data
                                                    var tmpSon = OracleDal.Query(tmpsonwhere);

                                                    tmpSon.DataSetName = td[3].Trim();
                                                    //**************************同步子表
                                                    tmpallSonCount += OracleDal.StartToMSSQL(_is1, true, tmpSon, "");
                                                }
                                                catch (Exception ex)
                                                {
                                                    logger.ErrorFormat("****************************更新主表：{0},记录：{1}  -->的子表失败。{2}", td[0], (p[0].ToString() + "," + p[1].ToString() + "," + p[2].ToString()), ex.Message);
                                                    //OracleDal.ilog(td[3].Trim(), allCount, AutoUpdateData._CONTRACT + ",Fail," + AutoUpdateData._updatemode, "AutoUpdateOracleMSSQL: SQL:" + sonTmpsonwhere + " Fail. Error:" + ex.Message, AutoUpdateData._ipAddMac);

                                                    OracleDal.ilog("error", updateJob._typeOfTable, updateJob._time_start, updateJob._time_done, sonTmpsonwhere, allCount, AutoUpdateData._ipAddMac, AutoUpdateData._CONTRACT + "|" + AutoUpdateData._updatemode);

                                                    continue;
                                                }


                                            }
                                            //OracleDal.ilog(td[3].Trim(), tmpallSonCount, AutoUpdateData._CONTRACT + ",Success," + AutoUpdateData._updatemode, "AutoUpdateOracleMSSQL:Update Count:" + tmpallSonCount + " Success.", AutoUpdateData._ipAddMac);
                                            OracleDal.ilog("success", updateJob._typeOfTable, updateJob._time_start, updateJob._time_done, sonTmpsonwhereAll, tmpallSonCount, AutoUpdateData._ipAddMac, AutoUpdateData._CONTRACT + "|" + AutoUpdateData._updatemode);


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

                                var allCount = OracleDal.GetCount(_tmpOracleDBname, item.Key, "");

                                logger.DebugFormat("*********Table: {0},已上传：{1} ,Oracle 现在有数据：{2}.", item.Key, preNum, allCount);

                                if (preNum >= allCount)
                                {
                                    logger.DebugFormat("*********(已上传数) {0} >= {1} (Oracle 现在有数据),无需更新.", preNum, allCount);
                                }
                                else
                                {

                                    var tmptoUpdate = (allCount - preNum);
                                    logger.DebugFormat("*********需更新数:{0}", tmptoUpdate);

                                    var tmpds = OracleDal.GetData(_tmpOracleDBname, item.Key, "", "", preNum, tmptoUpdate);
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
            AutoUpdateData.jobflag("Notice: Current Job is Run Over, Next Exec Job Time:" + context.NextFireTimeUtc.Value.DateTime);


        }
        //1-删除后再追加 2-直接更新
        public static bool _is1 { get; set; }
        public string getInSql(string strValueSplit, string colname, bool hasAnd)
        {
            var tmpPC = strValueSplit.Split(',');
            var tmpInSQL = "";
            if (tmpPC.Count() <= 0)
            {
                logger.ErrorFormat("*********设置 {0} 有问题，value:{1}", colname, strValueSplit);
                return null;//continue;
            }

            if (tmpPC.Count() == 1)
            {
                tmpInSQL = colname + "='" + strValueSplit + "' ";

                if (hasAnd)
                {
                    return " and " + tmpInSQL;
                }
                else
                {
                    return tmpInSQL;
                }


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

                tmpInSQL = colname + " in(" + tOr + ")";

                if (hasAnd)
                {
                    return " and " + tmpInSQL;
                }
                else
                {
                    return tmpInSQL;
                }
            }
        }
    }
}