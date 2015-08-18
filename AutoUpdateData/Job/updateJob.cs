using log4net;
using Quartz;
using System;
using System.Linq;
using System.Reflection;

namespace AutoUpdateData.Service.Job
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ClearJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(Quartz.IJobExecutionContext context)
        {
            logger.Debug("执行清理任务!!!!!!!!!!!!!!!");
            
        }
    }
}