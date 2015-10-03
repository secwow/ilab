using kursovoyHotel.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace kursovoyHotel
{
    public partial class RegistrationForm : Form
    {
        public VisitorList visitors = new VisitorList();
        MainForm mainf;
        int num;

        public RegistrationForm(int n, MainForm f)
        {
            num = n;
            InitializeComponent();
            mainf = f;
            this.Text += num;

            dateTimePicker3.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today.AddYears(-35);
            dateTimePicker1.Value = DateTime.Today.AddDays(7);

            if (mainf.roomList[num - 1].IsOccupied)
            {
                visitors = mainf.roomList[num - 1].CurrentVisitors;

                this.Text = "Изменнения даты отъезда из номера " + num;
                this.panel1.Enabled = false;
                this.dateTimePicker3.Enabled = false;
                this.button2.Text = "Перерасчёт и оформление дополнительной квитанции";
                foreach (var visitor in visitors)
                {
                    string s = visitor.Surname + " " + visitor.Name;
                    textBox1.AppendText(s + "\n");
                    comboBox1.Items.Add(s);
                }
            }
        }

        // Добавление гостя в спискок
        private void button1_Click(object sender, EventArgs e)
        {
            try 
            {
                Visitor visitor = new Visitor();

                visitor.Surname = textBox2.Text;
                visitor.Name = textBox3.Text;
                visitor.Patronymic = textBox6.Text;
                visitor.Country = textBox4.Text;
                visitor.City = textBox5.Text;
                visitor.Num = num;
                visitor.Birthday = dateTimePicker2.Value;

                if (visitor.Surname == "" || visitor.Name == "" || visitor.Country == "" || visitor.City == "")
                    throw new ArgumentException("Все поля (кроме отчества) должны быть заполнены.");

                visitors.Add(visitor);

                string s = visitor.Surname + " " + visitor.Name;
                textBox1.AppendText(s + "\n");
                comboBox1.Items.Add(s);

                textBox2.Clear();
                textBox3.Clear();
                textBox6.Clear();
            }
            catch (ArgumentException exc)
            {
                MessageBox.Show(exc.Message, "Регистрация гостя", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //регисрация и оформление квитанции
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                    throw new ArgumentException("Не указан плательщик.");

                if (dateTimePicker3.Value.Date >= dateTimePicker1.Value.Date ||
                        dateTimePicker1.Value.Date < DateTime.Today.Date)
                    throw new ArgumentException("Неверно указаны сроки пребывания в отеле.");

                Microsoft.Office.Interop.Excel.Application Excel = new Microsoft.Office.Interop.Excel.Application();
                Workbook wb = Excel.Workbooks.Add(XlSheetType.xlWorksheet);
                Worksheet ws = (Worksheet)Excel.ActiveSheet;
                Excel.Visible = true;

                ws.Cells[1, 1] = "Квитанция";
                ws.Cells[3, 1] = "Плательщик ";
                ws.Cells[3, 2] = comboBox1.Text;
                ws.Cells[4, 1] = "Дата заезда";
                ws.Cells[4, 2] = dateTimePicker3.Value;
                ws.Cells[5, 1] = "Дата выезда";
                ws.Cells[5, 2] = dateTimePicker1.Value;
                ws.Cells[6, 1] = "Класс номера";
                ws.Cells[6, 2] = mainf.label8.Text;
                ws.Cells[7, 1] = "количество мест";
                ws.Cells[7, 2] = mainf.label10.Text;
                ws.Cells[8, 1] = "Стоимость";
                ws.Cells[8, 2] = mainf.label6.Text;
                ws.Cells[8, 3] = "грн.";

                ws.Cells[10, 1] = "Сумма";
                ws.Cells[10, 2] = dateTimePicker1.Value.Subtract(dateTimePicker3.Value).Days * Convert.ToUInt32(mainf.label6.Text);
                ws.Cells[10, 3] = "грн.";

                ws.Cells[13, 1] = "Подпись плательщика";
                ws.Cells[13, 3] = "Подпись сотрудника";

                //описание случая перерасчёта (возврат или доплата)
                if (mainf.roomList[num - 1].IsOccupied)
                {
                    DateTime oldDate = mainf.roomList[num - 1].CurrentVisitors[0].DepatureDate;

                    ws.Cells[4, 1] = "Прошлая дата выезда";
                    ws.Cells[4, 2] = oldDate;
                    ws.Cells[5, 1] = "Новая дата выезда";

                    if (oldDate.Date == dateTimePicker1.Value.Date)
                        throw new ArgumentException("Указана прежняя дата.");
                    else if (dateTimePicker1.Value > oldDate)
                    {
                        ws.Cells[10, 1] = "Сумма доплаты";
                        ws.Cells[10, 2] = dateTimePicker1.Value.Subtract(oldDate).Days * Convert.ToUInt32(mainf.label6.Text);
                    }
                    else 
                    {
                        ws.Cells[10, 1] = "Сумма возврата";
                        ws.Cells[10, 2] = oldDate.Subtract(dateTimePicker1.Value).Days * Convert.ToUInt32(mainf.label6.Text);
                    }
                }


                foreach (var visitor in visitors)
                {
                    visitor.ArrivalDate = dateTimePicker3.Value;
                    visitor.DepatureDate = dateTimePicker1.Value;
                }


                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (ArgumentException exc)
            {
                MessageBox.Show(exc.Message, "Оформление гостя", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
