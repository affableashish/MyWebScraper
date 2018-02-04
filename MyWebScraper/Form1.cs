using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace MyWebScraper
{

    public partial class Form1 : Form
    {
        private DataTable _table;
        private readonly HtmlWeb _web = new HtmlWeb();
        public Form1()
        {
            InitializeComponent();
            InitTable();
        }

        private void InitTable()
        {
            _table = new DataTable("ScrapedInfoCollector");
            _table.Columns.Add("Starts With", typeof(string));
            _table.Columns.Add("Idiom", typeof(string));
            dataGridView1.DataSource = _table;
        }

        private async Task<List<AlphaAndIdiom>> IdiomsFromPage(string page)
        {
            string url = "http://www.englishclub.com/ref/Phrasal_Verbs/" + page + "/";
            var doc = await Task.Factory.StartNew(() => _web.Load(url));
            var alphaNodes = doc.DocumentNode.SelectNodes("/html/body/div[2]/main//div/h3/a");

            // /html/body/div[2]/main/div[3]/h3/a
            // /html/body/div[2]/main/div[4]/h3/a

            // /html/body/div[2]/main/div[3]/div
            // /html/body/div[2]/main/div[4]/div

            var idiomNodes = doc.DocumentNode.SelectNodes("/html/body/div[2]/main//div/div");

            if (alphaNodes == null || idiomNodes == null)
            {
                MessageBox.Show(@"Hey, XPATH didn't bring you anything");
                return new List<AlphaAndIdiom>();
            }

            var alphabets = alphaNodes.Select(node => node.InnerText);
            var idioms = idiomNodes.Select(node => node.InnerText);


            MessageBox.Show(@"I made it through the task");
            return alphabets.Zip(idioms, (alphabet, idiom) => new AlphaAndIdiom() {Alphabet = alphabet, Idiom = idiom}).ToList();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            int enumInit = 0;
            string page = Enum.GetName(typeof(Alphabets), enumInit);
            var idiomsFromPage = await IdiomsFromPage(page);
            while (idiomsFromPage.Count > 0)
            {
                foreach (var idiom in idiomsFromPage)
                {
                    _table.Rows.Add(idiom.Alphabet, idiom.Idiom);
                }
                page = Enum.GetName(typeof(Alphabets), ++enumInit);
                idiomsFromPage = await IdiomsFromPage(page);
            }

        }
    }
}
