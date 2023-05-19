using Bogus;
using GeoJSON.Net.Geometry;
using MyLiteMafia.Common.Factories;
using MyLiteMafia.Common.Models;
using MyLiteMafia.DTOs;
using MyLiteMafia.Tile38Facade.Repositories;
using MyLiteMafia.Tile38Facade.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLiteMafia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int CanvasWidth { get { return (int)brdCanvas.Width; } }
        public int CanvasHeight { get { return (int)brdCanvas.Height; } }

        public ObservableCollection<DatagridItemDto> RegistredRivals = new ObservableCollection<DatagridItemDto>();
        public ObservableCollection<DatagridItemDto> RegistredEstablishments = new ObservableCollection<DatagridItemDto>();

        private readonly IRivalRepository _rivalRepository;
        private readonly IEstablishmentRepository _establishmentRepository;
        private readonly IGeofenceService _geofenceService;

        public MainWindow(IRivalRepository rivalRepository, IEstablishmentRepository establishmentRepository, IGeofenceService geofenceService)
        {
            InitializeComponent();
            _rivalRepository = rivalRepository;
            _establishmentRepository = establishmentRepository;
            _geofenceService = geofenceService;

            dgRivals.ItemsSource = RegistredRivals;
            dgEstablishments.ItemsSource = RegistredEstablishments;

            _geofenceService.GeofenceNotificationReceived += OnGeofenceNotificationReceived;
        }

        private void OnGeofenceNotificationReceived(object? sender, Common.Events.GeofenceNotificationEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtGeofenceLog.Text += e.Notification + Environment.NewLine;
            });
        }

        private async void btnAddRival_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCoordinates(txtLatitudeAdd.Text, txtLongitudeAdd.Text))
                return;

            var latitude = double.Parse(txtLatitudeAdd.Text, CultureInfo.InvariantCulture);
            var longitude = double.Parse(txtLongitudeAdd.Text, CultureInfo.InvariantCulture);

            var rival = new Rival(latitude, longitude);

            await StoreRivalAndUpdateUiAsync(rival);
        }

        private async Task StoreRivalAndUpdateUiAsync(Rival rival)
        {
            var tile38Data = await _rivalRepository.StoreAndGetDataAsync(rival);

            var point = ShapesConverter.Convert(rival.Coordinates);
            point.ToolTip = rival.RedisId;

            Dispatcher.Invoke(() =>
            {
                RegistredRivals.Add(new DatagridItemDto()
                {
                    RedisId = rival.RedisId,
                    Tile38Data = tile38Data,
                    CanvasItem = point
                });

                cnvMap.Children.Add(point);
            });
        }

        private bool ValidateCoordinates(string latitude, string longitude)
        {
            if (!double.TryParse(latitude, out _))
            {
                MessageBox.Show("Invalid latitude");
                return false;
            }

            if (!double.TryParse(longitude, out _))
            {
                MessageBox.Show("Invalid longitude");
                return false;
            }

            return true;
        }

        private bool ValidateRivalId()
        {
            if (!int.TryParse(txtRivalId.Text, out _))
            {
                MessageBox.Show("Invalid rival ID");
                return false;
            }

            return true;
        }

        private async void btnMoveRival_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCoordinates(txtLatitudeMove.Text, txtLongitudeMove.Text))
                return;

            if (!ValidateRivalId())
                return;

            var rivalId = int.Parse(txtRivalId.Text);

            var latitude = double.Parse(txtLatitudeMove.Text, CultureInfo.InvariantCulture);
            var longitude = double.Parse(txtLongitudeMove.Text, CultureInfo.InvariantCulture);

            var rival = new Rival(rivalId, latitude, longitude);

            var existingRival = RegistredRivals.First(x => x.RedisId == rivalId);
            Dispatcher.Invoke(() =>
            {
                cnvMap.Children.Remove(existingRival.CanvasItem);
                RegistredRivals.Remove(existingRival);
            });

            await StoreRivalAndUpdateUiAsync(rival);
        }

        private async void btnAddEstablishment_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            var faker = new Faker();
            for (int i = 0; i < 10; i++)
            {
                var establishment = await GenerateEstablishment(random, faker);
                await AddToGeofenceAsync(establishment);
            }
        }

        private async Task AddToGeofenceAsync(Establishment establishment)
        {
            var southwesternPoint = establishment.Polygon.Coordinates.First().Coordinates.First();
            var northeasternPoint = establishment.Polygon.Coordinates.First().Coordinates[2];

            await _geofenceService.CreateAndSubscribeGeofenceAsync(establishment.RedisId, southwesternPoint, northeasternPoint);
        }

        private async Task<Establishment> GenerateEstablishment(Random random, Faker faker)
        {
            var farthestAllowedLongitude = CanvasWidth - Establishment.Size;
            var intialLongitude = random.Next(0, farthestAllowedLongitude);

            var farthestAllowedLatitude = CanvasHeight - Establishment.Size;
            var initialLatitude = random.Next(0, farthestAllowedLatitude);

            var establishment = EstablishmentFactory.Create(initialLatitude, intialLongitude, faker.Company.CompanyName());
            var redisData = await _establishmentRepository.StoreAndGetDataAsync(establishment);

            var polygon = ShapesConverter.Convert(establishment.Polygon);
            polygon.ToolTip = CreateEstablishmentTooltip(establishment);

            AddEstablishmentToUI(establishment, redisData, polygon);

            return establishment;
        }

        private void AddEstablishmentToUI(Establishment establishment, string redisData, System.Windows.Shapes.Polygon polygon)
        {
            Dispatcher.Invoke(() =>
            {
                RegistredEstablishments.Add(new DatagridItemDto()
                {
                    Name = establishment.Name,
                    RedisId = establishment.RedisId,
                    Tile38Data = redisData
                });

                cnvMap.Children.Add(polygon);
            });
        }

        private string CreateEstablishmentTooltip(Establishment establishment)
        {
            var coordinates = establishment.Polygon.Coordinates.First();

            var initialLatitude = coordinates.Coordinates.First().Latitude;
            var initialLongitude = coordinates.Coordinates.First().Longitude;

            var toolTip = $"{establishment.Name}" + Environment.NewLine +
                $"[{initialLatitude},{initialLongitude}]" +
                $" - " +
                $"[{initialLatitude + Establishment.Size},{initialLongitude + Establishment.Size}]";

            return toolTip;
        }
    }
}
