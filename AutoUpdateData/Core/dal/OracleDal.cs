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

        public static int GetCount(string TableName, string strwhere)
        {
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
        public static int GetCount(string TableName, string strwhere, params OracleParameter[] cmdParms)
        {


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
        public static DataSet GetData(string tablename, string tmpwhere, string orderby, int preNum, int rownumBatch)
        {
            StringBuilder sb = new StringBuilder();

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
        public static DataSet GetData(string tablename, string tmpwhere, string orderby, int preNum, int rownumBatch, params OracleParameter[] cmdParms)
        {
            StringBuilder sb = new StringBuilder();

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

                                    sbvalue.Append("'" + dvalue[colname].ToString() + "',");
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
                                    sbvalue.Append("'" + dvalue[colname].ToString() + "' )");
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

        public static string getSQLColumnsForDeleteByKeys(DataSet dsColumn, string strTablename, string[] tmpkeys, DataRow dvalue)
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

                sb.Append(" delete ");
                sb.Append(" [dbo].[" + strTablename.Trim() + "] where ");

                if (dsColumn != null)
                {
                    if (dsColumn.Tables.Count > 0)
                    {
                        var tb = dsColumn.Tables[0];

                        var p = dvalue;
                        if (tmpkeys.Count() == 1)
                        {
                            sb.Append(tmpkeys[0] + "='" + p[tmpkeys[0]].ToString().Trim() + "'");
                        }
                        else
                        {
                            for (int i = 0; i < tmpkeys.Count(); i++)
                            {
                                if (i == 0)
                                {
                                    sb.Append(tmpkeys[0] + "='" + p[tmpkeys[0]].ToString().Trim() + "'");
                                }
                                else
                                {
                                    sb.Append(" and " + tmpkeys[i] + "='" + p[tmpkeys[i]].ToString().Trim() + "'");
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
        public static string getSQLColumnsForUpdateByKeys(DataSet dsColumn, string[] strkeys, string strTablename, DataRow dvalue)
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


                                sb.Append("[" + colname + "]='" + dvalue[colname].ToString() + "',");

                            }
                            else
                            {

                                sb.Append(" [" + colname + "]='" + dvalue[colname].ToString() + "'");
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
                                sb.Append(strkeys[0] + "='" + dvalue[strkeys[0]].ToString().Trim() + "'");
                            }
                            else
                            {
                                for (int i = 0; i < strkeys.Count(); i++)
                                {
                                    if (i == 0)
                                    {
                                        sb.Append(strkeys[0] + "='" + dvalue[strkeys[0]].ToString().Trim() + "'");
                                    }
                                    else
                                    {
                                        sb.Append(" and " + strkeys[i] + "='" + dvalue[strkeys[i]].ToString().Trim() + "'");
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
                logger.DebugFormat("开始同步表：{0},表中无任何记录。", item.DataSetName);
                return allExecCount;
            }
            logger.DebugFormat("开始同步表：{0}，更新条数为：{1}。更新方式：{2}", item.DataSetName, item.Tables[0].Rows.Count, AutoUpdateData._updatemode);

            //get Allcount
            var allCount = item.Tables[0].Rows.Count;
            try
            {
                //get colmuns
                var tmpcolDS = OracleDal.GetTableColumns(item.DataSetName);

                //gen sql

                if (allCount <= AutoUpdateData._txt1batchNum)
                {
                    //init str
                    var strSQLinsert = new List<String>();

                    foreach (DataRow row in item.Tables[0].Rows)
                    {
                        //update mode
                        updateMode(is1, strSQLinsert, tmpcolDS, item, row);
                        //gen SQL all
                        var tmpinstall = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, row);
                        if (!string.IsNullOrEmpty(tmpinstall))
                        {
                            strSQLinsert.Add(tmpinstall);
                        }
                    }
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
                        var strSQLinsert = new List<String>();
                        for (int i = nextdiffCount; i < allCount; i++)
                        {
                            nextdiffCount++;

                            //update mode
                            updateMode(is1, strSQLinsert, tmpcolDS, item, item.Tables[0].Rows[i]);
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
                        allExecCount += updateToMSSQL(is1, item, strSQLinsert, setLastValue, "Upload ");


                        tmpCount++;

                    } while (nextdiffCount < allCount);

                }
                if (!isSon)
                {
                    ilog(item.DataSetName, allCount, AutoUpdateData._CONTRACT + ",Success", "AutoUpdateOracleMSSQL:Update Count:" + allCount + " Success.", AutoUpdateData._ipAddMac);

                }
                return allExecCount;

            }
            catch (Exception ex)
            {
                if (!isSon)
                {
                    ilog(item.DataSetName, allCount, AutoUpdateData._CONTRACT + ",Fail", "AutoUpdateOracleMSSQL:Update Count:" + allCount + " Fail. Error:" + ex.Message, AutoUpdateData._ipAddMac);

                }
                logger.ErrorFormat("*************{0}:更新出现问题，继续同步下个表.error：{1}", item.DataSetName, ex.Message);
                return 0;
            }
        }

        public static void updateMode(bool _is1, List<String> strSQLinsert, DataSet tmpcolDS, DataSet item, DataRow row)
        {

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
        private static int updateToMSSQL(bool _is1, DataSet item, List<string> strSQLinsert, string setLastValue, string msg)
        {
            var tmpcount = strSQLinsert.Count;
            try
            {

                var dd = DbHelperSQL.ExecuteSqlTran(strSQLinsert, _is1);

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
        ///  ([lName]
        ///       ,[ltype]
        ///      ,[lDesc]
        ///      ,[ldate]
        ///      ,[remark])
        ///VALUES
        ///      (<lName, nvarchar(50),>
        ///     ,<ltype, nvarchar(50),>
        ///    ,<lDesc, text,>
        ///      ,<ldate, datetime,>
        ///      ,<remark, text,>)
        /// </summary>
        /// <param name="tmpTableName"></param>
        /// <param name="count"></param>
        public static void ilog(string tmpTableName, int count, string itype, string idesc, string iremark)
        {
            try
            {
                var tmpilogsql = "INSERT INTO [dbo].[iLog] ([lName],[ltype],[lDesc],[lvalue],[ldate],[remark]) VALUES (";
                tmpilogsql += "'" + tmpTableName + "','" + itype + "','" + idesc + "','" + count.ToString() + "',Getdate(),'" + iremark + "')";
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
        public static string getIp()
        {
            string ip = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection nics = mc.GetInstances();
            foreach (ManagementObject nic in nics)
            {
                if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                {
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
