using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursovoyHotel.model
{
    [Serializable]
    public class Visitor : IComparable<Visitor>
    {
        public int Num { set; get; }
        public string Surname { set; get; }
        public string Name { set; get; }
        public string Patronymic { set; get; }
        public string Country { set; get; }
        public string City { set; get; }
        public DateTime Birthday { set; get; }
        

        public DateTime ArrivalDate { set; get; }
        public DateTime DepatureDate { set; get; }

        public Visitor()
        {
            Birthday = DateTime.Today.AddYears(-35);
            ArrivalDate = DateTime.Today;
            DepatureDate = DateTime.Today.AddDays(7);
        }

        //для сортировки по фамилии
        public int CompareTo(Visitor obj)
        {
            return Surname.CompareTo(obj.Surname);
        }
    }
}
