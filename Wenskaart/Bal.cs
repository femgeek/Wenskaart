using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Wenskaart
{
    public class Bal
    {
        public SolidColorBrush Kleur { set; get; }
        public double BalPosX { set; get; }
        public double BalPosY { set; get; }

        public Bal(SolidColorBrush nKleur, double nBalPosX, double nBalPosY)
        {
            Kleur = nKleur;
            BalPosX = nBalPosX;
            BalPosY = nBalPosY;
        }
    }
}
