using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdateData
{
    public class BssLineStock
    {

        #region 变量定义

        #endregion

        #region 检索数据

        /// <summary>
        /// 获取AIS配膳リストピッキング実績
        /// </summary>
        /// <returns></returns>
        public DataTable GetPickedActData()
        {
            SqlDataReader dataReader = null;
            DataTable tblPicked = null;

            try
            { 
                String SqlString = string.Format("SELECT * FROM N_AIS_SHOP_LIST_PICKED_ACT_TAB ");
                dataReader = DbHelperSQL.ExecuteReader(SqlString);
               
                tblPicked = ConvertDataReaderToDataTable(dataReader);
            }
            catch (Exception)
            { 
                throw;
            }

            return tblPicked;
        }


       /// <summary>
        /// 配膳データ 
       /// </summary>
       /// <param name="shopListId"></param>
       /// <param name="libPartsStock"></param>
       /// <param name="revision_no"></param>
       /// <returns></returns>
        public DataTable GetHaizenDtData(string shopListId, Lib_PARTS_STOCK libPartsStock, string revision_no)
        {
            SqlDataReader dataReader = null;
            DataTable tblHaizenDt = null;

            try
            {
                String SqlString = string.Format("SELECT * FROM NAIS_F_HAIZEN_DT "
                                            + "WHERE N_SHOP_LIST_ID='{0}' AND COMPONENT_PART='{1}' AND CONTRACT='{2}' AND PRODUCTION_LINE='{3}' AND ENG_CHG_LEVEL='{4}' "
                                            , shopListId, libPartsStock.COMPONENT_PART, libPartsStock.CONTRACT, libPartsStock.WORK_CENTER_NO, revision_no);

                dataReader = DbHelperSQL.ExecuteReader(SqlString);

                tblHaizenDt = ConvertDataReaderToDataTable(dataReader);
            }
            catch (Exception)
            {
                throw;
            }

            return tblHaizenDt;
        }
 
       /// <summary>
        /// 获取部品配列情報を持つテーブル
        /// 数据主键是：キット + 部品番号 + ｻｲﾄ + ライン+ 位置 + Z軸
       /// </summary>
       /// <param name="kitNo"></param>
       /// <param name="libPartsStock"></param>
       /// <param name="revision_no"></param>
       /// <returns></returns>
        public string GetPartListData(string kitNo, Lib_PARTS_STOCK libPartsStock, string revision_no)
        {

            string array_grp_id ="";
            object grp_id;

            try
            {
                String SqlString = string.Format("SELECT ARRAY_GRP_ID FROM M_PARTS_LIST_DD "
                                            + "WHERE PART_NO='{0}' AND COMPONENT_PART='{1}' AND CONTRACT='{2}' AND WORK_CENTER_NO='{3}' "
                                            + " AND POSITION='{4}' AND FEEDER='{5}' AND REVISION_NO='{6}' "
                                            , kitNo, libPartsStock.COMPONENT_PART, libPartsStock.CONTRACT, libPartsStock.WORK_CENTER_NO, libPartsStock.POSITION, libPartsStock.FEEDER, revision_no);

                grp_id = DbHelperSQL.GetSingle(SqlString);

                if (grp_id != null) array_grp_id = grp_id.ToString(); 
            }
            catch (Exception)
            {
                throw;
            }

            return array_grp_id;
        }

        /// <summary>
        /// 部品在庫マスタ
        /// 数据主键是：部品番号 + ｻｲﾄ + ライン+ 位置 + Z軸 + Z軸ID + 部材ロット№
        /// </summary>
        /// <param name="libPartsStock"></param>
        /// <returns></returns>
        public DataTable GetStockData(Lib_PARTS_STOCK libPartsStock)
        {
            SqlDataReader dataReader = null;
            DataTable tblLineStock = null;

            try
            {
                String SqlString = string.Format("SELECT * FROM M_PARTS_STOCK "
                                            + "WHERE COMPONENT_PART='{0}' AND CONTRACT='{1}' AND WORK_CENTER_NO='{2}' AND POSITION='{3}' AND FEEDER='{4}' AND ARRAY_GRP_ID='{5}' AND N_MATERIAL_LOT='{6}' "
                                            , libPartsStock.COMPONENT_PART, libPartsStock.CONTRACT, libPartsStock.WORK_CENTER_NO, libPartsStock.POSITION, libPartsStock.FEEDER, libPartsStock.ARRAY_GRP_ID, libPartsStock.N_MATERIAL_LOT);

                dataReader = DbHelperSQL.ExecuteReader(SqlString);

                tblLineStock = ConvertDataReaderToDataTable(dataReader);
            }
            catch (Exception)
            {
                throw;
            }

            return tblLineStock;
        }
  
       /// <summary>
        /// ライン毎の在庫情報
        /// 数据主键是：部品番号 + ｻｲﾄ + ライン+ 位置 + Z軸 + Z軸ID
       /// </summary>
       /// <param name="libLinePartsStock"></param>
       /// <returns></returns>
        public DataTable GetLineStockData(Lib_LINE_PARTS_STOCK libLinePartsStock)
        {
            SqlDataReader dataReader = null;
            DataTable tblLineStock = null;

            try
            {
                String SqlString = string.Format("SELECT * FROM M_LINE_PARTS_STOCK "
                                            + "WHERE COMPONENT_PART='{0}' AND CONTRACT='{1}' AND WORK_CENTER_NO='{2}' AND POSITION='{3}' AND FEEDER='{4}' AND ARRAY_GRP_ID='{5}' "
                                            , libLinePartsStock.COMPONENT_PART, libLinePartsStock.CONTRACT, libLinePartsStock.WORK_CENTER_NO, libLinePartsStock.POSITION, libLinePartsStock.FEEDER, libLinePartsStock.ARRAY_GRP_ID);

                dataReader = DbHelperSQL.ExecuteReader(SqlString);

                tblLineStock = ConvertDataReaderToDataTable(dataReader);
            }
            catch (Exception)
            {
                throw;
            }

            return tblLineStock;
        }

        #endregion
 
        #region 修改数据

        /// <summary>
        /// 更新ライン在庫マス数据
        /// </summary>
        /// <param name="tblPicked"></param>
        /// <returns></returns>
        public int SetLinePartsStockData(DataTable tblPicked)
        {

            int returnValue = -1;
            String SqlString;

            string shopListId;
            string kitNo = ""; 
            string revision_no;
             
            Decimal decPickedQty;

            DataTable tblHaizenDt = null;
            DataTable tblLinStock = null;
            string valKey;

            //ライン在庫マスタ
            Dictionary<string, Lib_LINE_PARTS_STOCK> dicLinePartsStock = new Dictionary<string,Lib_LINE_PARTS_STOCK>();
            //部品在庫マスタ
            List<Lib_PARTS_STOCK> lstPartsStock = new List<Lib_PARTS_STOCK>();
 
            Lib_LINE_PARTS_STOCK libLinePartsStock;
            Lib_PARTS_STOCK libPartsStock;
            string N_IF_SEQ;

            try
            {

                if (tblPicked != null && tblPicked.Rows.Count > 0)
                {
                    for (int i = 0; i < tblPicked.Rows.Count; i++)
                    {

                        libPartsStock = new Lib_PARTS_STOCK();

                        //買物配膳ID
                        shopListId = tblPicked.Rows[i]["N_SHOP_LIST_ID"].ToString();

                        //①部品番号
                        // ---> AIS配膳リストピッキング実績の品目番号
                        libPartsStock.COMPONENT_PART = tblPicked.Rows[i]["PART_NO"].ToString();
                        //リビジョン 
                        revision_no = tblPicked.Rows[i]["ENG_CHG_LEVEL"].ToString();
                        //ｻｲﾄ
                        libPartsStock.CONTRACT = tblPicked.Rows[i]["CONTRACT"].ToString();
                        //生産ライン
                        libPartsStock.WORK_CENTER_NO = tblPicked.Rows[i]["WORK_CENTER_NO"].ToString();

                        //②ﾌｨｰﾀﾞｰ№
                        // ---> 配膳データより,( AIS配膳リストピッキング実績の買物配膳IDで参照)
                        tblHaizenDt = GetHaizenDtData(shopListId, libPartsStock, revision_no);

                        if (tblHaizenDt != null && tblHaizenDt.Rows.Count > 0)
                        {

                            //品目番号
                            kitNo = tblHaizenDt.Rows[0]["PART_NO"].ToString();
                            //Z軸
                            libPartsStock.FEEDER = tblHaizenDt.Rows[0]["N_Z_AXIS"].ToString();
                            //位置
                            libPartsStock.POSITION = tblHaizenDt.Rows[0]["N_POSITION"].ToString();

                            //③Z軸ID
                            // ---> 部品配列データより,( AIS配膳リストピッキング実績、配膳データの項目値で参照),参照配膳数据
                            libPartsStock.ARRAY_GRP_ID = GetPartListData(kitNo, libPartsStock, revision_no);
                        }

                        //ロットバッチ番号
                        libPartsStock.N_MATERIAL_LOT = tblPicked.Rows[i]["LOT_BATCH_NO"].ToString();

                        //④部材数量
                        // --->  AIS配膳リストピッキング実績のピッキング数量
                        Decimal.TryParse(tblPicked.Rows[i]["N_QTY_PICKED"].ToString(), out decPickedQty);
                        //在庫数
                        libPartsStock.N_ACTUAL_USED = decPickedQty;
                        //⑤更新担当---> 設定無しで可
                        libPartsStock.UPD_OPERATOR = "";
                        //⑥更新端末---> PGが動作するPC名 
                        libPartsStock.UPD_PC = System.Net.Dns.GetHostName();

                        //部品在庫マスタ
                        lstPartsStock.Add(libPartsStock);
                    }
                }
               
                libLinePartsStock = new Lib_LINE_PARTS_STOCK();
                
                //数据解析处理
                for (int i = 0; i < lstPartsStock.Count; i++)
                {
                    valKey = lstPartsStock[i].COMPONENT_PART + lstPartsStock[i].CONTRACT + lstPartsStock[i].WORK_CENTER_NO + lstPartsStock[i].POSITION + lstPartsStock[i].FEEDER + lstPartsStock[i].ARRAY_GRP_ID;

                    if (!dicLinePartsStock.ContainsKey(valKey))
                    {
                        libLinePartsStock = new Lib_LINE_PARTS_STOCK();
                        libLinePartsStock.CONTRACT = lstPartsStock[i].CONTRACT;
                        libLinePartsStock.WORK_CENTER_NO = lstPartsStock[i].WORK_CENTER_NO;
                        libLinePartsStock.POSITION = lstPartsStock[i].POSITION;
                        libLinePartsStock.FEEDER = lstPartsStock[i].FEEDER;
                        libLinePartsStock.COMPONENT_PART = lstPartsStock[i].COMPONENT_PART;

                        libLinePartsStock.RESERVE_QTY = lstPartsStock[i].RESERVE_QTY;
                        libLinePartsStock.FREE_QTY = lstPartsStock[i].FREE_QTY;
                        libLinePartsStock.ARRAY_GRP_ID = lstPartsStock[i].ARRAY_GRP_ID;
                        libLinePartsStock.UPD_OPERATOR = lstPartsStock[i].UPD_OPERATOR;
                        libLinePartsStock.UPD_PC = lstPartsStock[i].UPD_PC;

                        dicLinePartsStock[valKey] = libLinePartsStock;
                    }

                    libLinePartsStock = dicLinePartsStock[valKey];
                    //部材数量
                    libLinePartsStock.STOCK_QTY += lstPartsStock[i].N_ACTUAL_USED;
                }




                //部品在庫マスタ
                for (int i = 0; i < lstPartsStock.Count; i++)
                {
                    libPartsStock = lstPartsStock[i];
                    //部品在庫マスタ
                    tblLinStock = GetStockData(libPartsStock);

                    if (tblLinStock != null && tblLinStock.Rows.Count > 0)
                    {
                        N_IF_SEQ = tblLinStock.Rows[0]["N_IF_SEQ"].ToString();

                        //⑦在庫数:レコードが存在している場合は、内容値にピッキング数量加算 
                        //⑧引当数:レコードが存在している場合は、内容値そのまま 
                        //⑨引当可能数:在庫数から引当数を減算,引当可能数=在库数-引当数
                        SqlString = string.Format("UPDATE M_PARTS_STOCK SET N_ACTUAL_USED = N_ACTUAL_USED + {0}, FREE_QTY = N_ACTUAL_USED - RESERVE_QTY + {0} WHERE N_IF_SEQ = '{1}'  ", libPartsStock.N_ACTUAL_USED, N_IF_SEQ);
                    }
                    else
                    {
                        N_IF_SEQ = DbHelperSQL.GetMaxID("N_IF_SEQ", "M_PARTS_STOCK").ToString();
                        //⑦在庫数:レコードが存在してない場合は、ピッキング数量
                        //⑧引当数:レコードが存在してない場合は、ZERO
                        //⑨引当可能数:在庫数から引当数を減算,引当可能数=在库数-引当数
                        SqlString = string.Format("INSERT INTO M_PARTS_STOCK "
                                                + "(N_IF_SEQ,COMPONENT_PART,N_MATERIAL_LOT,N_MATERIAL_TRACE_SEQ,N_ACTUAL_USED,CONTRACT"
                                                +   "       ,WORK_CENTER_NO,POSITION,FEEDER,ARRAY_GRP_ID,RESERVE_QTY,FREE_QTY,UPD_OPERATOR,UPD_PC) "
                                                + " VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')"
                                                , N_IF_SEQ, libPartsStock.COMPONENT_PART, libPartsStock.N_MATERIAL_LOT, libPartsStock.N_MATERIAL_TRACE_SEQ, libPartsStock.N_ACTUAL_USED, libPartsStock.CONTRACT
                                                , libPartsStock.WORK_CENTER_NO, libPartsStock.POSITION, libPartsStock.FEEDER, libPartsStock.ARRAY_GRP_ID, libPartsStock.RESERVE_QTY, libPartsStock.N_ACTUAL_USED, libPartsStock.UPD_OPERATOR, libPartsStock.UPD_PC);

                    }

                    DbHelperSQL.ExecuteSql(SqlString);
                }


                //ライン在庫マスタ
                foreach (string key in dicLinePartsStock.Keys)
                {
                    libLinePartsStock = dicLinePartsStock[key];
                    //ライン在庫マスタ
                    tblLinStock = GetLineStockData(libLinePartsStock);

                    if (tblLinStock != null && tblLinStock.Rows.Count > 0)
                    {
                        N_IF_SEQ = tblLinStock.Rows[0]["N_IF_SEQ"].ToString();

                        //⑦在庫数:レコードが存在している場合は、内容値にピッキング数量加算 
                        //⑧引当数:レコードが存在している場合は、内容値そのまま 
                        //⑨引当可能数:在庫数から引当数を減算,引当可能数=在库数-引当数
                        SqlString = string.Format("UPDATE M_LINE_PARTS_STOCK SET STOCK_QTY = STOCK_QTY + {0}, FREE_QTY = STOCK_QTY - RESERVE_QTY + {0} WHERE N_IF_SEQ = '{1}'  ", libLinePartsStock.STOCK_QTY, N_IF_SEQ);
                    }
                    else
                    {
                        N_IF_SEQ = DbHelperSQL.GetMaxID("N_IF_SEQ", "M_LINE_PARTS_STOCK").ToString();
                        //⑦在庫数:レコードが存在してない場合は、ピッキング数量
                        //⑧引当数:レコードが存在してない場合は、ZERO
                        //⑨引当可能数:在庫数から引当数を減算,引当可能数=在库数-引当数
                        SqlString = string.Format("INSERT INTO M_LINE_PARTS_STOCK "
                                                + "(N_IF_SEQ,CONTRACT,WORK_CENTER_NO,POSITION,FEEDER,COMPONENT_PART"
                                                + "       ,STOCK_QTY,RESERVE_QTY,FREE_QTY,ARRAY_GRP_ID,UPD_OPERATOR,UPD_PC) "
                                                + " VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')"
                                                , N_IF_SEQ, libLinePartsStock.CONTRACT, libLinePartsStock.WORK_CENTER_NO, libLinePartsStock.POSITION, libLinePartsStock.FEEDER, libLinePartsStock.COMPONENT_PART
                                                , libLinePartsStock.STOCK_QTY, libLinePartsStock.RESERVE_QTY, libLinePartsStock.STOCK_QTY, libLinePartsStock.ARRAY_GRP_ID, libLinePartsStock.UPD_OPERATOR, libLinePartsStock.UPD_PC);

                    }

                    DbHelperSQL.ExecuteSql(SqlString);

                }
 
                returnValue = 1;
            }
            catch (Exception)
            {
                throw;
            }

            return returnValue;
        }

        #endregion 

 
        /// <summary>
        /// 数据表转换
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static DataTable ConvertDataReaderToDataTable(SqlDataReader dataReader)
        {
            DataTable datatable = new DataTable("DataTable");
            DataTable schemaTable = dataReader.GetSchemaTable();

            //动态添加列

            try
            {
                if (dataReader != null)
                {

                    foreach (DataRow myRow in schemaTable.Rows)
                    {
                        DataColumn myDataColumn = new DataColumn();
                        myDataColumn.DataType = myRow["DataTypeName"].GetType();
                        myDataColumn.ColumnName = myRow[0].ToString();
                        datatable.Columns.Add(myDataColumn);
                    }


                    //添加数据
                    while (dataReader.Read())
                    {
                        DataRow myDataRow = datatable.NewRow();
                        for (int i = 0; i < schemaTable.Rows.Count; i++)
                        {
                            myDataRow[i] = dataReader[i].ToString();
                        }
                        datatable.Rows.Add(myDataRow);
                        myDataRow = null;
                    }
                    schemaTable = null;
                    dataReader.Close();

                }

                return datatable;
            }
            catch (Exception ex)
            {
                throw new Exception("转换出错出错!", ex);
            }

        }
    }
}
