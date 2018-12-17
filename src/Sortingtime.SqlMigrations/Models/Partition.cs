using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sortingtime.Models
{
    public class Partition
    {
        public long Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }

        [Required]
        public long CreatedByUserId { get; set; }

        [Required]
        public Plans Plan { get; set; }

        [Required]
        public short MaxUsers { get; set; }

        [Required]
        public short MaxReportsPerMonth { get; set; }

        [Required]
        public short MaxInvoicesPerMonth { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public virtual ICollection<ReportSetting> ReportSettings { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }

        public virtual ICollection<InvoiceSetting> InvoiceSettings { get; set; }

        public virtual ICollection<HourPriceSetting> HourPriceSettings { get; set; }

        public virtual Organization Organization { get; set; }

        public static Partition CreateNew(Plans plan)
        {
            var partition = new Partition
            {
                Timestamp = DateTime.UtcNow,
                Plan = plan,                
            };

            switch (plan)
            {
                case Plans.Demo:
                    partition.MaxUsers = 2;
                    partition.MaxReportsPerMonth = 10;
                    partition.MaxInvoicesPerMonth = 10;
                    break;
                case Plans.Free:
                    partition.MaxUsers = 5;
                    partition.MaxReportsPerMonth = 50;
                    partition.MaxInvoicesPerMonth = 50;
                    break;
                default:
                    throw new NotImplementedException($"Plan type '{plan}' not implemented.");
            }

            return partition;
        }

    }
}