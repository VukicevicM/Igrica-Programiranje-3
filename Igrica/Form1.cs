using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KnjiznicaMajcaPustolovscina;

namespace Igrica
{
    public partial class Form1 : Form
    {
        Macka macka;
        List<Blok> bloki = new List<Blok>();
        Timer timer = new Timer();
        bool levo, desno;

        // Ozadje se premika, ko mačka prečka sredino
        int premikOzadja = 0;
        int mejaKamere = 400; // polovica širine okna

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = 800;
            this.Height = 600;

            // Inicializiramo mačko nad tlemi
            macka = new Macka(100, 400, 40);

            // Dodamo tla kot blok
            bloki.Add(new Blok(0, 500, 800, 100));

            // Dodamo nekaj platform za skakanje
            bloki.Add(new Blok(300, 400, 100, 20));
            bloki.Add(new Blok(500, 350, 100, 20));
            bloki.Add(new Blok(700, 300, 100, 20));
            bloki.Add(new Blok(900, 250, 100, 20));
            bloki.Add(new Blok(1100, 200, 100, 20));

            // Nastavimo timer za glavno zanko igre
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            timer.Start();

            // Nastavimo dogodke za tipkovnico in risanje
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.Paint += Form1_Paint;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Mačka se premika levo/desno
            if (levo) macka.PremakniLevo();
            if (desno) macka.PremakniDesno();

            // Kamera se premakne, če mačka prečka polovico zaslona v desno
            if (macka.Obmocje.X > mejaKamere)
            {
                int dx = macka.Obmocje.X - mejaKamere;
                macka.Obmocje.X = mejaKamere;
                premikOzadja -= dx;
                foreach (var b in bloki)
                    b.Obmocje.X -= dx;
            }
            // Kamera se premakne, če mačka gre levo mimo meje kamere
            else if (macka.Obmocje.X < 100 && premikOzadja < 0)
            {
                int dx = 100 - macka.Obmocje.X;
                macka.Obmocje.X = 100;
                premikOzadja += dx;
                foreach (var b in bloki)
                    b.Obmocje.X += dx;
            }

            // Posodobimo pozicijo glede na gravitacijo
            macka.Posodobi();

            // Detekcija trka z bloki
            macka.NaTleh = false;
            foreach (var blok in bloki)
            {
                Rectangle b = blok.Obmocje;
                Rectangle m = macka.Obmocje;

                // Trk od zgoraj (padanje na platformo)
                if (m.IntersectsWith(b) && m.Bottom <= b.Top + macka.HitrostY && macka.HitrostY >= 0)
                {
                    macka.Obmocje.Y = b.Top - m.Height;
                    macka.HitrostY = 0;
                    macka.NaTleh = true;
                }
                // Trk s spodnje strani (udarjanje v spodnji del bloka)
                else if (m.IntersectsWith(b) && m.Top >= b.Bottom - macka.HitrostY && macka.HitrostY < 0)
                {
                    macka.Obmocje.Y = b.Bottom;
                    macka.HitrostY = 0; // začne padati
                }
                // Trk z leve ali desne (vodoravna blokada)
                else if (m.IntersectsWith(b))
                {
                    if (macka.Obmocje.X < b.X)
                        macka.Obmocje.X = b.X - m.Width;
                    else if (macka.Obmocje.X > b.X)
                        macka.Obmocje.X = b.Right;
                }
            }

            // Preverimo če je mačka padla izven zaslona
            if (macka.Obmocje.Y > this.Height)
            {
                timer.Stop();
                MessageBox.Show("Mačka je padla! Konec igre.");
                Application.Exit();
            }

            Invalidate(); // Povzroči ponovno risanje
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Ponovi nebo kot ozadje (v prihodnje lahko nadomestiš s sliko ozadja)
            g.Clear(Color.LightSkyBlue);

            // Nariši mačko (trenutno oranžen krog)
            macka.Narisi(g);

            // Nariši vse platforme
            foreach (var b in bloki)
                g.FillRectangle(Brushes.Green, b.Obmocje);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) levo = true;
            if (e.KeyCode == Keys.Right) desno = true;
            if (e.KeyCode == Keys.Up) macka.Skoci();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) levo = false;
            if (e.KeyCode == Keys.Right) desno = false;
        }
    }
}
