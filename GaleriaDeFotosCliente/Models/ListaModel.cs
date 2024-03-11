using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GaleriaDeFotosCliente.Models
{
    public class ListaModel
    {
        public BitmapImage Imagen { get; set; } = null!;
        public string Usuario { get; set; } = "";
        public int id { get; set; }
    }
}
