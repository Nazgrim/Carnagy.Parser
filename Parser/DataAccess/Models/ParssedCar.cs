using System.Collections.Generic;

namespace DataAccess.Models
{
    public class ParssedCar
    {
        public ParssedCar()
        {
            FieldValues = new List<FieldValue>();
        }
        public int Id { get; set; }

        public virtual ICollection<FieldValue> FieldValues { get; set; }
    }
}
