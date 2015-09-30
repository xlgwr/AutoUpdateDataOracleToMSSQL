using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using AutoUpdateData.Core.enity;
using log4net;
using System.Reflection;
using System.Net;
using System.Management;
using AutoUpdateData.Service.Job;

namespace AutoUpdateData.Core.dal
{
    public class OracleDal : DbHelperOra
    {
        //var tmpsrt = "user id=nec;password=nec;data source=xe";
        //   OracleConnection con = new OracleConnection(tmpsrt);
        //   con.Open();
        //   MessageBox.Show(con.ServerVersion);
        //   con.Close();
        //   con.Dispose();
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static int GetCount(string schema, string TableName, string strwhere)
        {
            if (!string.IsNullOrEmpty(schema))
            {
                TableName = schema.Trim() + "." + TableName;
            }
            string strsql = "select count(*) from " + TableName;
            if (!string.IsNullOrEmpty(strwhere))
            {
                strsql += " where " + strwhere;
            }
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public static int GetCount(string schema, string TableName, string strwhere, params OracleParameter[] cmdParms)
        {
            if (!string.IsNullOrEmpty(schema))
            {
                TableName = schema.Trim() + "." + TableName;
            }

            string strsql = "select count(*) from " + TableName;
            if (!string.IsNullOrEmpty(strwhere))
            {
                strsql += " where " + strwhere;
            }
            object obj = GetSingle(strsql, cmdParms);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public static DataSet GetTableColumns(string tablename)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select t.COLUMN_NAME,t.DATA_TYPE,t.DATA_LENGTH,t.NULLABLE,t.COLUMN_ID from user_tab_columns t,user_col_comments c where t.table_name = c.table_name and t.column_name = c.column_name");
            sb.Append("  and t.table_name = '" + tablename + "'");

            logger.Debug(sb.ToString());
            var result = DbHelperOra.Query(sb.ToString());
            return result;
        }
        /// <summary>
        /// top 10
        /// select * from table where rownum<=10;
        /// more page
        /// 根椐表及条件，获取前面N条记录
        /// SELECT * FROM ( SELECT A.*, ROWNUM RN 
        //  FROM (SELECT * FROM TABLE_NAME) A WHERE ROWNUM <= 40 ) WHERE RN >= 21
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="tmpwhere"></param>
        /// <param name="rownum"></param>
        /// <returns></returns>
        public static DataSet GetData(string schema, string tablename, string tmpwhere, string orderby, int preNum, int rownumBatch)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(schema))
            {
                tablename = schema.Trim() + "." + tablename;
            }

            int takeNum = preNum + rownumBatch;

            sb.Append("SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT * FROM " + tablename + ") A ");
            if (!string.IsNullOrEmpty(tmpwhere))
            {
                sb.Append(" where " + tmpwhere);
                sb.Append(" and ROWNUM <= " + takeNum.ToString() + ") WHERE RN >" + preNum.ToString());
            }
            else
            {

                sb.Append(" where ROWNUM <= " + takeNum.ToString() + ") WHERE RN >" + preNum.ToString());
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                sb.Append(" order by " + orderby);
            }

            logger.Debug(sb.ToString());
            var result = DbHelperOra.Query(sb.ToString());
            return result;
        }
        /// <summary>
        /// 根椐表及条件，获取前面N条记录
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="tmpwhere"></param>
        /// <param name="orderby"></param>
        /// <param name="rownum"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static DataSet GetData(string schema, string tablename, string tmpwhere, string orderby, int preNum, int rownumBatch, params OracleParameter[] cmdParms)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(schema))
            {
                tablename = schema.Trim() + "." + tablename;
            }

            int takeNum = preNum + rownumBatch;

            sb.Append("SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT * FROM " + tablename + ") A ");
            if (!string.IsNullOrEmpty(tmpwhere))
            {
                sb.Append(" where " + tmpwhere);
                sb.Append(" and ROWNUM <= " + takeNum.ToString() + ") WHERE RN >" + preNum.ToString());
            }
            else
            {

                sb.Append(" where ROWNUM <= " + takeNum.ToString() + ") WHERE RN >" + preNum.ToString());
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                sb.Append(" order by " + orderby);
            }

