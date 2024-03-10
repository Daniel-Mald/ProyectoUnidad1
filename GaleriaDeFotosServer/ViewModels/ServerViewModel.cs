﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GaleriaDeFotosServer.Models.DTOs;
using GaleriaDeFotosServer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

//using System.Windows.Controls;
//using System.IO;

//using System.Windows.Controls;


namespace GaleriaDeFotosServer.ViewModels
{
    public partial class ServerViewModel:ObservableObject
    {
        public string IP { get; set; } = "0.0.0.0";
        string rutaImagenes = "../Imagenes/";
        public GaleriaServer _server { get; set; } = new();
        public ObservableCollection<BitmapImage> Imagenes { get; set; } = new();
        //public ObservableCollection<string> Imagenes2 { get; set; } = new();



        public ServerViewModel()
        {
            var direcciones = Dns.GetHostAddresses(Dns.GetHostName());
            if(direcciones!= null)
            {
                IP = string.Join(",", direcciones.
                    Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).
                    Select(x => x.ToString()));
            }
           // _server.FotoRecibida += _server_FotoRecibida;
            
            CargarImagenes();
        }

        //public void _server_FotoRecibida(object? sender, ImageDTO e)
        //{
            
        //    if(e.Estado == true)
        //    {
        //        //guardar
        //        byte[] bytes = Convert.FromBase64String(e.Img);
        //        BitmapImage _image;
        //        int n;
        //        if (!Directory.Exists($"{rutaImagenes}{e.NombreUser}"))
        //        {
        //            Directory.CreateDirectory($"{rutaImagenes}{e.NombreUser}");
        //        }
        //        n = Directory.GetFiles($"{rutaImagenes}{e.NombreUser}").Count();
        //        using (MemoryStream ms = new MemoryStream(bytes))
        //        {
        //            _image = Image.FromStream(ms);
        //            _image.Save($"{rutaImagenes}{e.NombreUser}/{n+1}.jpg");


        //        }


        //        Imagenes.Add(_image);
        //    }
        //    else
        //    {
        //        //eliminar
        //        try
        //        {
        //            string ruta = $"{rutaImagenes}{e.NombreUser}.jpg";
        //            if (System.IO.File.Exists(ruta))
        //            {
        //                System.IO.File.Delete(ruta);
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
                
                
        //    }
        //    //guardar la imagen
        //    //cargar la imagen a la lista
        //}

        [RelayCommand]
        private void IniciarServer()
        {
            _server.IniciarServer();
        }
        [RelayCommand]
        void DetenerServidor()
        {
            _server.CerrarServer();
            Imagenes.Clear();

        }


        void CargarImagenes()
        {
            //tiene que ser de todas las carpetas
            //string rutaImagenes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Imagenes");
            //string[] archivos = Directory.GetFiles(rutaImagenes);

            string[]? carpetas = Directory.GetDirectories(rutaImagenes);
           // Directory.get
            foreach (var item in carpetas)
            {
                var x = Directory.GetFiles(item);
                foreach (var j in x)
                {
                    BitmapImage bitmapImage = new BitmapImage(new Uri(j));
                    Imagenes.Add(bitmapImage);
                   // Imagenes2.Add(j);
                }
            }
            

        }
    }
}
