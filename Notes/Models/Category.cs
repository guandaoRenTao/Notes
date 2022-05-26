using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Notes.Models
{
    [Table("Categories")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public DateTime Date { get; set; }
        [OneToMany]
        public List<Note> Notes { get; set; }
    }
}
