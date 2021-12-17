using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models
{
    public class Group
    {
        public Group()
        {
        }

        public Group(string name)
        {
            Name = name;
        }

        [Key] 
        public string Name { get; set; }

        public IList<Connection> Connections { get; set; } = new List<Connection>();
    }
}