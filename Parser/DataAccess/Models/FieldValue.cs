namespace DataAccess.Models
{
    public class FieldValue
    {
        public int FieldId { get; set; }
        public int ParssedCarId { get; set; }
        public string Value { get; set; }

        public virtual Field Field { get; set; }
        public virtual ParssedCar ParssedCar { get; set; }
    }
}
