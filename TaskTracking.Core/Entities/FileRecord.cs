using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking.Core.Entities
{
    public class FileRecord
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public int TaskId { get; set; }
        public TaskItem Task { get; set; }
    }
}
