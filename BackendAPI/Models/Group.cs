using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models
{
    public class Group
    {
        [Key] 
        public string Name { get; set; }

        public IList<Connection> Connections { get; set; } = new List<Connection>();
    }
}