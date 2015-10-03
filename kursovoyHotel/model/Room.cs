using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursovoyHotel.model
{
    [Serializable]
    public class Room : IComparable<Room>
    {
        public enum Kind { Standart, JuniorSuite, Suite };

        public int Num;
        public Kind RoomKind;
        public bool IsOccupied;
        public VisitorList CurrentVisitors;
        public int NumOfBed;
        public double Price;

        public Room()
        {
            CurrentVisitors = new VisitorList();
        }

        //для сортировки по номеру
        public int CompareTo(Room obj)
        {
            return Num.CompareTo(obj.Num);
        }
    }
}
