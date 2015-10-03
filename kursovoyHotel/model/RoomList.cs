using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursovoyHotel.model
{
    [Serializable]
    public class RoomList : List<Room>
    {
        //создает список номеров по умолчанию (в дальнейшем следует вынести в настройки
        //программы)
        public RoomList()
        {
            for (int i = 1; i <= 36; i++)
            {
                Room room = new Room();
                room.Num = i;

                if (i <= 12)
                {
                    room.RoomKind = Room.Kind.Standart;
                    room.Price = 100;
                }
                else if (i >= 25)
                {
                    room.RoomKind = Room.Kind.Suite;
                    room.Price = 200;
                }
                else
                {
                    room.RoomKind = Room.Kind.JuniorSuite;
                    room.Price = 150;
                }

                if (i % 12 <= 3 && i % 12 >= 1)
                    room.NumOfBed = 1;
                else if (i % 12 <= 6 && i % 12 >= 4)
                {
                    room.NumOfBed = 2;
                    room.Price += room.Price / 2;
                }
                else if (i % 12 <= 9 && i % 12 >= 7)
                {
                    room.NumOfBed = 3;
                    room.Price = 2 * room.Price;
                }
                else if (i % 12 != 0)
                {
                    room.NumOfBed = 4;
                    room.Price = 2.5 * room.Price;
                }
                else
                {
                    room.NumOfBed = 5;
                    room.Price = 3 * room.Price;
                }
                this.Add(room);
            }
        }

        //список комнат, из которых выезжают сегодня
        public List<int> TodayDepature()
        {
            var result = 
                from room in this
                where room.CurrentVisitors.Count != 0 && 
                room.CurrentVisitors[0].DepatureDate.Equals(DateTime.Today)
                select room.Num;
            return new List<int>(result);
        }

        //осуществляет подбор комнат
        public List<int> FindRooms(int priceMin, int priceMax, int bedNum, DateTime date1, DateTime date2)
        {
            var result =
                from room in this
                where (! room.IsOccupied) && (bedNum == 0 || room.NumOfBed == bedNum) && 
                room.Price >= priceMin && room.Price <= priceMax && 
                ( ! room.IsOccupied || room.CurrentVisitors[0].DepatureDate < date1)
                select room.Num;
            return new List<int>(result);
        }
        
        //поселение в номер
        public void SetInRoom(VisitorList visitors, int num)
        {
            this[num - 1].IsOccupied = true;
            this[num - 1].CurrentVisitors = visitors;
        }

        //удаление просроченных, в реальной йпрограмме эта функция не нужна
        public void DeleteDelayed()
        {
            foreach (Room room in this)
            {
                if (room.IsOccupied && 
                    room.CurrentVisitors[0].DepatureDate < DateTime.Today)
                {
                    this.ClearRoom(room.Num);
                }
            }
        }

        // освобождение комнаты
        public void ClearRoom(int num)
        {
            this[num - 1].IsOccupied = false;
            this[num - 1].CurrentVisitors.Clear();
        }

        //возвращает VisitorList (список всех постояльцев)
        public VisitorList GetVisitorList()
        {
            VisitorList result = new VisitorList();
            foreach (var room in this)
                result.AddRange(room.CurrentVisitors);
            return result; 
        }
    }
}
