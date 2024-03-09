using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleriaDeFotosServer.Models.DTOs
{
    public class ImageDTO
    {
        public string Img { get; set; } = "";
        public string NombreUser { get; set; } = "";
        public bool Estado { get; set; }
    }
}
