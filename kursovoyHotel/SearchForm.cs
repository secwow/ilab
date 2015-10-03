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

namespace kursovoyHotel
{
    public partial class SearchForm : Form
    {
        VisitorList visitors;

        public SearchForm(VisitorList visitorList)
        {
            InitializeComponent();
            visitors = visitorList;

            dataGridView1.DataSource = null;
            visitorListBindingSource.DataSource = visitors;
            dataGridView1.DataSource = visitorListBindingSource;

        }

        //фильтр по номеру
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int num = Convert.ToInt32(comboBox1.Text);
                dataGridView1.DataSource = null;
                visitorListBindingSource.DataSource = ((VisitorList)(visitorListBindingSource.DataSource)).GetByRoomNum(num);
                dataGridView1.DataSource = visitorListBindingSource;
                panel2.Enabled = false;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Фильтр по номеру", MessageBoxButtons.OK);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Номер введен в неверном формате.", "Фильтр по номеру", MessageBoxButtons.OK);
            }
        }

        // сброс фильтров
        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            visitorListBindingSource.DataSource = visitors;
            dataGridView1.DataSource = visitorListBindingSource;

            panel2.Enabled = true;
            panel3.Enabled = true;
            panel4.Enabled = true;
            panel5.Enabled = true;
            panel6.Enabled = true;
            panel7.Enabled = true;
            panel8.Enabled = true;
        }

        //фильтр по стране
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.Text == "")
                    throw new FormatException("Не выбрана страна.");
                dataGridView1.DataSource = null;
                visitorListBindingSource.DataSource = ((VisitorList)(visitorListBindingSource.DataSource)).GetByCountry(comboBox2.Text);
                dataGridView1.DataSource = visitorListBindingSource;

                panel3.Enabled = false;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Фильтр по стране", MessageBoxButtons.OK);
            }
        }
        // формирование выпадающего списка стран при нажатии н акомбобокс
        private void comboBox2_MouseDown(object sender, MouseEventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(((VisitorList)(visitorListBindingSource.DataSource)).GetContries());
        }

        // формирование выпадающего списка городов при нажатии н акомбобокс
        private void comboBox3_MouseDown(object sender, MouseEventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Items.AddRange(((VisitorList)(visitorListBindingSource.DataSource)).GetCities());
        }
        //фильтр по городу
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.Text == "")
                    throw new FormatException("Город не выбран.");
                dataGridView1.DataSource = null;
                visitorListBindingSource.DataSource = ((VisitorList)(visitorListBindingSource.DataSource)).GetByCity(comboBox3.Text);
                dataGridView1.DataSource = visitorListBindingSource;

                panel4.Enabled = false;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Фильтр по городу", MessageBoxButtons.OK);
            }
        }

        //Фильтр по дате приезда
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = null;
                visitorListBindingSource.DataSource = ((VisitorList)(visitorListBindingSource.DataSource)).GetByArrivalDate(dateTimePicker1.Value, dateTimePicker2.Value);
                dataGridView1.DataSource = visitorListBindingSource;
                panel5.Enabled = false;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Фильтр по дате приезда", MessageBoxButtons.OK);
            }
        }

        //Фильтр по дате отъезда
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = null;
                visitorListBindingSource.DataSource = ((VisitorList)(visitorListBindingSource.DataSource)).GetByDepatureDate(dateTimePicker3.Value, dateTimePicker4.Value);
                dataGridView1.DataSource = visitorListBindingSource;
                panel6.Enabled = false;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Фильтр по дате отъезда", MessageBoxButtons.OK);
            }
        }

        //Фильтр по дате рождения
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = null;
                visitorListBindingSource.DataSource = ((VisitorList)(visitorListBindingSource.DataSource)).GetByBirthday(dateTimePicker5.Value, dateTimePicker6.Value);
                dataGridView1.DataSource = visitorListBindingSource;
                panel7.Enabled = false;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Фильтр по дате рождения", MessageBoxButtons.OK);
            }
        }

        //фильтр по части имени
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "Введите любую часть ФИО")
                return;
            dataGridView1.DataSource = null;
            visitorListBindingSource.DataSource = ((VisitorList)(visitorListBindingSource.DataSource)).GetByNamePart(textBox1.Text);
            dataGridView1.DataSource = visitorListBindingSource;

            panel8.Enabled = false;
        }

        //при получении фокуса ввода очистить поле
        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;
            textBox1.ForeColor = Color.Black;
        }

        //при покидании фокуса ввода вернуть подсказку
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == String.Empty)
            {
                textBox1.Text = "Введите любую часть ФИО";
                textBox1.ForeColor = Color.Silver;
            }
        }


        
    }
}
