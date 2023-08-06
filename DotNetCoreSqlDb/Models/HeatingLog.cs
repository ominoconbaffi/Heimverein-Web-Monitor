using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DotNetCoreSqlDb.Models
{
    public class HeatingLog
    {
        public int ID { get; set; }
        public string? Comment { get; set; }

        [DisplayName("Created Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy.MM.dd - hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }
    }
}