            logger.Debug(sb.ToString());
            var result = DbHelperOra.Query(sb.ToString(), cmdParms);
            return result;
        }
        public static Dictionary<string, column> getColumns(DataSet ds)
        {
            var tmpColumns = new Dictionary<string, column>();

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    var tb = ds.Tables[0];
                    foreach (DataRow item in tb.Rows)
                    {
                        if (tmpColumns.ContainsKey(item["COLUMN_NAME"].ToString()))
                        {
                            var tmpcol = new column()
                            {
                                COLUMN_NAME = item["COLUMN_NAME"].ToString(),
                                COLUMN_ID = Convert.ToInt32(item["COLUMN_ID"]),
                                DATA_LENGTH = Convert.ToInt32(item["DATA_LENGTH"]),
                                DATA_TYPE = item["DATA_TYPE"].ToString(),
                                NULLABLE = item["NULLABLE"].ToString()
                            };
                            tmpColumns.Add(tmpcol.COLUMN_NAME, tmpcol);
                        }
                    }


                }
            }
            return tmpColumns;
        }

        public static DateTime getMaxCol(DataSet ds, string colName)
        {
            var tmpresult = DateTime.Now;
            try
            {
                if (ds == null)
                {
                    return tmpresult;
                }
                if (ds.Tables.Count < 0)
                {
                    return tmpresult;
                }
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    return tmpresult;
                }

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    var tmpcolValue = DateTime.Now;

                    if (DateTime.TryParse(item[colName].ToString(), out tmpcolValue))
                    {
                        if (tmpcolValue > tmpresult)
                        {
                            tmpresult = tmpcolValue;
                        }
                    }
                }

            }
            catch (Exception)
            {
                return tmpresult;
            }
            return tmpresult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsColumn"></param>
        /// <param name="strTablename"></param>
        /// <returns></returns>
        public static string getSQLColumnsForInsert(DataSet dsColumn, string strTablename, DataRow dvalue)
        {

            //INSERT INTO 
            //[nec].[dbo].[INVENTORY_PART_TAB]
            //([PART_NO],[CONTRACT],[ASSET_CLASS],[ACCOUNTING_GROUP],[LAST_ACTIVITY_DATE])
            //VALUES    
            //(@PART_NO,@CONTRACT,@ASSET_CLASS,@ACCOUNTING_GROUP,@LAST_ACTIVITY_DATE)

            var sb = new StringBuilder();
            var sbvalue = new StringBuilder();

            sb.Append(" INSERT INTO ");
            sb.Append(" [dbo].[" + strTablename.Trim() + "] (");

            if (dsColumn != null)
            {
                if (dsColumn.Tables.Count > 0)
                {
                    var tb = dsColumn.Tables[0];
                    if (tb.Rows.Count > 0)
                    {
                        var tmpcount = tb.Rows.Count;
                        for (int i = 0; i < tmpcount; i++)
                        {
                            var colname = tb.Rows[i]["COLUMN_NAME"].ToString();
                            var dd = dvalue[colname].GetType().ToString();

                            if (i < tmpcount - 1)
                            {

                                sb.Append("[" + colname + "],");

                                if (dvalue[colname] != null)
                                {

                                    sbvalue.Append("N'" + dvalue[colname].ToString() + "',");
                                }
                                else
                                {
                                    sbvalue.Append("'',");
                                }
                            }
                            else
                            {

                                sb.Append("[" + colname + "]");

                                sb.Append(" ) VALUES ( ");

                                //System.DateTime
                                if (dvalue[colname] != null)
                                {
                                    sbvalue.Append("N'" + dvalue[colname].ToString() + "' )");
                                }
                                else
                                {
                                    sbvalue.Append("'')");
                                }
                            }

                        }

                        return sb.Append(sbvalue.ToString()).ToString();
                    }
                }
            }
            return null;

        }
        /// <summary>
        /// true: top 1 exist, false: Delete
        /// </summary>
        /// <param name="isForDeleteOrExist"></param>
        /// <param name="dsColumn"></param>
        /// <param name="strTablename"></param>
        /// <param name="tmpkeys"></param>
        /// <param name="dvalue"></param>
        /// <returns></returns>
        public static string getSQLColumnsForDeleteByKeys(bool isForDeleteOrExist, DataSet dsColumn, string strTablename, string[] tmpkeys, DataRow dvalue)
        {

            //delete dbo.INVENTORY_PART_TAB where dd='xx' and bb='ddd'



            if (tmpkeys.Count() <= 0)
            {
                logger.DebugFormat("*************************表：{0} 的主键 为空：{1}。", strTablename, tmpkeys);
            }
            else
            {
                //logger.DebugFormat("*************************表：{0} 的主键：{1}。", strTablename, tmpkeys);
                var sb = new StringBuilder();

                if (isForDeleteOrExist)
                {
                    sb.Append(" select count(*) from ");
                }
                else
                {
                    sb.Append(" delete ");
                }
                sb.Append(" [dbo].[" + strTablename.Trim() + "] where ");

                if (dsColumn != null)
                {
                    if (dsColumn.Tables.Count > 0)
                    {
                        var tb = dsColumn.Tables[0];

                        var p = dvalue;
                        if (tmpkeys.Count() == 1)
                        {
                            sb.Append(tmpkeys[0] + "=N'" + p[tmpkeys[0]].ToString().Replace("'", "").Trim() + "'");
                        }
                        else
                        {
                            for (int i = 0; i < tmpkeys.Count(); i++)
                            {
                                if (i == 0)
                                {
                                    sb.Append(tmpkeys[0] + "=N'" + p[tmpkeys[0]].ToString().Replace("'", "").Trim() + "'");
                                }
                                else
                                {
                                    sb.Append(" and " + tmpkeys[i] + "=N'" + p[tmpkeys[i]].ToString().Replace("'", "").Trim() + "'");
                                }
                            }
                        }
                        return sb.ToString();

                    }
                }
            }
            return null;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsColumn"></param>
        /// <param name="strTablename"></param>
        /// <returns></returns>
        public static string getSQLColumnsForUpdateByKeys(DataSet dsColumn, string strTablename, string[] strkeys, DataRow dvalue)
        {

            //UPDATE [dbo].[INVENTORY_PART_TAB]
            //   SET [PART_NO] = <PART_NO, varchar(25),>
            //      ,[CONTRACT] = <CONTRACT, varchar(5),>
            //      ,[ASSET_CLASS] = <ASSET_CLASS, varchar(2),>
            //      ,[ACCOUNTING_GROUP] = <ACCOUNTING_GROUP, varchar(5),>
            //      ,[PRIME_COMMODITY] = <PRIME_COMMODITY, varchar(5),>
            //      ,[LAST_ACTIVITY_DATE] = <LAST_ACTIVITY_DATE, datetime,>
            //      ,[HAZARD_CODE] = <HAZARD_CODE, varchar(6),>
            // WHERE <搜索条件,,>

            var sb = new StringBuilder();

            sb.Append(" UPDATE ");
            sb.Append(" [dbo].[" + strTablename.Trim() + "] SET ");

            if (dsColumn != null)
            {
                if (dsColumn.Tables.Count > 0)
                {
                    var tb = dsColumn.Tables[0];
                    if (tb.Rows.Count > 0)
                    {
                        var tmpcount = tb.Rows.Count;
                        for (int i = 0; i < tmpcount; i++)
                        {

                            var colname = tb.Rows[i]["COLUMN_NAME"].ToString();

                            if (strkeys.Contains(colname))
                            {
                                //remove keys
                                continue;
                            }

                            if (i < tmpcount - 1)
                            {


                                sb.Append("[" + colname + "]=N'" + dvalue[colname].ToString().Replace("'", "").Trim() + "',");

                            }
                            else
                            {

                                sb.Append(" [" + colname + "]=N'" + dvalue[colname].ToString().Replace("'", "").Trim() + "'");
                            }

                        }
                        //where
                        if (strkeys.Count() <= 0)
                        {
                            return null;
                        }
                        else
                        {
                            sb.Append(" where ");
                            if (strkeys.Count() == 1)
                            {
                                sb.Append(strkeys[0] + "=N'" + dvalue[strkeys[0]].ToString().Replace("'", "").Trim() + "'");
                            }
                            else
                            {
                                for (int i = 0; i < strkeys.Count(); i++)
                                {
                                    if (i == 0)
                                    {
                                        sb.Append(strkeys[0] + "=N'" + dvalue[strkeys[0]].ToString().Replace("'", "").Trim() + "'");
                                    }
                                    else
                                    {
                                        sb.Append(" and " + strkeys[i] + "=N'" + dvalue[strkeys[i]].ToString().Replace("'", "").Trim() + "'");
                                    }

                                }
                            }

                            return sb.ToString();
                        }

                    }
                }
            }
            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="is1"></param>
        /// <param name="item"></param>
        /// <param name="setLastValue"></param>
        public static int StartToMSSQL(bool is1, bool isSon, DataSet item, string setLastValue)
        {
            var allExecCount = 0;
            if (item.Tables.Count <= 0)
            {
                logger.DebugFormat("#####################################开始同步表：{0},表中无任何记录。", item.DataSetName);
                return allExecCount;
            }
            logger.DebugFormat("#####################################开始同步表：{0}，更新条数为：{1}。更新方式：{2}", item.DataSetName, item.Tables[0].Rows.Count, AutoUpdateData._updatemode);

            //get Allcount
            var allCount = item.Tables[0].Rows.Count;
            var strSQLinsert = new List<String>();
            var strSQLinsertAll = new StringBuilder();
            try
            {
                //get colmuns
                var tmpcolDS = OracleDal.GetTableColumns(item.DataSetName);

                //gen sql

                if (allCount <= AutoUpdateData._txt1batchNum)
                {
                    //init str

                    foreach (DataRow row in item.Tables[0].Rows)
                    {
                        //update mode 1:d
                        updateMode(is1, strSQLinsert, tmpcolDS, item, row);
                    }

                    //foreach (var aitem in strSQLinsert)
                    //{
                    //    strSQLinsertAll.AppendLine(aitem);
                    //}

                    //upload to mssql 
                    allExecCount = updateToMSSQL(is1, item, strSQLinsert, setLastValue, "Upload ");

                }
                else
                {
                    var nextdiffCount = 0;

                    var tmpCount = 1;
                    do
                    {
                        //init str
                        strSQLinsert = new List<String>();
                        for (int i = nextdiffCount; i < allCount; i++)
                        {
                            nextdiffCount++;

                            //update mode and gen SQL
                            updateMode(is1, strSQLinsert, tmpcolDS, item, item.Tables[0].Rows[i]);


                            if (nextdiffCount % AutoUpdateData._txt1batchNum == 0)
                            {
                                break;
                            }
                        }

                        //foreach (var aitem in strSQLinsert)
                        //{
                        //    strSQLinsertAll.AppendLine(aitem);
                        //}

                        //upload to mssql 
                        allExecCount += updateToMSSQL(is1, item, strSQLinsert, setLastValue, "Upload ");


                        tmpCount++;

                    } while (nextdiffCount < allCount);

                }
                if (!isSon)
                {
                    //strSQLinsertAll.ToString()
                    ilog("success", updateJob._typeOfTable, updateJob._time_start, updateJob._time_done, updateJob._sql, allCount, AutoUpdateData._ipAddMac, AutoUpdateData._CONTRACT + "|" + AutoUpdateData._updatemode);

                }
                return allExecCount;

            }
            catch (Exception ex)
            {
                if (!isSon)
                {
                    ilog("error", updateJob._typeOfTable, updateJob._time_start, updateJob._time_done, updateJob._sql, allCount, AutoUpdateData._ipAddMac, AutoUpdateData._CONTRACT + "|" + AutoUpdateData._updatemode);

                }
                logger.ErrorFormat("*************{0}:更新出现问题，继续同步下个表.error：{1}", item.DataSetName, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Update Mode： 1-Deleted First,Then Adding
        /// </summary>
        /// <param name="_is1"></param>
        /// <param name="strSQLinsert"></param>
        /// <param name="tmpcolDS"></param>
        /// <param name="item"></param>
        /// <param name="row"></param>
        public static void updateMode(bool isOne, List<String> strSQLinsert, DataSet tmpcolDS, DataSet item, DataRow row)
        {

            //get all key for table
            var tmpKeyname = item.DataSetName + "_KEY";
            var tmpExistForSQL = "";
            var tmpDeleteForSQL = "";
            var tmpUpdateForSQL = "";
            var tmpInsertForSQL = "";
            try
            {
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
                        tmpExistForSQL = OracleDal.getSQLColumnsForDeleteByKeys(true, tmpcolDS, item.DataSetName, tmpkeys, row);

                        tmpDeleteForSQL = OracleDal.getSQLColumnsForDeleteByKeys(false, tmpcolDS, item.DataSetName, tmpkeys, row);

                        tmpUpdateForSQL = OracleDal.getSQLColumnsForUpdateByKeys(tmpcolDS, item.DataSetName, tmpkeys, row);


                    }
                }

                //update mode 1 or 2
                if (isOne)
                {
                    // for mode 1
                    if (!string.IsNullOrEmpty(tmpDeleteForSQL))
                    {
                        strSQLinsert.Add(tmpDeleteForSQL);
                    }
                    //for insert
                    //gen sql by batch
                    tmpInsertForSQL = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, row);
                    if (!string.IsNullOrEmpty(tmpInsertForSQL))
                    {
                        strSQLinsert.Add(tmpInsertForSQL);
                    }
                }
                else
                {
                    var tmpExitThis = DbHelperSQL.Exists(tmpExistForSQL);
                    // exit 
                    if (tmpExitThis)
                    {
                        //for mode 2
                        if (!string.IsNullOrEmpty(tmpUpdateForSQL))
                        {
                            strSQLinsert.Add(tmpUpdateForSQL);
                        }

                    }
                    else
                    {
                        //for insert
                        //gen sql by batch
                        tmpInsertForSQL = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, row);
                        if (!string.IsNullOrEmpty(tmpInsertForSQL))
                        {
                            strSQLinsert.Add(tmpInsertForSQL);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ",SQL:" + tmpExistForSQL + "," + tmpDeleteForSQL + "," + tmpUpdateForSQL + "," + tmpInsertForSQL);
            }


        }
        private static int updateToMSSQL(bool _is1, DataSet item, List<string> strSQLinsert, string setLastValue, string msg)
        {
            var tmpcount = strSQLinsert.Count;
            try
            {

                var dd = DbHelperSQL.ExecuteSqlTran(strSQLinsert, _is1, true);

                if (dd > 0)
                {
                    //save upload count for each;
                    var tmpTableTakeDataNum = AutoUpdateData._iniToday.IniReadValue("TableTakeDataNum", item.DataSetName);
                    int getNum = 0;
                    if (!int.TryParse(tmpTableTakeDataNum, out getNum))
                    {
                        getNum = 0;
                    }
                    if (_is1)
                    {
                        tmpcount = tmpcount / 2;
                    }
                    int tmpcountNum = 0;// getNum + tmpcount;

                    AutoUpdateData._iniToday.IniWriteValue("TableTakeDataNum", item.DataSetName, tmpcountNum.ToString());


                    if (!string.IsNullOrEmpty(setLastValue))
                    {
                        var count = item.Tables[0].Rows.Count - 1;
                        var tmpkey = setLastValue.Split('.');
                        var lastvalue = item.Tables[0].Rows[count][tmpkey[1].ToString()];

                        AutoUpdateData._iniToday.IniWriteValue("TableKeyLastValue", setLastValue, lastvalue.ToString());
                    }

                    logger.DebugFormat(msg + " Table:[{0}],Success Count:{1}", item.DataSetName, dd);
                    //ilog(item.DataSetName, dd, AutoUpdateData._CONTRACT + ",Success", "AutoUpdateOracleMSSQL:Update Count:" + dd + " Success.", AutoUpdateData._ipAddMac);

                }
                else
                {
                    //ilog(item.DataSetName, dd, AutoUpdateData._CONTRACT + ",Fail", "AutoUpdateOracleMSSQL:Update Count:" + dd + " Fail.", AutoUpdateData._ipAddMac);
                    logger.DebugFormat(msg + " Fail Table:[{0}],Success Count:{1}", item.DataSetName, dd);
                }
                return dd;
            }
            catch (Exception ex)
            {
                //logger.ErrorFormat(msg + " Fail Table:[{0}],Success Count:{1},Msg:{2}.", item.DataSetName, tmpcount, ex.Message);
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// INSERT INTO [dbo].[iLog]
        ///[type] [nvarchar](50) NULL,
        ///[system_id] [nvarchar](50) NULL,
        ///[time_start] [datetime] NULL,
        ///[time_done] [datetime] NULL,
        ///[sql] [text] NULL,
        ///[size] [int] null,
        ///[address] [nvarchar](50) null,
        ///[comment] [text] NULL,
        ///VALUES
        /// </summary>
        /// <param name="type">(error, success)</param>
        /// <param name="system_id"></param>
        /// <param name="time_start"> DateTime time_start,</param>
        /// <param name="time_done"></param>
        /// <param name="sql"></param>
        /// <param name="size"></param>
        /// <param name="address"></param>
        /// <param name="comment"></param>
        public static void ilog(string type, string system_id, string time_start, string time_done, string sql, int size, string address, string comment)
        {
            try
            {   //+ "',Getdate(),'" 
                sql = sql.Replace("'", "|");
                var tmpilogsql = "INSERT INTO [dbo].[iLog] ([type],[system_id],[time_start],[time_done],[sql],[size],[address],[comment]) VALUES (";
                tmpilogsql += "'" + type + "','" + system_id + "','" + time_start + "','" + time_done + "','" + sql + "','" + size + "','" + address + "','" + comment + "')";
                var tmptoIn = DbHelperSQL.ExecuteSql(tmpilogsql);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("**************写入日记失败：{0}", ex.Message);
            }
        }
        /// <summary>
        /// ip+mac+version
        /// </summary>
        /// <returns></returns>
        public static string getIp(bool isOnlyIp)
        {
            string ip = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection nics = mc.GetInstances();
            foreach (ManagementObject nic in nics)
            {
                if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                {
                    if (isOnlyIp)
                    {
                        ip = (nic["IPAddress"] as String[])[0];//IP地址
                        break;
                    }
                    string mac = nic["MacAddress"].ToString();//Mac地址
                    ip += "*" + (nic["IPAddress"] as String[])[0];//IP地址
                    ip += "*" + mac;
                    string ipsubnet = (nic["IPSubnet"] as String[])[0];//子网掩码
                    string ipgateway = (nic["DefaultIPGateway"] as String[])[0];//默认网关


                    break;
                }
            }
            return ip;
        }
        public static string GetClientInternetIP()
        {
            string ipAddress = "";
            using (WebClient webClient = new WebClient())
            {
                ipAddress = webClient.DownloadString("http://www.dxda.com/ip.asp");//站获得IP的网页
                //判断IP是否合法
                if (!System.Text.RegularExpressions.Regex.IsMatch(ipAddress, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
                {
                    ipAddress = webClient.DownloadString("http://www.zu14.cn/ip/");//站获得IP的网页
                }
            }
            return ipAddress;
        }

    }

}
