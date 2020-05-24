using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Timer {
    public partial class Form1 : Form {
        Save_Font s = new Save_Font();
        BinaryFormatter binFormat = new BinaryFormatter();
        BinaryFormatter formatter = new BinaryFormatter();
        int alert = 60; // время с которого начинают мигать цифры
        int limit = 120; // время для отсчета
        int reset_alert = 60; // для быстрого восстновления отсчета
        int reset_limit = 120; // для быстрого восстновления отсчета
        int visible_second = 60; // используется для визуализации в формате минуты:секнуд
        double temp; // временная переменная для преобразования дробного к целому
        int[] one_numb = new int[]{0,1,2,3,4,5,6,7,8,9}; //массив для одиночных цифр
        bool first_interval = true; // для отслеживания первого интервала таймера 
        bool first_start = true; // для отслеживания первого нажатия на кнопку старта\паузы
        public Form1() { InitializeComponent(); }
        private void timer1_Tick(object sender, EventArgs e) {   
            if (limit >= 0) {
                temp = limit / 60;
                if(visible_second <= 0) visible_second = 60; //визуализация секунд
                timer_label.Text = Convert.ToString(Convert.ToInt32(Math.Truncate(temp))) + ":" + fix_second(visible_second);
                if (first_interval) {
                    timer_label.Text = "00:00";
                    first_interval = false;
                }
                if (limit < alert) alert_alert(); // поредение вызова алерта
                limit--; // на секунду меньше
                visible_second--; // на секунду меньше для визуализации
            }
            else {
                timer1.Stop();
                first_start = true;
                button1.Text = "Start";
                first_interval = true;
                visible_second = 60;
            }
        }
        string fix_second(int n) {
            foreach (int i in one_numb)
                if (i == n) return "0" + n.ToString();
            return n.ToString();   
        }
        void alert_alert() {
            if (timer_label.ForeColor == Color.White) timer_label.ForeColor = Color.Red;
            else timer_label.ForeColor = Color.White;
        }
        private void timer_label_Click(object sender, EventArgs e) {
            if (settings_panel.Visible) settings_panel.Visible = false; else settings_panel.Visible = true;
        }
        private void input_limit_KeyPress(object sender, KeyPressEventArgs e) { if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8) e.Handled = true; } // ввод только цифр 
        private void button3_Click(object sender, EventArgs e)  {
            fontDialog1.ShowColor = true;
            fontDialog1.Font = timer_label.Font;
            fontDialog1.Color = timer_label.ForeColor;
            if (fontDialog1.ShowDialog() != DialogResult.Cancel) {
                timer_label.Font = fontDialog1.Font;
                timer_label.ForeColor = fontDialog1.Color;
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            if (!File.Exists("_settingfont.timer"))  File.Create("_settingfont.timer");
            using (Stream fStream = new FileStream("_settingfont.timer", FileMode.Create, FileAccess.Write, FileShare.None)) {
                    s.save_font = timer_label.Font;
                    s.save_color = Color.White;
                    binFormat.Serialize(fStream, s);
                    fStream.Close();
                }
            }
        private void button1_Click(object sender, EventArgs e) {
            error_log.Text = "";
            if(input_alert.Text == "")  error_log.Text += "Не установлен алерт; ";
            if(input_limit.Text == "")  error_log.Text += "Не установленно новое время; ";
            if (error_log.Text == "") if (Convert.ToInt32(input_limit.Text) < Convert.ToInt32(input_alert.Text)) error_log.Text += "Значение алерта больше значения времени; ";
            if (error_log.Text == "")
                 if (button1.Text == "Старт") {
                    button1.Text = "Пауза";
                    timer1.Start();
                    if (first_start)
                    {
                        limit = Convert.ToInt32(input_limit.Text) * 60;
                        alert = Convert.ToInt32(input_alert.Text) * 60;
                        reset_limit = Convert.ToInt32(input_limit.Text) * 60;
                        reset_alert = Convert.ToInt32(input_alert.Text) * 60;
                        first_start = false;
                    }
                 }
                 else {
                    button1.Text = "Старт";
                    timer1.Stop();
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            limit = reset_limit;
            alert = reset_alert;
        }
        private void Form1_Load(object sender, EventArgs e) {
            if (File.Exists("_settingfont.timer")) {
                FileStream fs = new FileStream("_settingfont.timer", FileMode.Open);
                s = (Save_Font)formatter.Deserialize(fs);
                timer_label.Font = s.save_font;
                timer_label.ForeColor = s.save_color;
                fs.Close();
            }
            else using (Stream fStream = new FileStream("_settingfont.timer", FileMode.Create, FileAccess.Write, FileShare.None)) {
                    s.save_font = new Font(timer_label.Font.Name, 100);
                    s.save_color = Color.White;
                    binFormat.Serialize(fStream, s);
                    fStream.Close();
                 }
        }
        private void задатьЗначенияПоНовойToolStripMenuItem_Click(object sender, EventArgs e) {
            error_log.Text = "";
            timer1.Stop();
            button1.Text = "Старт";
            first_start = true;
            first_interval = true;
            timer_label.Text = "00:00";
            visible_second = 60;
            error_log.Text = "Таймер cброшен; ";
        }
   }
}
