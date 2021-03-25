using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserManagement.Data
{
    public class Group
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public List<User> Users { get; set; } = new List<User>();

        [JsonIgnore]
        public List<Group> ChildGroups { get; set; } = new List<Group>();

        [JsonIgnore]
        public Group? ParentGroup { get; set; }

        [JsonIgnore]
        public int? ParentGroupId { get; set; }
    }
}