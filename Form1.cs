using System;
using System.IO;
using OpenQA.Selenium;
using System.Globalization;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
public struct ParamsForBot
{
    public string TypeOfGame;
    public int HowManyRates;
    public string SumOfRate;
    public string PercentOfRate;
}
public struct GameStruct
{
    public string[] TimeNow;
    public bool[] Win;
    public string[] Balance;
}
namespace FreeBitcoin
{
    public partial class Form1 : Form
    {
        ParamsForBot[] Params = new ParamsForBot[1000];
        GameStruct[] Game = new GameStruct[1000];
        IWebDriver Browser;
        StreamWriter Writer;
        FileStream FileTxt1;
        public string GoodDecSeparator, BadDecSeparator;

        public Form1()
        {
            InitializeComponent();
            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            GoodDecSeparator = nfi.NumberDecimalSeparator;
            BadDecSeparator = (GoodDecSeparator == ".") ? "," : ".";
        }    
    public void instructions_Click(object sender, EventArgs e)
        {            
            int j = 1;
            OpenFileDialog OPF = new OpenFileDialog();
            OPF.Filter = "Файлы .txt|*.txt";
            if (OPF.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(OPF.FileName);
                string InstructionsPath = OPF.FileName;
                FileStream FileTxt = new FileStream(InstructionsPath, FileMode.Open);
                StreamReader Reader = new StreamReader(FileTxt);
                string temp;
                while ((temp = Reader.ReadLine()) != null) // считывание из файла и заполнение массива структур
                {
                    string[] ParamsArr = new string[4];
                    try
                    {
                        ParamsArr = temp.Split(' '); // массив из строк в каждой строке файла

                        switch (ParamsArr[0][0]) // Вверх или вниз
                        {
                            case '!':
                                Params[j].TypeOfGame = Params[j - 1].TypeOfGame;
                                break;
                            default:
                                Params[j].TypeOfGame = ParamsArr[0];
                                break;
                        }
                        switch (ParamsArr[1][0]) // Количество ставок
                        {
                            case '!':
                                Params[j].HowManyRates = Params[j - 1].HowManyRates;
                                break;
                            default:
                                Params[j].HowManyRates = Int32.Parse(ParamsArr[1]);
                                break;
                        }
                        switch (ParamsArr[2][0]) // Cтрока с суммой ставки
                        {
                            case '!':
                                Params[j].SumOfRate = Params[j - 1].SumOfRate;
                                break;
                            default:
                                if (ParamsArr[2][ParamsArr[2].Length - 1] != '!') Params[j].SumOfRate = ParamsArr[2];
                                else
                                {
                                    string t = ParamsArr[2].Substring(0, ParamsArr[2].Length - 1);
                                    Params[j].SumOfRate = t;
                                }
                                break;
                        }
                        switch (ParamsArr[3][0]) // Строка с процентом выигрыша
                        {
                            case '!':
                                Params[j].PercentOfRate = Params[j - 1].PercentOfRate;
                                break;
                            default:
                                if (ParamsArr[3][ParamsArr[3].Length - 1] != '!') Params[j].PercentOfRate = ParamsArr[3];
                                else
                                {
                                    string t = ParamsArr[3].Substring(0, ParamsArr[3].Length - 1);
                                    Params[j].PercentOfRate = t;
                                }
                                break;
                        }
                        j++;
                    }
                    catch
                    {
                        MessageBox.Show("Проверьте правильность тектового документа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
                Params[j + 1].TypeOfGame = null; // остановка
            }
            else MessageBox.Show("Ошибка открытия файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);   
        }
        BackgroundWorker GoToSite;
        BackgroundWorker Work;
        private async void GoToSiteClick()
        {
            Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            Browser.Manage().Window.Maximize();
            Browser.Navigate().GoToUrl("https://freebitco.in/");
            Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            IWebElement LoginTab = Browser.FindElement(By.XPath("//li[@class='login_menu_button']"));
            LoginTab.Click();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GoToSite = new BackgroundWorker();
                GoToSite.DoWork += (obj, ea) => GoToSiteClick();
                GoToSite.RunWorkerAsync(); 
            }
            catch
            {
                MessageBox.Show("Ошибка на сайте", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                async void work()
                {
                    Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                    IWebElement High = Browser.FindElement(By.Id("double_your_btc_bet_hi_button")); // Hi
                    IWebElement Low = Browser.FindElement(By.Id("double_your_btc_bet_lo_button")); // Lo
                    IWebElement Bal = Browser.FindElement(By.Id("balance")); // Balance
                    IWebElement d1 = Browser.FindElement(By.Id("multiplier_first_digit")); // 1 цифра
                    IWebElement d2 = Browser.FindElement(By.Id("multiplier_second_digit")); // 2 цифра
                    IWebElement d3 = Browser.FindElement(By.Id("multiplier_third_digit")); // 3 цифра
                    IWebElement d4 = Browser.FindElement(By.Id("multiplier_fourth_digit")); // 4 цифра
                    IWebElement d5 = Browser.FindElement(By.Id("multiplier_fifth_digit")); // 5 цифра
                    IWebElement More = Browser.FindElement(By.XPath("//span[@class='gt bold gt_lt_span']"));
                    IWebElement Less = Browser.FindElement(By.XPath("//span[@class='lt bold gt_lt_span']"));
                    int WinsAll = 0, LosesAll = 0;
                    for (int i = 1; i < 1000; i++)
                    {
                        if (Params[i].TypeOfGame == null) break;
                        Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                        IWebElement SumOfRate = Browser.FindElement(By.Id("double_your_btc_stake")); // поле с суммой ставки
                        SumOfRate.SendKeys("\b\b\b\b\b\b\b\b\b\b");
                        SumOfRate.SendKeys(Params[i].SumOfRate);
                        IWebElement PercentOfRate = Browser.FindElement(By.Id("double_your_btc_win_chance")); // поле с процентом выигрыша
                        PercentOfRate.SendKeys("\b\b\b\b\b\b");
                        PercentOfRate.SendKeys(Params[i].PercentOfRate);
                        Game[i].Balance = new string[Params[i].HowManyRates];
                        Game[i].Win = new bool[Params[i].HowManyRates];
                        Game[i].TimeNow = new string[Params[i].HowManyRates];
                        string History = "";
                        switch (Params[i].TypeOfGame)
                        {
                            case "u":
                                for (int j = 0; j < Params[i].HowManyRates; j++)
                                {
                                    //System.Threading.Thread.Sleep(500);
                                    Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                                    Game[i].Balance[j] = Bal.Text;
                                    //System.Threading.Thread.Sleep(250);
                                    string number = ""; // Результат игры
                                    if (d1.Text != "0") number += d1.Text;
                                    number += d2.Text;
                                    number += d3.Text;
                                    number += d4.Text;
                                    number += d5.Text;
                                    Game[i].Win[j] = (Convert.ToInt32(number) > Convert.ToInt32(More.Text)) ? true : false;
                                    Game[i].TimeNow[j] = DateTime.Now.ToLongTimeString();
                                    string TempWin = Game[i].Win[j] ? "Выигрыш" : "Проигрыш";
                                    string end = Game[i].Win[j] ? "1" : "0";
                                    History += end;
                                    Writer.WriteLine(i.ToString() + ". " + Game[i].TimeNow[j] + " " + TempWin + "   Баланс:" + Game[i].Balance[j] + "\n");
                                    High.Click();
                                }
                                break;
                            case "d":
                                for (int j = 0; j < Params[i].HowManyRates; j++)
                                {
                                    //System.Threading.Thread.Sleep(500);
                                    Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                                    Game[i].Balance[j] = Bal.Text;
                                    //System.Threading.Thread.Sleep(250);
                                    string number = ""; // Результат игры
                                    if (d1.Text != "0") number += d1.Text;
                                    number += d2.Text;
                                    number += d3.Text;
                                    number += d4.Text;
                                    number += d5.Text;
                                    Game[i].Win[j] = (Convert.ToInt32(number) < Convert.ToInt32(Less.Text)) ? true : false;
                                    Game[i].TimeNow[j] = DateTime.Now.ToLongTimeString();
                                    string TempWin = Game[i].Win[j] ? "Выигрыш" : "Проигрыш";
                                    string end = Game[i].Win[j] ? "1" : "0";
                                    History += end;
                                    Writer.WriteLine(i.ToString() + ". " + Game[i].TimeNow[j] + " " + TempWin + "   Баланс:" + Game[i].Balance[j] + "\n");
                                    Low.Click();
                                }
                                break;
                        }
                        int WinMax = 0, LoseMax = 0, temp1 = 0, temp2 = 0, Wins = 0, Loses = 0;
                        for (int k = 0; k < History.Length; k++)
                        {
                            if (History[k] == '1')
                            {
                                Wins++;
                                if (temp2 > LoseMax) LoseMax = temp2;
                                temp2 = 0;
                                temp1++;
                            }
                            else
                            {
                                Loses++;
                                if (temp1 > WinMax) WinMax = temp1;
                                temp1 = 0;
                                temp2++;
                            }
                        }
                        if (temp1 > WinMax) WinMax = temp1;
                        if (temp2 > LoseMax) LoseMax = temp2;
                        WinsAll += Wins;
                        LosesAll += Loses;
                        string LogInBox = (i.ToString() + ".   Всего выигрышей:" + Wins.ToString() + "   Всего проигрышей:" + Loses.ToString());
                        LogInBox += ("   Наибольшее число побед подряд:" + WinMax.ToString() + "   Наибольшее число проигрышей подряд:" + LoseMax.ToString());
                        LogInBox += ("   Выигрышей за все время:" + WinsAll.ToString() + "   Проигрышей за все время:" + LosesAll.ToString());
                        int r = Params[i].HowManyRates - 1;
                        double sum = Convert.ToDouble((Game[i].Balance[r]).Replace(BadDecSeparator, GoodDecSeparator));
                        sum -= (Convert.ToDouble((Game[i].Balance[0]).Replace(BadDecSeparator, GoodDecSeparator)));
                        if (sum >= 0)
                        {
                            LogInBox += ("   Вы в плюсе на:" + sum.ToString("F8") + "\n");
                        }
                        else LogInBox += ("   Вы в минусе на:" + ((-1) * sum).ToString("F8") + "\n");
                        richTextBox1.Invoke((MethodInvoker) (() => richTextBox1.Text += LogInBox));
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(100);
                    }
                    double sum0 = Convert.ToDouble((Game[1].Balance[0]).Replace(BadDecSeparator, GoodDecSeparator));
                    int posEnd = 1;
                    for (int k = 1; k < 100; k++)
                    {
                        if (Params[k].TypeOfGame == null)
                        {
                            posEnd = k - 1;
                            break;
                        }
                    }
                    double sumEnd = Convert.ToDouble((Game[posEnd].Balance[(Params[posEnd].HowManyRates) - 1]).Replace(BadDecSeparator, GoodDecSeparator));
                    richTextBox2.Invoke((MethodInvoker) (() => richTextBox2.Text += ("Итог\n")));
                    richTextBox2.Invoke((MethodInvoker) (() => richTextBox2.Text += ("Сумма на балансе после старта бота: " + sum0.ToString("F8") + "\n")));
                    richTextBox2.Invoke((MethodInvoker) (() => richTextBox2.Text += ("Сумма на балансе после работы бота: " + sumEnd.ToString("F8") + "\n")));
                    if (sumEnd>=sum0) richTextBox2.Invoke((MethodInvoker)(() => richTextBox2.Text += ("Вы в плюсе на: " + (sumEnd-sum0).ToString("F8") + "\n")));
                    else richTextBox2.Invoke((MethodInvoker)(() => richTextBox2.Text += ("Вы в плюсе на: " + (sum0-sumEnd).ToString("F8") + "\n")));
                    Writer.Close();
                }
                Work = new BackgroundWorker();
                Work.DoWork += (obj, ea) => work();
                Work.RunWorkerAsync();
            }
            catch
            {
                MessageBox.Show("Ошибка на сайте", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF1 = new OpenFileDialog();
            OPF1.Filter = "Файлы .txt|*.txt";
            if (OPF1.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(OPF1.FileName);
                string LogsPath = OPF1.FileName;
                FileTxt1 = new FileStream(LogsPath, FileMode.Open);
                Writer = new StreamWriter(FileTxt1);
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
