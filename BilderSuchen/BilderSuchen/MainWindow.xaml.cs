using System;
using System.Windows;
using System.IO;
using System.Data;
using System.Reflection;

namespace BilderSuchen
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable FehlendeBilderTabelle = new DataTable();
        DataTable VorhandeneBilderTabelle = new DataTable();

        string AktuellesVerzeichnis = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        string Datei_Extension;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void BilderSuchenDateiLesen()
        {
            GetTableFromCSV("BilderListe.txt", '#');
        }


        private void GetTableFromCSV(string path, char seperator)
        {
            FileStream aFile = new FileStream(path, FileMode.Open);

            using (StreamReader sr = new StreamReader(aFile, System.Text.Encoding.Default))
            {
                string strLine = sr.ReadLine();
                string[] strArray = strLine.Split(seperator);

                FehlendeBilderTabelle.Columns.Add("DateinameMitPfad");
                FehlendeBilderTabelle.Columns.Add("NurDateiname");

                DataRow dr = FehlendeBilderTabelle.NewRow();

                while (sr.Peek() > -1)
                {
                    strLine = sr.ReadLine();
                    strArray = strLine.Split(seperator);
                    if (strArray.Length > 1)
                    {
                        if (strArray[1].Contains(Datei_Extension))
                        {
                            FehlendeBilderTabelle.Rows.Add(strArray);
                        }
                    }
                }
            }
        }

        public void InhaltsverzeichnisLesen()
        {
            DataRow dr = VorhandeneBilderTabelle.NewRow();
            VorhandeneBilderTabelle.Columns.Add("DateiSystemNameMitPfad");
            DirSearch(AktuellesVerzeichnis);
        }

        private void DirSearch(string sDir)
        {
            string TextZeile;

            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        TextZeile = f.ToString();
                        if (TextZeile.Contains(Datei_Extension))
                        {
                            VorhandeneBilderTabelle.Rows.Add(TextZeile);
                        }
                    }
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void BilderAnzeigen()
        {
            DataRow[] resultFehlend = FehlendeBilderTabelle.Select();
            DataRow[] resultDateiSystem = VorhandeneBilderTabelle.Select();

            foreach (DataRow rowFehlt in resultFehlend)
            {
                ListBoxFehlendeBilder.Items.Add(rowFehlt.ItemArray[1].ToString());
            }

            foreach (DataRow rowVorhande in resultDateiSystem)
            {
                ListBoxDateiverzeichnisBilder.Items.Add(rowVorhande.ItemArray[0].ToString());
            }
        }

        private void BilderSuchenUndAnzeigen()
        {
            string BilderName;
            string SelectQuery;

            DataRow[] resultFehlend = FehlendeBilderTabelle.Select();
            DataRow[] resultDatei;

            foreach (DataRow rowFehlt in resultFehlend)
            {
                BilderName = rowFehlt.ItemArray[1].ToString();
                SelectQuery = "DateiSystemNameMitPfad like '%" + BilderName + "%' ";
                resultDatei = VorhandeneBilderTabelle.Select(SelectQuery);

                foreach (DataRow row in resultDatei)
                {
                    ListBoxTrefferBilder.Items.Add(rowFehlt.ItemArray[0].ToString());
                    ListBoxTrefferBilder.Items.Add(row.ItemArray[0].ToString());
                    ListBoxTrefferBilder.Items.Add(" ");
                }
            }
        }

        private void KnopfSuchen_Click(object sender, RoutedEventArgs e)
        {
            Datei_Extension = '.' + BilderExtension.Text;
            ListBoxFehlendeBilder.Items.Clear();
            ListBoxDateiverzeichnisBilder.Items.Clear();

            BilderSuchenDateiLesen();
            InhaltsverzeichnisLesen();
            BilderAnzeigen();
            BilderSuchenUndAnzeigen();
        }
    }

}
