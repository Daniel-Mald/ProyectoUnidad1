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
        string rutaImagenes = @"..\Imagenes\";
        public GaleriaServer _server { get; set; } = new();
        public ObservableCollection<BitmapImage> Imagenes2 { get; set; } = new();
        [ObservableProperty]
        public BitmapImage ultimaImg;

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
            
            
        }

        public void _server_FotoRecibida(object? sender, ImageDTO e)
        {
            
            if(e.Estado == true)
            {
                //guardar
                byte[] bytes = Convert.FromBase64String(e.Img);
                Image _image;
                int n;
                if (!Directory.Exists($"{rutaImagenes}{e.NombreUser}"))
                {
                    Directory.CreateDirectory($"{rutaImagenes}{e.NombreUser}");
                }
                n = Directory.GetFiles($"{rutaImagenes}{e.NombreUser}").Count();
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    _image = Image.FromStream(ms);
                    _image.Save($"{rutaImagenes}{e.NombreUser}/{n+1}.jpg");


                }

                var imagenN =PasarABitMapImage($"{rutaImagenes}{e.NombreUser}/{n+1}.jpg");
                UltimaImg = imagenN;
                Imagenes2.Add(imagenN);
            }
            else
            {
                //eliminar
                try
                {
                    string ruta = $"{rutaImagenes}{e.NombreUser}.jpg";
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
        }

        [RelayCommand]
        private void IniciarServer()
        {
            _server.IniciarServer();
            CargarImagenes();
            if(UltimaImg != null)
            UltimaImg = Imagenes2.Last();
        }
        [RelayCommand]
        void DetenerServidor()
        {
            _server.CerrarServer();
            Imagenes2.Clear();

        }


        void CargarImagenes()
        {
            
            if (!Directory.Exists(rutaImagenes))
            {
                Directory.CreateDirectory (rutaImagenes);
            }

            string[]? carpetas = Directory.GetDirectories(rutaImagenes);
           
            foreach (var item in carpetas)
            {
                var x = Directory.GetFiles(item);
                var y = Directory.GetFiles(item, "*jpg");
                foreach (var j in y)
                {
                   
                   Imagenes2.Add( PasarABitMapImage(j));
                    
                }
            }
            

        }
        BitmapImage PasarABitMapImage(string ruta)//este metodo solo es para que haga las imagenes abitmap para la vista
        {
            BitmapImage imagen = new BitmapImage();
            imagen.BeginInit();
            imagen.UriSource = new Uri(ruta, UriKind.Relative);
            imagen.CacheOption = BitmapCacheOption.OnLoad;
          
            imagen.EndInit();

            return imagen;
            
        }
    }
}
