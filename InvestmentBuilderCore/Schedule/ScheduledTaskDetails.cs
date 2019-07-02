using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InvestmentBuilderCore.Schedule
{
    /// <summary>
    /// Interface for a scheduled task. A task is run from the scheduler.
    /// </summary>
    public interface IScheduledTask
    {
        string Name { get; }
        void RunJob();
    }

    /// <summary>
    /// An individual scheduled task
    /// </summary>
    [XmlType("task")]
    public class ScheduledTaskDetails
    {
        /// <summary>
        /// Unique name for scheduled task.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Schedule time for task (once a day).
        /// </summary>
        [XmlElement("scheduled")]
        public DateTime ScheduledTime { get; set; }

    }
}
