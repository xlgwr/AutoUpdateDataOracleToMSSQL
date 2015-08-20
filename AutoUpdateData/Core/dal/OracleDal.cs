﻿using System;
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
                var sbvalue = new StringBuilder();

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
                        return sb.Append(sbvalue.ToString()).ToString();

                    }
                }
            }
            return null;

        }
    }
}
