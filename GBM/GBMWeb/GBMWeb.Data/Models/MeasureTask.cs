using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBMWeb.Data.Models
{
    [Table("MeasureTask")]
    public class MeasureTask
    {
        [Column("Id"), Key]
        public string Id { get; set; }
        
        [Column("Status")]
        public int Status { get; set; }
        
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        
        [Column("InputImageFilePath")]
        public string InputImageFilePath { get; set; }
        
        [Column("InputImageMime")]
        public string InputImageMime { get; set; }
        
        [Column("OutputImageFilePath")]
        public string OutputImageFilePath { get; set; }
        
        [Column("OutputImageMime")]
        public string OutputImageMime { get; set; }
        
        [Column("OutputMetrics")]
        public string OutputMetrics { get; set; }
    }
}
