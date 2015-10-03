using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursovoyHotel.model
{
    [Serializable]
    public class VisitorList : List<Visitor> 
    {
        public VisitorList() : base() { }

        // конструктор, принимающий IEnumerable<Visitor>
        public VisitorList(IEnumerable<Visitor> list) : base(list) {}

        //массив стран, гости из которых находятся в гостинице
        public string[] GetContries()
        {
            var res =
                from visitor in this
                select visitor.Country;
            return res.Distinct().ToArray();
        }

        //список городов по выбранной стране
        public string[] GetCities()
        {
            var res =
                from visitor in this
                select visitor.City;
            return res.Distinct().ToArray();
        }

        //выбор гостей по стране
        public VisitorList GetByCountry(string country)
        {
            var res =
                from visitor in this
                where visitor.Country == country
                select visitor;
            return new VisitorList(res);
        }
        
        //выбор гостей по городу
        public VisitorList GetByCity(string city)
        {
            var res =
                from visitor in this
                where visitor.City == city
                select visitor;
            return new VisitorList(res);
        }

        // выбор гостей по интервалу даты рождения
        public VisitorList GetByBirthday(DateTime date1, DateTime date2)
        {
            if (date1 > date2)
                throw new ArgumentException("Неправильно указан предполагаемый интервал, в который входит дата рождения.");
            var res =
                from visitor in this
                where visitor.Birthday >= date1 && visitor.Birthday <= date2
                select visitor;
            return new VisitorList(res);
        }

        // выбор гостей по интервалу, в который входит дата заезда
        public VisitorList GetByArrivalDate(DateTime date1, DateTime date2)
        {
            if (date1 > date2)
                throw new ArgumentException("Неправильно указан интервал, в который входит дата заезда.");
            var res =
                from visitor in this
                where visitor.ArrivalDate >= date1 && visitor.ArrivalDate <= date2
                select visitor;
            return new VisitorList(res);
        }

        // выбор гостей по интервалу, в который входит дата выезда
        public VisitorList GetByDepatureDate(DateTime date1, DateTime date2)
        {
            if (date1 > date2)
                throw new ArgumentException("Неправильно указан интервал, в который входит дата выезда.");
            var res =
                from visitor in this
                where visitor.DepatureDate >= date1 && visitor.DepatureDate <= date2
                select visitor;
            return new VisitorList(res);
        }

        //выбор по номеру
        public VisitorList GetByRoomNum(int num)
        {
            if (num < 1 || num > 36)
                throw new ArgumentException("Такого номера не существует.");
            var res =
                from visitor in this
                where visitor.Num == num
                select visitor;
            return new VisitorList(res);
        }


        // определяет входят ли все строки массива в имя(фамилию, отчество) гостя
        private bool IsInName(string[] words, Visitor visitor)
        {
            foreach (var word in words)
            {
                if (!(visitor.Name.ToLower().Contains(word) || 
                    visitor.Patronymic.ToLower().Contains(word) || 
                    visitor.Surname.ToLower().Contains(word)))
                    return false;
            }
            return true;
        }
        //выбор по части имени
        public VisitorList GetByNamePart(string str)
        {
            string[] words = str.ToLower().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var res =
                from visitor in this
                where IsInName(words, visitor)
                select visitor;
            return new VisitorList(res);
        }
    }
}
