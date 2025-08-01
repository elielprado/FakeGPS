using System;
using System.Globalization;
using System.Windows.Forms;
using FakeGPS.Common;

namespace FakeGPS
{
    public partial class FakeGPSForm : Form
    {
        public FakeGPSForm()
        {
            InitializeComponent();
        }

        // Detects whether the system language is Portuguese
        private bool IsPortuguese()
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            return culture.Equals("pt", StringComparison.InvariantCultureIgnoreCase);
        }

        private void FakeGPSForm_Load(object sender, EventArgs e)
        {
            // Sets interface text based on system language (Portuguese or English)
            if (IsPortuguese())
            {
                lblLatitude.Text = "Latitude:";
                lblLongitude.Text = "Longitude:";
                btnLoad.Text = "Carregar Localização";
                btnSave.Text = "Salvar Localização";
                this.Text = "FakeGPS - Localização";
            }
            else
            {
                lblLatitude.Text = "Latitude:";
                lblLongitude.Text = "Longitude:";
                btnLoad.Text = "Load Location";
                btnSave.Text = "Set Location";
                this.Text = "FakeGPS - Location";
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                var latLong = RegistryHelper.GetLatLong();
                txtLatitude.Text = latLong.Latitude.ToString("F5", CultureInfo.InvariantCulture);
                txtLongitude.Text = latLong.Longitude.ToString("F5", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading current location: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string latitudeText = txtLatitude.Text.Replace(",", ".");
                string longitudeText = txtLongitude.Text.Replace(",", ".");

                double latitude = Math.Round(double.Parse(latitudeText, CultureInfo.InvariantCulture), 5);
                double longitude = Math.Round(double.Parse(longitudeText, CultureInfo.InvariantCulture), 5);

                RegistryHelper.SetLatLong(new LatLong { Latitude = latitude, Longitude = longitude });

                MessageBox.Show("The location has been set!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting location: {ex.Message}");
            }
        }
    }
}
