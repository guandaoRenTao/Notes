using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Notes.Models
{
    [Table("Notes")]
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        
        [ForeignKey(typeof(Category))]
        public int CategoryID { get; set; }
    }
}
