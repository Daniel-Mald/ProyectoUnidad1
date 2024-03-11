using CommunityToolkit.Mvvm.Input;
using GaleriaDeFotosCliente.Models.DTOs;
using GaleriaDeFotosCliente.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using GaleriaDeFotosCliente.Models;


namespace GaleriaDeFotosCliente.ViewModels
{
    public class FotosViewModel : INotifyPropertyChanged
    {
        FotosClient cliente = new();
        public string Imagen {  get; set; } //Ruta de la imagen de la pc, ejemplo: C:\Users\Alka\Pictures\1.jpg
        //public System.Drawing.Image Imagencion { get; set; }
        public string IP { get; set; } = ""; 
        public bool Conectado { get; set; }

        public ICommand ConectarCommand { get; set; }
        public ICommand DesconectarCommand { get; set; }
        public ICommand CargarFotoCommand { get; set; }
        public ICommand EnviarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }

        
        //public ObservableCollection<ImageDTO> ListaImgs { get; set; } = new();
        public ObservableCollection<ListaModel> ListaImagenes { get; set; } = new();//puse esta lista en vez de la otra

        public FotosViewModel()
        {
            //cliente.ImagenRecibida += Cliente_ImagenRecibida;
            EnviarCommand = new RelayCommand(Enviar);
            ConectarCommand = new RelayCommand(Conectar);
            DesconectarCommand = new RelayCommand(Desconectar);
            EliminarCommand = new RelayCommand<ListaModel>(Eliminar); //Enviar como parametro el string "img" de ImageDTO
            CargarFotoCommand = new RelayCommand(CargarFoto);
            cliente.ImagenRecibida += Cliente_ImagenRecibida1;
        }
        
        private void CargarFoto()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {

               Imagen = openFileDialog.FileName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Imagen)));
            }

        }

        private void Cliente_ImagenRecibida1(object? sender, ImageDTO e)//aqui es donde carga las imagens que me entrego es sserver
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                

                    byte[] bytes = Convert.FromBase64String(e.Img);
                
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {

                        BitmapImage imagen = new BitmapImage();
                        imagen.BeginInit();
                        imagen.StreamSource = ms;
                        imagen.CacheOption = BitmapCacheOption.OnLoad;
                        //imagen.CacheOption = BitmapCacheOption.OnLoad;
                        
                         imagen.EndInit();


                   ListaModel item = new ListaModel
                    {
                        id = e.ImagenId,
                        Usuario = cliente.Equipo,
                        Imagen = imagen
                    };

                    ListaImagenes.Add(item);
                    }
                    
                
                  
                
            });
        }
        void CargarFotos()
        {

        }
        private void Desconectar()
        {
            cliente.Desconectar();
        }

        private void Eliminar(ListaModel l) //no entra el comando de este metodo, le puse otro modelo que hice para pasar la info, al final si es un ImageDTO
        {
            if (l != null)
            {
               
                byte[] bytesImg = File.ReadAllBytes(l.Imagen.ToString());
                ImageDTO img = new()
                {
                    ImagenId = l.id,
                    Estado = false,
                    NombreUser = cliente.Equipo,
                    Img = Convert.ToBase64String(bytesImg),
                };
                cliente.EnviarIMG(img);
            }
        }

        private void Conectar()
        {
            IPAddress.TryParse(IP, out IPAddress? ipAddress);
            if(ipAddress != null)
            {
                cliente.Conectar(ipAddress);
                Conectado = true;
                PropertyChanged?.Invoke(this, new(nameof(Conectado)));
            }
        }

        private void Enviar()
        {
            if (!string.IsNullOrWhiteSpace(Imagen) && Conectado)
            {
               
                byte[] bytesImg = File.ReadAllBytes(Imagen);

                ImageDTO img = new()
                {
                    Estado = true,
                    NombreUser = cliente.Equipo,
                    Img = Convert.ToBase64String(bytesImg)
                };
                
                cliente.EnviarIMG(img);
                Imagen = "";
            }
        }


        

        


        public event PropertyChangedEventHandler? PropertyChanged;

    }
}
