using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdateData.Core.enity
{
    public class column
    {
        public string COLUMN_NAME { get; set; }

        public string DATA_TYPE { get; set; }

        public int DATA_LENGTH { get; set; }

        public string NULLABLE { get; set; }

        public int COLUMN_ID { get; set; }
    }
}
