using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using AutoUpdateData.Core.enity;

namespace AutoUpdateData.Core.dal
{
    public class OracleDal
    {
        //var tmpsrt = "user id=nec;password=nec;data source=xe";
        //   OracleConnection con = new OracleConnection(tmpsrt);
        //   con.Open();
        //   MessageBox.Show(con.ServerVersion);
        //   con.Close();
        //   con.Dispose();

        public static DataSet GetTableColumns(string tablename)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select t.COLUMN_NAME,t.DATA_TYPE,t.DATA_LENGTH,t.NULLABLE,t.COLUMN_ID from user_tab_columns t,user_col_comments c where t.table_name = c.table_name and t.column_name = c.column_name");
            sb.Append("  and t.table_name = '" + tablename + "'");

            var result = DbHelperOra.Query(sb.ToString());
            return result;
        }
        /// <summary>
        /// top 10
        /// select * from table where rownum<=10;
        /// more page
        /// SELECT * FROM ( SELECT A.*, ROWNUM RN 
        //  FROM (SELECT * FROM TABLE_NAME) A WHERE ROWNUM <= 40 ) WHERE RN >= 21
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="tmpwhere"></param>
        /// <param name="rownum"></param>
        /// <returns></returns>
        public static DataSet GetData(string tablename, string tmpwhere, int rownum)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from " + tablename);
            if (!string.IsNullOrEmpty(tmpwhere))
            {
                sb.Append(" where " + tmpwhere);
                sb.Append(" and rownum<= " + rownum.ToString());
            }
            else
            {

                sb.Append(" where  rownum<= " + rownum.ToString());
            }

            var result = DbHelperOra.Query(sb.ToString());
            return result;
        }
        public static DataSet GetData(string tablename, string tmpwhere, int rownum, params OracleParameter[] cmdParms)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from " + tablename);
            if (!string.IsNullOrEmpty(tmpwhere))
            {
                sb.Append(" where " + tmpwhere);
                sb.Append(" and rownum<= " + rownum.ToString());
            }
            else
            {

                sb.Append(" where  rownum<= " + rownum.ToString());
            }

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
    }
}
