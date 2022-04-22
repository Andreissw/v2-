using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using ModuleConnect;
using РеестрКонтроля_КЗ;


namespace РежимыРеестраКЗ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void Уникальные_наименование()
            {
            string sql;
            sql = @"USE QA SELECT distinct(r.NameProduct)  FROM [QA].[dbo].[Registry_SMT_Table1] as TB1  left join Registry_Name as R on TB1.idNameProduct = r.id  where idUser !=211";
            Class1.loadgrid(ГридProject, sql);
        }

        void УникальныйПользователь()
        {
            string sql;
            sql = @"USE QA SELECT distinct(us.UserName)  FROM [QA].[dbo].[Registry_SMT_Table1] as tb  inner join Fas.dbo.M_Users as us on tb.idUser = us.UserID where userid != 211";
            Class1.loadgrid(ГридProject, sql);
        }

        void цикл(ComboBox CB)
        {
            
            for (int i = 0; i < ГридProject.RowCount - 1; i++)
            {
              CB.Items.Add(ГридProject.Rows[i].Cells[0].Value);
            }
        }



        private void ОтчетКнопка_Click(object sender, EventArgs e)
        {

            ДатаГрупп.Enabled = false;
            Уникальные_наименование();
            цикл(ComboPoisk);
            УникальныйПользователь();
            цикл(UserCombo);
            ЗагрузкаЭкрана(ГруппОтчеты, StartGR, true, 780, 625, false);
            Список_ПроектовВсех();
            ГридProject.Columns[2].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            Цех.Checked = true;
            СМТ.Checked = true;
            ComboPoisk.Select();
            Модель = null;
            try
            {
                if (ГридProject.RowCount == 0) { } else { Модель = ГридProject.Rows[0].Cells[0].Value.ToString(); idmode = ГридProject.Rows[0].Cells[1].Value.ToString(); NAMEUSER = ГридProject.Rows[0].Cells[3].Value.ToString(); Дата = ГридProject.Rows[0].Cells[2].FormattedValue.ToString(); key = ГридProject.Rows[0].Cells[4].Value.ToString(); }
            }
            catch (Exception t)
            { Clipboard.SetText(t.ToString()); }

        }

        private void РедактированиеКнопка_Click(object sender, EventArgs e) // Начало программы, кнопка редактирование, создает новую ветвь событий, где пользователь сможет создать новый проект
        {
            ЗагрузкаЭкрана(LogginGR, StartGR, true, 235, 210, false); //Функция позволяет подогнать размеры формы под контроллер
            RFID.Select();

        }

        bool Проверка = false;
        private void LogginBT_Click(object sender, EventArgs e)
        {

            ЛоггинМетод(); //Заходит в базу, проверяет наличие RFID в базе
          


        }

        public static string idus; //Глобальная переменная с ID user

        void idUser(string cmd)
        {
            idus = Class1.LogginID(cmd).ToString();
        }

        void Список_Проектов() //Функция показывает определнному пользователю его созданные проекты
        {
            string sql;
            sql = @"Use QA  SELECT NameProduct as Наименование ,m.Mode as Цех ,date as Дата_Создание_Проекта, nm.id
                        FROM [QA].[dbo].[Registry_SMT_Table1] as TB   Inner join Registry_ProjectDate as dt on tb.idDateCreate = dt.id
                        inner join Registry_Name as nm on tb.idNameProduct = nm.id    inner join Registry_Mode as m on tb.idmode = m.id     where idUser = '" + idus + "' and idrow = 1   order by Дата_Создание_Проекта desc   ";
            Class1.loadgrid(ГридСписокПроектов, sql);
            //Class1.loadgrid(Тест, sql);
        }

        void Список_ПроектовВсех() //Функция показывает определнному пользователю его созданные проекты
        {
            string sql;
            sql = @"Use QA  SELECT Distinct(NameProduct) as Наименование ,m.Mode as Цех ,date as 'Дата', us.UserName as 'Пользователь', nm.id
             FROM [QA].[dbo].[Registry_SMT_Table1] as TB  	 Inner join Registry_ProjectDate as dt on tb.idDateCreate = dt.id    inner join Registry_Name as nm on tb.idNameProduct = nm.id   
	         inner join Registry_Mode as m on tb.idmode = m.id  	 inner join FAS.dbo.M_Users as us on TB.idUser = us.UserID	 where userid != 211  group by NameProduct,[date],m.Mode,us.UserName, nm.id
                
	         order by Дата desc  ";
            Class1.loadgrid(ГридProject, sql);
        }

       



        private void ЛоггинМетод() //Заходит в базу, проверяет наличие RFID в базе
        {
            Class1.Loggin(RFID, Result, ErrorLabel, Проверка);
            if (Result.Text == "")
            {

            }
            else
            {
                Модель = null;
                ГридСписокПроектов.DataSource = null;
                LoginLabel.Text = $"Приветствую {Result.Text}";
                ЗагрузкаЭкрана(СписокГруппа, LogginGR, true, 480, 435, false);
                ГруппаЦех.Visible = false;
                Loggin = Result.Text;
                LogginLB.Text = Result.Text;
                idUser(Result.Text);
                Список_Проектов();
                ГридСписокПроектов.Columns[2].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                try { if (ГридСписокПроектов.RowCount == 0) { } else { Модель = ГридСписокПроектов.Rows[0].Cells[0].Value.ToString(); idmode = ГридСписокПроектов.Rows[0].Cells[1].Value.ToString(); Дата = ГридСписокПроектов.Rows[0].Cells[2].FormattedValue.ToString(); key = ГридСписокПроектов.Rows[0].Cells[3].Value.ToString();  }
                } catch (Exception e) { Clipboard.SetText(e.ToString()); }

            }

        }



        private void Перехолдкглавнойформе() //Создает новый класс формы, загружает с библиотеке второй проект
        {
            var реестр = new ГлавнаяФорма();
            реестр.Show();
            this.Hide(); //Скрывает текущую форму
        }


        public static bool Новый_отчёт = false;
        private void button1_Click(object sender, EventArgs e) //Кнопка возвращение с логина в начальное состояние программы
        {
            ЗагрузкаЭкрана(StartGR, LogginGR, true, 350, 150, false);
            ErrorLabel.Visible = false;
            RFID.Clear();
        }

        bool ГруппаЦехбул = false;

        private void Новый_отчет_Click(object sender, EventArgs e) //Кнопка загружающий новый чистый отчет, создается булевая переменная, которая далее 2 проекту говорит что проект нужно зпускать с чистого листа
        {
            ГруппаЦехбул = true;
            ЗагрузкаЭкрана(ГруппаЦех, СписокГруппа, true, 360, 290, false);
            Новый_отчёт = true; // та самая перменная
            NameProjec.Select();
            ГруппаЦех.Visible = true;
        }

        public static int mode;
        public static bool modeAQV;
        private void button2_Click(object sender, EventArgs e) //Открытие 2 проекта
        {
            СозданиПроекта();
        }

        private void СозданиПроекта()
        {
            if (CheckSMT.Checked == false & CheckЦех.Checked == false) //Условие на выбор цеха
            {
                MessageBox.Show("Выберите цех", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NameProjec.Select();
                return;
            }            
            else if (NameProjec.Text == "")
            {
                MessageBox.Show("Напишите имя заказа", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NameProjec.Select();
                return;
            }
            else if (CheckSMT.Checked == true)
            {
                if (AqvCH.Checked == false) modeAQV = false; else modeAQV = true;
                mode = 1;                
                Модель = NameProjec.Text;
                Перехолдкглавнойформе();  //Сам переход, если тыкнуть на код и нажать F12 откроется сама функция -_-

            }

            else if (CheckЦех.Checked == true)
            {
               if (AqvCH.Checked == false) modeAQV = false; else modeAQV = true;
                mode = 2;
                Модель = NameProjec.Text;
                Перехолдкглавнойформе();  //Сам переход, если тыкнуть на код и нажать F12 откроется сама функция -_-
            }


            }

        string ПроверкаИменование(string Имя, int num)
        {
            string sql;
            sql = @"Use QA SELECT   [NameProduct]  FROM [QA].[dbo].[Registry_Name]  where NameProduct = '" + Имя + "' and idMode = '" + num + "'";
            return Class1.SelectString(sql).ToString();



        }


        public static string Loggin;
        void ЗагрузкаЭкрана(GroupBox GR, GroupBox GR2, bool видимость = false, int width = 1433, int height = 1000, bool автоскрол = true)
        {       //Функция загрузки экрана/, то есть подгон размеров формы под групповой контроллер
            var p = new Point(6, 6);
            var sz = new Size(0, 0);
            var pt = new Size(width, height);
            GR.Visible = видимость;
            GR.Location = Point.Add(p, sz);

            GR2.Visible = false;

            this.Size = Size.Add(sz, pt);
            this.AutoScroll = автоскрол;


        }

        private void RFID_KeyDown(object sender, KeyEventArgs e) //Обработка нажатие на кнопку "Enter"
        {
            if (e.KeyCode == Keys.Enter)
            {
                ЛоггинМетод();

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            End.Value = DateTime.Now.AddDays(1);
            ЗагрузкаЭкрана(StartGR, LogginGR, true, 350, 150, false);
            
        }

        public static int idРежим;

        private void CheckSMT_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckSMT.Checked == true)
            {
                CheckЦех.Checked = false;
                idРежим = 1;

            }
        }

        private void CheckЦех_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckЦех.Checked == true)
            {
                CheckSMT.Checked = false;
                idРежим = 2;

            }
        }

        int Индекс;
        public static string Модель, Дата, idmode,NAMEUSER,key;

        private void Выбрать_Click(object sender, EventArgs e)
        {
            if (Модель == null)
            {
                MessageBox.Show("Проект не выбран", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Новый_отчёт = false;
                modeAQV = Class1.SelectStringBool("use QA SELECT [Aqv]  FROM [QA].[dbo].[Registry_Name]  where id = '" + ГридСписокПроектов[3, ГридСписокПроектов.CurrentCell.RowIndex].Value + "'");
                mode = ВозвратЦеха();
                Перехолдкглавнойформе();
            }

        }

        int ВозвратЦеха()
        {
            string sql;
            sql = @"USE QA SELECT  id  FROM [QA].[dbo].[Registry_Mode]  where mode = '" + idmode + "'";
            return Convert.ToInt32(Class1.SelectStringInt(sql));
        }


        private void button3_Click(object sender, EventArgs e)
        {
            ГруппаЦехбул = false;
            ЛоггинМетод();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ЗагрузкаЭкрана(LogginGR, СписокГруппа, true, 235, 210, false);

            ErrorLabel.Text = "";
            Result.Visible = false;
            Result.Text = "";
            RFID.Enabled = true;
            RFID.Select();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ГруппаЦехбул == true)
            {
                if (e.KeyValue == (Char)Keys.Enter)
                {
                    СозданиПроекта();
                }
            }

        }

           
        private void button7_Click(object sender, EventArgs e)
        {

            ЗагрузкаЭкрана(StartGR, ГруппОтчеты, true, 350, 150, false);
        }
        
        private void Цех_CheckedChanged(object sender, EventArgs e)
        {
          
            ComboPoisk.Select();
        }

       

        private void СМТ_CheckedChanged(object sender, EventArgs e)
        {
           
            ComboPoisk.Select();
        }

        void ПоискЗапрос(string znak1, string zkan2, string zkan3, string nameproduct, string mode, string username, string startdate, string enddate) //Функция показывает определнному пользователю его созданные проекты
        {
            string sql;
            sql = @"USE QA SELECT Distinct(NameProduct) as Наименование ,m.Mode as Цех ,date as Дата, us.UserName as 'Пользователь', nm.id
            FROM [QA].[dbo].[Registry_SMT_Table1] as TB  	   Inner join Registry_ProjectDate as dt on tb.idDateCreate = dt.id  inner join Fas.dbo.M_Users as us on tb.idUser = us.UserID
  inner join Registry_Name as nm on tb.idNameProduct = nm.id    inner join Registry_Mode as m on tb.idmode = m.id

   where NameProduct " + znak1 + "= '" + nameproduct + "' and m.mode " + zkan2 + " = '" + mode + "' and UserName " + zkan3 + "= '" + username + "'   and Date between ('" + startdate + "') and ('" + enddate + "') and   userid != 211  order by Дата desc ";
            Class1.loadgrid(ГридProject, sql);
        }

        private void Поиск_Click(object sender, EventArgs e)
        {
            if (ComboPoisk.Text != "" & UserCombo.Text == "" & СМТ.Checked == true & Цех.Checked == true & checkBox1.Checked == false) //Именование не пустое, чеки все тру и без даты
            {
                ПоискЗапрос("","!","!",ComboPoisk.Text,"","", "01.01.2000", "01.01.2100");
            }
             else if (ComboPoisk.Text != "" & UserCombo.Text == "" & СМТ.Checked == false  & Цех.Checked == true & checkBox1.Checked == false) //Именование не пустое, чек цехсборки и без даты
            {
                ПоискЗапрос("", "", "!", ComboPoisk.Text, "ЦЕХ Сборки", "", "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text != "" & UserCombo.Text == "" & СМТ.Checked == true & Цех.Checked == false & checkBox1.Checked == false) //Именование не пустое, чек SMT и без даты
            {
                ПоискЗапрос("", "", "!", ComboPoisk.Text, "ЦПМ", "", "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text == "" & UserCombo.Text == "" & СМТ.Checked == true & Цех.Checked == true & checkBox1.Checked == false) //ТОлько чеки все тру
            {
                ПоискЗапрос("!", "!", "!", "", "", "", "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text == "" & UserCombo.Text == "" & СМТ.Checked == false & Цех.Checked == true & checkBox1.Checked == false) //ТОлько чек цех сбокри
            {
                ПоискЗапрос("!", "", "!", "", "ЦЕХ Сборки", "", "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text == "" & UserCombo.Text == "" & СМТ.Checked == true & Цех.Checked == false & checkBox1.Checked == false) //ТОлько чеки SMT
            {
                ПоискЗапрос("!", "", "!", "", "ЦПМ", "", "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text != "" & UserCombo.Text != "" & СМТ.Checked == true & Цех.Checked == true & checkBox1.Checked == false) //Имя ок, чеки все, пользователь ок, без даты
            {
                ПоискЗапрос("", "!", "", ComboPoisk.Text, "", UserCombo.Text, "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text != "" & UserCombo.Text != "" & СМТ.Checked == true & Цех.Checked == false & checkBox1.Checked == false) //Имя ок, чек SMT, пользователь ок, без даты
            {
                ПоискЗапрос("", "", "", ComboPoisk.Text, "ЦПМ", UserCombo.Text, "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text != "" & UserCombo.Text != "" & СМТ.Checked == false & Цех.Checked == true & checkBox1.Checked == false) //Имя ок, чек ЦехСборки, пользователь ок, без даты
            {
                ПоискЗапрос("", "", "", ComboPoisk.Text, "ЦЕХ Сборки", UserCombo.Text, "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text == "" & UserCombo.Text != "" & СМТ.Checked == true & Цех.Checked == true & checkBox1.Checked == false) //ТОлько польщзователь и чеки тру
            {
                ПоискЗапрос("!", "!", "", "", "", UserCombo.Text, "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text == "" & UserCombo.Text != "" & СМТ.Checked == false & Цех.Checked == true & checkBox1.Checked == false) //ТОлько польщзователь и чеки FAS
            {
                ПоискЗапрос("!", "", "", "", "ЦЕХ Сборки", UserCombo.Text, "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text == "" & UserCombo.Text != "" & СМТ.Checked == true & Цех.Checked == false & checkBox1.Checked == false) //ТОлько польщзователь и чеки SMT
            {
                ПоискЗапрос("!", "", "", "", "ЦПМ", UserCombo.Text, "01.01.2000", "01.01.2100");
            }
            else if ( СМТ.Checked == false & Цех.Checked == false) //ТОлько польщзователь и чеки SMT
            {
                ПоискЗапрос("!", "", "", "", "f", UserCombo.Text, "01.01.2000", "01.01.2100");
            }
            else if (ComboPoisk.Text != "" & UserCombo.Text == "" & СМТ.Checked == true & Цех.Checked == true & checkBox1.Checked == true) //С ДАТОЙ и именем проекта все чеки тру
            {
                ПоискЗапрос("", "!", "!", ComboPoisk.Text, "", "", starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }
            else if (ComboPoisk.Text != "" & UserCombo.Text == "" & СМТ.Checked == false & Цех.Checked == true & checkBox1.Checked == true) //С ДАТОЙ и именем проекта Чек цехсборки
            {
                ПоискЗапрос("", "", "!", ComboPoisk.Text, "ЦЕХ Сборки", "", starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }
            else if (ComboPoisk.Text != "" & UserCombo.Text == "" & СМТ.Checked == true & Цех.Checked == false & checkBox1.Checked == true) //С ДАТОЙ и именем проекта Чек SMT
            {
                ПоискЗапрос("", "", "!", ComboPoisk.Text, "ЦЕХ Сборки", "", starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }

            else if (ComboPoisk.Text != "" & UserCombo.Text != "" & СМТ.Checked == true & Цех.Checked == true & checkBox1.Checked == true) //С ДАТОЙ и именем проекта все чеки и пользователь
            {
                ПоискЗапрос("", "!", "", ComboPoisk.Text, "", UserCombo.Text, starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }

            else if (ComboPoisk.Text != "" & UserCombo.Text != "" & СМТ.Checked == true & Цех.Checked == false & checkBox1.Checked == true) //С ДАТОЙ и именем проекта Чек SMT и пользователь
            {
                ПоискЗапрос("", "", "", ComboPoisk.Text, "ЦПМ", UserCombo.Text, starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }

            else if (ComboPoisk.Text != "" & UserCombo.Text != "" & СМТ.Checked == false & Цех.Checked == true & checkBox1.Checked == true) //С ДАТОЙ и именем проекта Чек SMT и пользователь
            {
                ПоискЗапрос("", "", "", ComboPoisk.Text, "ЦЕХ Сборки", UserCombo.Text, starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }

            else if (ComboPoisk.Text == "" & UserCombo.Text != "" & СМТ.Checked == true & Цех.Checked == true & checkBox1.Checked == true) //С ДАТОЙ Пользователем, все чеки
            {
                ПоискЗапрос("!", "!", "", "", "", UserCombo.Text, starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }

            else if (ComboPoisk.Text == "" & UserCombo.Text == "" & СМТ.Checked == true & Цех.Checked == true & checkBox1.Checked == true) //С ДАТОЙ все чеки
            {
                ПоискЗапрос("!", "!", "!", "", "", "", starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }

            else if (ComboPoisk.Text == "" & UserCombo.Text == "" & СМТ.Checked == false & Цех.Checked == true & checkBox1.Checked == true) //С ДАТОЙ чек Цех
            {
                ПоискЗапрос("!", "", "!", "", "ЦЕХ Сборки", "", starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }
            else if (ComboPoisk.Text == "" & UserCombo.Text == "" & СМТ.Checked == true & Цех.Checked == false & checkBox1.Checked == true) //С ДАТОЙ чек SMT
            {
                ПоискЗапрос("!", "", "!", "", "ЦПМ", "", starrt.Value.ToString("yyyy-MM-dd"), End.Value.ToString("yyyy-MM-dd"));
            }

            try
            {
                if (ГридProject.RowCount == 0) { } else { Модель = ГридProject.Rows[0].Cells[0].Value.ToString(); idmode = ГридProject.Rows[0].Cells[1].Value.ToString(); NAMEUSER = ГридProject.Rows[0].Cells[3].Value.ToString(); Дата = ГридProject.Rows[0].Cells[2].FormattedValue.ToString(); key = ГридProject.Rows[0].Cells[4].Value.ToString(); }
            }
            catch (Exception t)
            { Clipboard.SetText(t.ToString()); }
        }

        void Чистка()
        {
            ComboPoisk.Text = "";
            UserCombo.Text = "";
            Цех.Checked = true;
            СМТ.Checked = true;
            checkBox1.Checked = false;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                ДатаГрупп.Enabled= true;
            }
            else
            {
                ДатаГрупп.Enabled = false;
            }
        }

        private void Сброс_Click(object sender, EventArgs e)
        {
            Чистка();
            Список_ПроектовВсех();
            
        }
        public static bool  Отчет = false;
        public static bool ОтчетШаблон = false;
        private void button6_Click(object sender, EventArgs e)
        {
           
            if (Модель == null)
            {
                MessageBox.Show("Проект не выбран", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                modeAQV = Class1.SelectStringBool("use QA SELECT [Aqv]  FROM [QA].[dbo].[Registry_Name]  where id = '" + ГридProject[4, ГридProject.CurrentCell.RowIndex].Value + "'");
                Отчет = true;
                idUser(NAMEUSER);
                mode = ВозвратЦеха();
                Перехолдкглавнойформе();

            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e) //Кнопка создание проекта по шаблону
        {
            if (Модель == null)
            {
                MessageBox.Show("Проект не выбран", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                ОтчетШаблон = true;
                idUser(Result.Text);
                mode = ВозвратЦеха();
                Перехолдкглавнойформе();

            }
        }

        private void ГридProject_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Start_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Индекс = ГридProject.CurrentCell.RowIndex;
            Модель = ГридProject.Rows[Индекс].Cells[0].Value.ToString();//обработка нажатие в гриде на строки проектов, записывает в переменную, 
            idmode = ГридProject.Rows[Индекс].Cells[1].Value.ToString();
            Дата = ГридProject.Rows[Индекс].Cells[2].FormattedValue.ToString();
            NAMEUSER = ГридProject.Rows[Индекс].Cells[3].Value.ToString();
            key = ГридProject.Rows[Индекс].Cells[4].Value.ToString(); //Записывается ключ
        }

        private void ГридСписокПроектов_CellClick_1(object sender, DataGridViewCellEventArgs e) 
        {

            Индекс = ГридСписокПроектов.CurrentCell.RowIndex;               
            Модель = ГридСписокПроектов.Rows[Индекс].Cells[0].Value.ToString();//обработка нажатие в гриде на строки проектов, записывает в переменную, 
            idmode = ГридСписокПроектов.Rows[Индекс].Cells[1].Value.ToString();
            Дата = ГридСписокПроектов.Rows[Индекс].Cells[2].FormattedValue.ToString(); // далее идет запрос в базу, и программа понимает с какиим проектом ему следует работать дальше
            key = ГридСписокПроектов.Rows[Индекс].Cells[3].Value.ToString(); //Записывается ключ
        }

     
    }
}
