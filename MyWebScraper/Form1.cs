using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        }

        private void InitTable()
        {
            _table = new DataTable("ScrapedInfoCollector");
            _table.Columns.Add("Phrasal Verb", typeof(string));
            _table.Columns.Add("Meaning", typeof(string));
            dataGridView1.DataSource = _table;
            dataGridView1.Columns[0].Width = 150;
            dataGridView1.Columns[1].Width = 500;
        }

        private async Task<List<AlphaAndIdiom>> IdiomsFromPage(string page)
        {
            string url = "https://www.englishclub.com/ref/Phrasal_Verbs/" + page + "/";
            var doc = await Task.Factory.StartNew(() => _web.Load(url));
            var alphaNodes = doc.DocumentNode.SelectNodes("//html//body//div//main//div//h3");
            var idiomNodes = doc.DocumentNode.SelectNodes("//html//body//div//main//div//div");

            if (alphaNodes == null || idiomNodes == null)
            {
                MessageBox.Show(@"Finished fetching all records");
                return new List<AlphaAndIdiom>();
            }

            var alphabets = alphaNodes.Select(node => node.InnerText);
            var idioms = idiomNodes.Select(node => node.InnerText);

            return alphabets.Zip(idioms, (alphabet, idiom) => new AlphaAndIdiom() {Alphabet = alphabet, Idiom = idiom}).ToList();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            InitTable();
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
