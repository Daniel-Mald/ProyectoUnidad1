﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GaleriaDeFotosCliente.Models.DTOs
{
    public class ImageDTO
    {
        public bool Estado {  get; set; }
        public string NombreUser { get; set; } = "";
        public string Img { get; set; } = "";
        public int ImagenId { get; set; }
    }
}
