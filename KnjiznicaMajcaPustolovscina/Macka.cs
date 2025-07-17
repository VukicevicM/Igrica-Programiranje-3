using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
 

namespace KnjiznicaMajcaPustolovscina
{
    public class Macka
    {
        // Pravokotnik, ki opisuje položaj in velikost mačke
        public Rectangle Obmocje;

        // Navpična hitrost zaradi gravitacije
        public int HitrostY = 0;

        // Ali je mačka trenutno na tleh
        public bool NaTleh = false;

        // Hitrost vodoravnega premikanja
        public int HitrostPremika = 5;

        // Moč skoka (višja vrednost pomeni višji skok)
        public int SkokMoc = 15;

        // Pripravljen prostor za animacijo
        private Image[] slikeHoje;
        private Image slikaStoji;
        private int trenutniFrame = 0;
        private int animCounter = 0;

        // Ali se trenutno premika
        public bool SePremika = false;

        public Macka(int x, int y, int velikost)
        {
            Obmocje = new Rectangle(x, y, velikost, velikost);

            // 🖼️ Tukaj lahko enkrat pozneje naložiš slike:
            slikeHoje = new Image[] {
                 Image.FromFile("Hoja1.png"),
                 Image.FromFile("Hoja2.png"),
                 Image.FromFile("Hoja3.png"),
                 Image.FromFile("Hoja4.png"),
                 Image.FromFile("Hoja5.png"),
                 Image.FromFile("Hoja6.png"),
                 Image.FromFile("Hoja7.png"),
                 Image.FromFile("Hoja8.png"),
                 Image.FromFile("Hoja9.png"),
                 Image.FromFile("Hoja10.png"),
                 Image.FromFile("Hoja11.png"),
                 Image.FromFile("Hoja12.png")

             };
            //
            // slikaStoji = Image.FromFile(\"idle.png\");
        }

        // Premik levo
        public void PremakniLevo()
        {
            Obmocje.X -= HitrostPremika;
            SePremika = true;
        }

        // Premik desno
        public void PremakniDesno()
        {
            Obmocje.X += HitrostPremika;
            SePremika = true;
        }

        // Skoči, če je na tleh
        public void Skoci()
        {
            if (NaTleh)
            {
                HitrostY = -SkokMoc;
                NaTleh = false;
            }
        }

        // Posodobi padanje (gravitacija)
        public void Posodobi()
        {
            HitrostY += 1;
            Obmocje.Y += HitrostY;

            // Štejemo animacijo samo če se premika
            if (SePremika && slikeHoje != null && slikeHoje.Length > 0)
            {
                animCounter++;
                if (animCounter % 5 == 0)
                    trenutniFrame = (trenutniFrame + 1) % slikeHoje.Length;
            }
        }

        // Nariši mačko (z animacijo ali kot oranžen krog)
        public void Narisi(Graphics g)
        {
            if (slikeHoje != null && slikeHoje.Length > 0 && SePremika)
            {
                g.DrawImage(slikeHoje[trenutniFrame], Obmocje);
            }
            else if (slikaStoji != null)
            {
                g.DrawImage(slikaStoji, Obmocje);
            }
            else
            {
                // Privzeta oblika – oranžen krog
                g.FillEllipse(Brushes.Orange, Obmocje);
            }

            // Reset premika po risanju
            SePremika = false;
        }

        // (Dodatno: nastavitev slik, če želiš ločeno naložiti)
        public void NastaviAnimacije(Image[] hoje, Image stoji)
        {
            slikeHoje = hoje;
            slikaStoji = stoji;
        }
    }


    // Razred za vse vrste blokov (npr. tla, platforme)
    public class Blok
    {
        public Rectangle Obmocje;

        public Blok(int x, int y, int sirina, int visina)
        {
            Obmocje = new Rectangle(x, y, sirina, visina);
        }
    }


    // Prepreke – lahko razširiš za posebna pravila (npr. nevarnosti)
    public class Prepreka
    {
        public Rectangle Obmocje;

        public Prepreka(int x, int y, int sirina, int visina)
        {
            Obmocje = new Rectangle(x, y, sirina, visina);
        }
    }


    /// <summary>
    /// Predstavlja ribico, ki jo mačka lahko pobere.
    /// </summary>
    public class Ribica
    {
        // Pravokotnik, ki določa pozicijo in velikost ribice
        public Rectangle Obmocje;

        // Ali je ribica že zbrana (ne rišemo je več)
        public bool Zbrana = false;

        // Privzeta velikost ribice (lahko tudi spremeniš po želji)
        private static readonly Size velikost = new Size(30, 30);

        public Ribica(int x, int y)
        {
            Obmocje = new Rectangle(new Point(x, y), velikost);
        }

        // Nariše ribico, če še ni zbrana
        public void Narisi(Graphics g)
        {
            if (!Zbrana)
            {
                g.FillEllipse(Brushes.Gold, Obmocje);
                using (Font font = new Font("Arial", 8))
                {
                    g.DrawString("riba", font, Brushes.Black, Obmocje.X + 4, Obmocje.Y + 10);
                }
            }
        }

        // Preveri, če jo je mačka pobrala
        public void PreveriZbiranje(Rectangle mackinObmocje)
        {
            if (!Zbrana && Obmocje.IntersectsWith(mackinObmocje))
            {
                Zbrana = true;
                // (Po želji: lahko sprožiš dogodek za povečanje točk)
            }
        }
    }


}
