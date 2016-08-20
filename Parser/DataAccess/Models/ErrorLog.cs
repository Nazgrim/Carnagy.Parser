using System;

namespace DataAccess.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public int MainConfigurationId { get; set; }

        public virtual MainConfiguration MainConfiguration { get; set; }
    }
}
