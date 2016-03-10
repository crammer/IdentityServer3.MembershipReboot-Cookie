using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    public class Logs
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid ID
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Activity
        {
            get;
            set;
        }

        public string Detail
        {
            get;
            set;
        }

        public string PrivateIP
        {
            get;
            set;
        }

        public string PublicIP
        {
            get;
            set;
        }
    }
}

