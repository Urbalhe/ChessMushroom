using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SakkGombas
{
	public partial class MainWindow : Window
	{
		const int TABLA_MERET = 8;

		Button[,] tablaMezok = new Button[TABLA_MERET, TABLA_MERET];

		Point figuraPozicio;              // figura aktualis pozicioja
		List<Point> gombaPoziciok = new List<Point>();

		int lepesekSzama = 0;
		int nehezsegiSzint = 1;            // 1: Bastya, 2: Huszar, 3: Vezer

		Random veletlen = new Random();

		// Tabla szinei
		Brush vilagosMezo = new SolidColorBrush(Color.FromRgb(181, 216, 168));
		Brush sotetMezo = new SolidColorBrush(Color.FromRgb(92, 138, 92));

		public MainWindow()
		{
			InitializeComponent();
			TablaLetrehoz();
			UjJatekIndit();
		}

		void TablaLetrehoz()
		{
			Tabla.Children.Clear();

			for (int y = 0; y < TABLA_MERET; y++)
			{
				for (int x = 0; x < TABLA_MERET; x++)
				{
					Button mezo = new Button
					{
						Tag = new Point(x, y),
						Background = (x + y) % 2 == 0 ? vilagosMezo : sotetMezo,
						BorderThickness = new Thickness(0)
					};

					mezo.Click += MezoKattintas;

					tablaMezok[x, y] = mezo;
					Tabla.Children.Add(mezo);
				}
			}
		}

		void UjJatekIndit()
		{
			// Figura kezdo pozicio
			figuraPozicio = new Point(
				veletlen.Next(TABLA_MERET),
				veletlen.Next(TABLA_MERET)
			);

			// 5 gomba elhelyezese
			gombaPoziciok.Clear();

			while (gombaPoziciok.Count < 5)
			{
				Point ujGomba = new Point(
					veletlen.Next(TABLA_MERET),
					veletlen.Next(TABLA_MERET)
				);

				if (!gombaPoziciok.Contains(ujGomba) && ujGomba != figuraPozicio)
				{
					gombaPoziciok.Add(ujGomba);
				}
			}

			lepesekSzama = 0;
			FrissitTabla();
		}

		void MezoKattintas(object sender, RoutedEventArgs e)
		{
			Button kattintottMezo = sender as Button;
			Point celPozicio = (Point)kattintottMezo.Tag;

			if (SzabalyosLepes(celPozicio))
			{
				figuraPozicio = celPozicio;
				lepesekSzama++;

				// Gomba felszedese
				for (int i = gombaPoziciok.Count - 1; i >= 0; i--)
				{
					if (gombaPoziciok[i] == figuraPozicio)
					{
						gombaPoziciok.RemoveAt(i);
						break;
					}
				}

				if (gombaPoziciok.Count == 0)
				{
					MessageBox.Show(
						$"Nehezsegi Szint: {nehezsegiSzint} | Vege a jateknak! Lepesek: {lepesekSzama}"
					);

					nehezsegiSzint++;
					if (nehezsegiSzint > 3)
						nehezsegiSzint = 1;

					UjJatekIndit();
				}
				else
				{
					FrissitTabla();
				}
			}
		}

		bool SzabalyosLepes(Point cel)
		{
			int dx = (int)Math.Abs(cel.X - figuraPozicio.X);
			int dy = (int)Math.Abs(cel.Y - figuraPozicio.Y);

			switch (nehezsegiSzint)
			{
				case 1: // Bastya
					return cel.X == figuraPozicio.X || cel.Y == figuraPozicio.Y;

				case 2: // Huszar
					return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);

				case 3: // Vezer
					return cel.X == figuraPozicio.X ||
						   cel.Y == figuraPozicio.Y ||
						   dx == dy;

				default:
					return false;
			}
		}

		void FrissitTabla()
		{
			// Tabla torlese
			for (int y = 0; y < TABLA_MERET; y++)
			{
				for (int x = 0; x < TABLA_MERET; x++)
				{
					tablaMezok[x, y].Content = null;
					tablaMezok[x, y].IsEnabled = true;
				}
			}

			// Figura kep kivalsztasa szint szerint
			string kepNev =
				nehezsegiSzint == 1 ? "feher.png" :
				nehezsegiSzint == 2 ? "knight.png" :
									  "queen.png";

			tablaMezok[(int)figuraPozicio.X, (int)figuraPozicio.Y].Content =
				KepBetolt("Images/" + kepNev);

			// Gombak kirajzolasa
			foreach (Point gomba in gombaPoziciok)
			{
				tablaMezok[(int)gomba.X, (int)gomba.Y].Content =
					KepBetolt("Images/mushroom.png");
			}

			// Informacios szoveg
			InfoSzoveg.Text =
				$"Nehezsegi Szint: {nehezsegiSzint} | Lepesek: {lepesekSzama} | Gombak Szama: {gombaPoziciok.Count}/5";
		}

		Image KepBetolt(string eleresiUt)
		{
			return new Image
			{
				Source = new BitmapImage(new Uri(eleresiUt, UriKind.Relative)),
				Stretch = Stretch.Uniform
			};
		}

		void UjJatek_Click(object sender, RoutedEventArgs e)
		{
			UjJatekIndit();
		}
	}
}
