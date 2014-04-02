using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Wenskaart
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            SortDescription sd = new SortDescription("Source", ListSortDirection.Ascending);
            LTypeComboBox.Items.SortDescriptions.Add(sd);
            KleurComboBox.Items.SortDescriptions.Add(sd);
            New_Executed();
        }

        public void LaadKleuren()
        {
            foreach (PropertyInfo info in typeof(Colors).GetProperties())
            {
                BrushConverter bc = new BrushConverter();
                SolidColorBrush deKleur = (SolidColorBrush)bc.ConvertFromString(info.Name);
                Kleur kleurke = new Kleur();
                kleurke.Borstel = deKleur;
                kleurke.Naam = info.Name;
                KleurComboBox.Items.Add(kleurke);
                if (kleurke.Naam == "White")
                    KleurComboBox.SelectedItem = kleurke;
            }
        }

        private void New_Executed()
        {
            TekeningCanvas.Children.Clear();
            TekeningCanvas.Background = (ImageBrush)this.TryFindResource("kerstAchtergrond");
            WensTextBox.Text = "";
            WensTextBox.FontSize = 10;
            HuidigBestand.Content = "Nieuw";
            LTypeComboBox.SelectedItem = new FontFamily("Arial");
            LaadKleuren();
        }
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            New_Executed();
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "Wenskaart";
                dlg.DefaultExt = ".wka";
                dlg.Filter = "Wenskaart documenten|*.wka";
                if (dlg.ShowDialog() == true)
                {
                    using (StreamReader bestand = new StreamReader(dlg.FileName))
                    {
                        TekeningCanvas.Background = new ImageBrush(new BitmapImage(new Uri(bestand.ReadLine())));
                        int aantalBallen = Convert.ToInt16(bestand.ReadLine());
                        int teller = 0;
                        while (teller < aantalBallen)
                        {
                            Ellipse eenBal = new Ellipse();
                            eenBal.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(bestand.ReadLine()));
                            Canvas.SetLeft(eenBal, Convert.ToDouble(bestand.ReadLine()));
                            Canvas.SetTop(eenBal, Convert.ToDouble(bestand.ReadLine()));
                            TekeningCanvas.Children.Add(eenBal);
                            teller++;
                        }
                        WensTextBox.Text = bestand.ReadLine();
                        LTypeComboBox.SelectedValue = new FontFamily(bestand.ReadLine());
                        WensTextBox.FontSize = (Double)(new FontSizeConverter().ConvertFrom(bestand.ReadLine()));

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Openen mislukt : " + ex.Message);
            }

        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ImageBrush background = (ImageBrush)TekeningCanvas.Background;
                List<Bal> BallenLijst = new List<Bal>();
                Int16 aantalBallen = new Int16();
                foreach (Ellipse eenBal in TekeningCanvas.Children)
                {
                    SolidColorBrush eenKleur = (SolidColorBrush)eenBal.Fill;
                    Double eenPosX = Canvas.GetLeft(eenBal);
                    Double eenPosY = Canvas.GetTop(eenBal);
                    BallenLijst.Add(new Bal(eenKleur, eenPosX, eenPosY));
                    aantalBallen++;
                }

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "Wenskaart";
                dlg.DefaultExt = ".wka";
                dlg.Filter = "Wenskaart documenten|*.wka";
                if (dlg.ShowDialog() == true)
                {
                    using (StreamWriter bestand = new StreamWriter(dlg.FileName))
                    {
                        bestand.WriteLine(background.ImageSource.ToString());
                        bestand.WriteLine(aantalBallen);
                        foreach (Bal eenBal in BallenLijst)
                        {
                            bestand.WriteLine(eenBal.Kleur.ToString());
                            bestand.WriteLine(eenBal.BalPosX.ToString());
                            bestand.WriteLine(eenBal.BalPosY.ToString());
                        }
                        bestand.WriteLine(WensTextBox.Text);
                        bestand.WriteLine(LTypeComboBox.SelectedValue.ToString());
                        bestand.WriteLine(LGrootteLabel.Content.ToString());
                    }
                    HuidigBestand.Content = dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Opslaan mislukt : " + ex.Message);
            }
        }
        private void PrintPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Bestand afdrukken ...", "Open", MessageBoxButton.OK);
        }
        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Wilt u het programma afsluiten?",
            "Afsluiten",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void Kaarten_Click(object sender, RoutedEventArgs e)
        {
            MenuItem kaartSoort = (MenuItem)sender;
            if ((string)kaartSoort.Header == "Geboortekaart")
                TekeningCanvas.Background = (ImageBrush)this.TryFindResource("geboorteAchtergrond");
            else
                TekeningCanvas.Background = (ImageBrush)this.TryFindResource("kerstAchtergrond");
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (WensTextBox.FontSize < 40)
            {
                WensTextBox.FontSize += 1;
            }
        }
        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            if (WensTextBox.FontSize > 10)
            {
                WensTextBox.FontSize -= 1;
            }
        }

        private Ellipse sleepBal = new Ellipse();
        private void BalEllipse_MouseMove(object sender, MouseEventArgs e)
        {
            sleepBal = (Ellipse)sender;
            if ((e.LeftButton == MouseButtonState.Pressed))
            {
                DataObject sleepKleur = new DataObject("deKleur", sleepBal.Fill);
                DragDrop.DoDragDrop(sleepBal, sleepKleur, DragDropEffects.Move);
            }
        }
        private void BalEllipse_Drop(object sender, DragEventArgs e)
        {
            Ellipse nieuweBal = new Ellipse();
            nieuweBal.Fill = (Brush)e.Data.GetData("deKleur");
            Point dropPoint = e.GetPosition(TekeningCanvas);
            Canvas.SetLeft(nieuweBal, (dropPoint.X - 20));
            Canvas.SetTop(nieuweBal, (dropPoint.Y - 20));
            TekeningCanvas.Children.Add(nieuweBal);
        }
    }
}



