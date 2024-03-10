using CommunityToolkit.Mvvm.ComponentModel;
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

//using System.Windows.Controls;
//using System.IO;

//using System.Windows.Controls;


namespace GaleriaDeFotosServer.ViewModels
{
    public partial class ServerViewModel:ObservableObject
    {
        public string IP { get; set; } = "0.0.0.0";
        public GaleriaServer _server { get; set; } = new();
        public ObservableCollection<Image> Imagenes { get; set; } = new();



        public ServerViewModel()
        {
            var direcciones = Dns.GetHostAddresses(Dns.GetHostName());
            if(direcciones!= null)
            {
                IP = string.Join(",", direcciones.
                    Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).
                    Select(x => x.ToString()));
            }
            _server.FotoRecibida += _server_FotoRecibida;
            CargarImagenes();
        }

        public void _server_FotoRecibida(object? sender, ImageDTO e)
        {
            
            if(e.Estado == true)
            {
                //guardar
                byte[] bytes = Convert.FromBase64String(e.Img);
                Image _image;
                int n = 2;
                if (!Directory.Exists($"imagenes/{e.NombreUser}"))
                {
                    Directory.CreateDirectory($"imagenes/{e.NombreUser}");
                }
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    _image = Image.FromStream(ms);
                    _image.Save($"Imagenes/{e.NombreUser}/{n}.jpg");


                }


                Imagenes.Add(_image);
            }
            else
            {
                //eliminar
                try
                {
                    string ruta = $"Imagenes/{e.NombreUser}.jpg";
                    if (System.IO.File.Exists(ruta))
                    {
                        System.IO.File.Delete(ruta);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                
                
            }
            //guardar la imagen
            //cargar la imagen a la lista
        }

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
            string[] fotos = Directory.GetFiles("Imagenes");
            foreach (var item in fotos)
            {   
                Imagenes.Add(Image.FromFile(item));
            }

        }
    }
}
