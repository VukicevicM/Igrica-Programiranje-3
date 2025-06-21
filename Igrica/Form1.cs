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
            // Premikanje levo/desno
            if (levo) macka.PremakniLevo();
            if (desno) macka.PremakniDesno();

            // Posodobimo pozicijo glede na gravitacijo
            macka.Posodobi();

            // Detekcija trka z bloki (npr. tla)
            macka.NaTleh = false;
            foreach (var blok in bloki)
            {
                Rectangle b = blok.Obmocje;
                Rectangle m = macka.Obmocje;

                // Trk od zgoraj: mačka ne sme pasti skozi tla
                if (m.IntersectsWith(b) && m.Bottom <= b.Top + 10 && macka.HitrostY >= 0)
                {
                    macka.Obmocje.Y = b.Top - m.Height;
                    macka.HitrostY = 0;
                    macka.NaTleh = true;
                }
            }

            Invalidate(); // Povzroči ponovno risanje
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.LightSkyBlue); // Ozadje

            // Narišemo mačko (krog ali slika)
            macka.Narisi(g);

            // Narišemo vse bloke (tla ipd.)
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
