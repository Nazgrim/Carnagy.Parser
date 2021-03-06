﻿using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Field
    {
        public Field()
        {
            FieldValues = new List<FieldValue>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Xpath { get; set; }
        public string Attribute { get; set; }
        public bool IsDefault { get; set; }
        public string RegExPattern { get; set; }
        public FieldConfigurationType ConfigurationType { get; set; }

        public int MainConfigurationId { get; set; }

        public virtual MainConfiguration MainConfiguration { get; set; }
        public virtual ICollection<FieldValue> FieldValues { get; set; }
    }
}
