using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpdateData
{

  #region 属性设置

    /// <summary>
    /// ライン部品在庫マスタ
    /// </summary>
    public class Lib_LINE_PARTS_STOCK
    {

        /// <summary>
        /// 全体SEQ	
        /// </summary>
        public string N_IF_SEQ
        {
            get;
            set;
        }
        /// <summary>
        /// ｻｲﾄ
        /// </summary>
        public string CONTRACT
        {
            get;
            set;
        }
        /// <summary>
        /// ライン
        /// </summary>
        public string WORK_CENTER_NO
        {
            get;
            set;
        }
        /// <summary>
        /// 位置
        /// </summary>
        public string POSITION
        {
            get;
            set;
        }
        /// <summary>
        /// Z軸
        /// </summary>
        public string FEEDER
        {
            get;
            set;
        }
        /// <summary>
        /// 部品番号
        /// </summary>
        public string COMPONENT_PART
        {
            get;
            set;
        }
  
        /// <summary>
        /// 部材数量
        /// </summary>
        public decimal STOCK_QTY
        {
            get;
            set;
        }

        /// <summary>
        /// 引当数
        /// </summary>
        public decimal RESERVE_QTY
        {
            get;
            set;
        }

        /// <summary>
        /// 引当可能数
        /// </summary>
        public decimal FREE_QTY
        {
            get;
            set;
        } 

        /// <summary>
        /// Z軸ID
        /// </summary>
        public string ARRAY_GRP_ID
        {
            get;
            set;
        }
  
        /// <summary>
        /// 更新担当
        /// </summary>
        public string UPD_OPERATOR
        {
            get;
            set;
        }
  
        /// <summary>
        /// 更新端末
        /// </summary>
        public string UPD_PC
        {
            get;
            set;
        }
     
    }


  #endregion

  #region 共通处理方法


  #endregion
}
