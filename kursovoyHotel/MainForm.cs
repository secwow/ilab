using kursovoyHotel.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kursovoyHotel
{
    public partial class MainForm : Form
    {
        public RoomList roomList = new RoomList();
        //public static VisitorList visitorList = new VisitorList();
        public int CELLS_NUM = 6;

        public MainForm()
        {
            InitializeComponent();

            сохранитьToolStripMenuItem.Enabled = false;
            
        }

        //заполнение таблицы с номерами (можно перенести в настройки)
        public void FillTable()
        {
            dataGridView1.RowCount = CELLS_NUM;
            dataGridView1.ColumnCount = CELLS_NUM;

            foreach (var room in roomList)
            {
                DataGridViewCell cell = dataGridView1.Rows[(room.Num - 1) / CELLS_NUM].Cells[(room.Num - 1) % CELLS_NUM];
                cell.Value = room.Num;

                int i = room.Num;
                if (i <= CELLS_NUM * 2)
                    cell.Style.BackColor = Color.LightGreen;
                else if (i >= CELLS_NUM * 4 + 1)
                    cell.Style.BackColor = Color.LightCoral;
                else
                    cell.Style.BackColor = Color.LightSteelBlue;

                if (room.IsOccupied)
                    cell.Style.BackColor = Color.FromArgb(cell.Style.BackColor.A, cell.Style.BackColor.R - 40, cell.Style.BackColor.G - 40, cell.Style.BackColor.B - 40);
            }
            RefreshInfo();
        }

        //изменение информации о номере
        private void dataGridView1_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            RefreshInfo();
        }

        //обновляет информацию о выбранном номере
        public void RefreshInfo()
        {
            if (dataGridView1.SelectedCells.Count == 0 || dataGridView1.SelectedCells[0].Value == null)
                return;
            int num = Convert.ToInt32(dataGridView1.SelectedCells[0].Value);
            Room rom = roomList[num - 1];
            label2.Text = Convert.ToString(rom.Num);

            

            if (rom.IsOccupied) 
            {
                label4.Text = "Занят";

                button1.Enabled = false;
                button4.Enabled = false;
                if (rom.CurrentVisitors[0].DepatureDate == DateTime.Today)
                    button4.Enabled = true;
                button5.Enabled = true;
                
                label19.Text = rom.CurrentVisitors[0].ArrivalDate.Date.ToString("d") + " -- " + rom.CurrentVisitors[0].DepatureDate.Date.ToString("d");
            }
            else
            {
                label4.Text = "Свободен";
                button1.Enabled = true;
                button4.Enabled = false;
                button5.Enabled = false;
                label19.Text = "";
            }

            label6.Text = Convert.ToString(rom.Price);

            if (rom.RoomKind == Room.Kind.Standart)
                label8.Text = "Стандарт";
            else if (rom.RoomKind == Room.Kind.JuniorSuite)
                label8.Text = "Полулюкс";
            else
                label8.Text = "Люкс";

            label10.Text = Convert.ToString(rom.NumOfBed);
            textBox3.Text = "";
            if (rom.CurrentVisitors.Count != 0)
            {
                foreach (Visitor visitor in rom.CurrentVisitors)
                {
                    string s = visitor.Surname + " " + visitor.Name;
                    textBox3.AppendText(s + "\n");
                }
            }
        }

        //подсветка подходящих комнат
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int kol = Convert.ToInt32(comboBox1.SelectedItem);
                int min = Convert.ToInt32(textBox1.Text);
                int max = Convert.ToInt32(textBox2.Text);

                List<int> res = roomList.FindRooms(min, max, kol);
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                        if (!res.Contains(Convert.ToInt32(cell.Value)))
                            cell.Style.BackColor = Color.LightGray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неверно заполнены поля.", "Подбор номеров", MessageBoxButtons.OK);
            }
        }

        //сброс настроек подбора номера
        private void button3_Click(object sender, EventArgs e)
        {
            FillTable();
            comboBox1.SelectedItem = " ";
            textBox1.Text = "";
            textBox2.Text = "";
        }

        //добавление гостей в номер
        private void button1_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(dataGridView1.SelectedCells[0].Value);
            RegistrationForm form = new RegistrationForm(num, this);
            form.ShowDialog();

            if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                roomList.SetInRoom(form.visitors, num);
                form.Close();
                FillTable();
                RefreshInfo();
            }
        }

        //сохранение базы
        private void Save()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream file = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    formatter.Serialize(file, roomList);
                }
            }
        }
        // Сохранение при нажатии соответствующего пункта меню
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        //выгрузка из файла базы
        private void Load()
        {
            сохранитьToolStripMenuItem.Enabled = true;

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "txt файлы|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        roomList = new RoomList();
                        BinaryFormatter formatter = new BinaryFormatter();
                        using (FileStream file = new FileStream(dialog.FileName, FileMode.Open))
                        {
                            roomList = (RoomList)formatter.Deserialize(file);
                        }
                        roomList.DeleteDelayed();
                        //visitorList = new VisitorList(roomList);
                    }
                    catch (Exception excep)
                    { 
                        MessageBox.Show(excep.Message, "Error", MessageBoxButtons.OK);
                    }
                }
            }
            RefreshInfo();
            FillTable();
        }
        // выгрузка из файла при гажатии на соответствующий пункт меню
        private void продолжитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Load();
            pictureBox1.Visible = false;
            сохранитьToolStripMenuItem.Enabled = true;

        }

        //начало работы с программой
        private void начатьЗановоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            FillTable();
            RefreshInfo();
            сохранитьToolStripMenuItem.Enabled = true;
        }

        //освобождение комнаты, кнопка активна, только если сегодня - день выезда
        private void button4_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(dataGridView1.SelectedCells[0].Value);
            roomList.ClearRoom(num);
            FillTable();
            RefreshInfo();
        }

        //изменнение даты отъезда
        private void button5_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(dataGridView1.SelectedCells[0].Value);
            RegistrationForm form = new RegistrationForm(num, this);
            form.ShowDialog();

            if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                roomList.SetInRoom(form.visitors, num);
                form.Close();
                FillTable();
                RefreshInfo();
            }
        }

        //выбор всех постояльцев, отъезжающих сегодня
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> res = roomList.TodayDepature();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                        if (!res.Contains(Convert.ToInt32(cell.Value)))
                            cell.Style.BackColor = Color.LightGray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Bыбор всех постояльцев, отъезжающих сегодня", MessageBoxButtons.OK);
            }
        }

        // сброс после выбора всех постояльцев, отъезжающих сегодня
        private void button7_Click(object sender, EventArgs e)
        {
            FillTable();
        }

        // Поиск гостя по произвольному признаку
        private void button8_Click(object sender, EventArgs e)
        {
            SearchForm form = new SearchForm(roomList.GetVisitorList());
            form.ShowDialog();
            RefreshInfo();
        }
        
        // нажатие на кнопку "Сохранить изменения"
        private void button9_Click(object sender, EventArgs e)
        {
            Save();
        }

        // нажатие на пункт меню "Справка"
        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help form = new Help();
            form.ShowDialog();
        }
    }
}
