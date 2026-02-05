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

		const int MERET = 8;

		Button[,] tabla = new Button[MERET, MERET];

		Point babu;                 // figura pozicioja

		List<Point> gombak = new List<Point>();

		int lepesSzam = 0;

		int szint = 1;              // 1: Bastya, 2: Huszar, 3: Vezer

		Random rnd = new Random();

		// Tabla szinei

		Brush vilagosMezo = new SolidColorBrush(Color.FromRgb(181, 216, 168));

		Brush sotetMezo = new SolidColorBrush(Color.FromRgb(92, 138, 92));

		public MainWindow()

		{

			InitializeComponent();

			TablaLetrehoz();

			UjJatek();

		}

		void TablaLetrehoz()

		{

			Tabla.Children.Clear();

			for (int y = 0; y < MERET; y++)

			{

				for (int x = 0; x < MERET; x++)

				{

					Button b = new Button();

					b.Tag = new Point(x, y);

					b.Click += Mezo_Click;

					// Sakk tabla zold szinekkel

					b.Background = (x + y) % 2 == 0 ? vilagosMezo : sotetMezo;

					// Gomb keret eltuntetese

					b.BorderThickness = new Thickness(0);

					tabla[x, y] = b;

					Tabla.Children.Add(b);

				}

			}

		}

		void UjJatek()

		{

			// Figura kezdopozicio

			babu = new Point(rnd.Next(MERET), rnd.Next(MERET));

			// 5 gomba elhelyezese

			gombak.Clear();

			while (gombak.Count < 5)

			{

				Point g = new Point(rnd.Next(MERET), rnd.Next(MERET));

				if (!gombak.Contains(g) && g != babu)

					gombak.Add(g);

			}

			lepesSzam = 0;

			Frissit();

		}

		void Mezo_Click(object sender, RoutedEventArgs e)

		{

			Button b = sender as Button;

			Point cel = (Point)b.Tag;

			if (SzabalyosLepes(cel))

			{

				babu = cel;

				lepesSzam++;

				// Gomba felszedese

				for (int i = gombak.Count - 1; i >= 0; i--)

				{

					if (gombak[i] == babu)

					{

						gombak.RemoveAt(i);

						break;

					}

				}

				if (gombak.Count == 0)

				{

					MessageBox.Show($"Nehézségi Szint: {szint} Vége a játéknak! Lépések: {lepesSzam}");

					szint++;

					if (szint > 3) szint = 1;

					UjJatek();

				}

				else

				{

					Frissit();

				}

			}

		}

		bool SzabalyosLepes(Point cel)

		{

			int dx = (int)Math.Abs(cel.X - babu.X);

			int dy = (int)Math.Abs(cel.Y - babu.Y);

			switch (szint)

			{

				case 1: // Bastya

					return cel.X == babu.X || cel.Y == babu.Y;

				case 2: // Huszar

					return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);

				case 3: // Vezer

					return cel.X == babu.X || cel.Y == babu.Y || dx == dy;

				default:

					return false;

			}

		}

		void Frissit()

		{

			// Tabla torles

			for (int y = 0; y < MERET; y++)

			{

				for (int x = 0; x < MERET; x++)

				{

					tabla[x, y].Content = null;

					tabla[x, y].IsEnabled = true;

				}

			}

			// Figura kep szint szerint

			string kepnev =

				szint == 1 ? "feher.png" :

				szint == 2 ? "knight.png" :

							 "queen.png";

			tabla[(int)babu.X, (int)babu.Y].Content = Kep("Images/" + kepnev);

			// Gombak kirajzolasa

			foreach (Point g in gombak)

			{

				tabla[(int)g.X, (int)g.Y].Content = Kep("Images/mushroom.png");

			}

			// Informacios szoveg

			InfoSzoveg.Text =

				$"Nehézségi Szint: {szint} | Lépések: {lepesSzam} |  Gombák Száma: {gombak.Count}/5";

		}

		Image Kep(string ut)

		{

			return new Image

			{

				Source = new BitmapImage(new Uri(ut, UriKind.Relative)),

				Stretch = Stretch.Uniform

			};

		}

		void UjJatek_Click(object sender, RoutedEventArgs e)

		{

			UjJatek();

		}

	}

}

