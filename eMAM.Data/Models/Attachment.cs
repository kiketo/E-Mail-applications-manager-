using System.ComponentModel.DataAnnotations.Schema;

namespace eMAM.Data.Models
{
    public class Attachment
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public double FileSizeInMb { get; set; }

      
        public int EmailId { get; set; }

        public Email Email { get; set; }
    }
}
