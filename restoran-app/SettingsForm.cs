using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace restoran_app
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            // Load trendyol
            trendyolEmail.Text = Credentials.Default.trendyolEmail;
            trendyolSellerID.Text = Credentials.Default.trendyolSellerID;
            trendyolApiKey.Text = Credentials.Default.trendyolApiKey;
            trendyolApiSecret.Text = Credentials.Default.trendyolApiSecret;
            // Load yemeksepeti
            yemeksepetiEmail.Text = Credentials.Default.yemeksepetiEmail;
            yemeksepetiPassword.Text = Credentials.Default.yemeksepetiPassword;
            // Load getir
            getirRestaurantID.Text = Credentials.Default.getirRestaurantID;
            getirToken.Text = Credentials.Default.getirToken;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Save trendyol settings
            Credentials.Default.trendyolEmail = trendyolEmail.Text;
            Credentials.Default.trendyolSellerID = trendyolSellerID.Text;
            Credentials.Default.trendyolApiKey = trendyolApiKey.Text;
            Credentials.Default.trendyolApiSecret = trendyolApiSecret.Text;
            // Save yemeksepeti
            Credentials.Default.yemeksepetiEmail = yemeksepetiEmail.Text;
            Credentials.Default.yemeksepetiPassword = yemeksepetiPassword.Text;
            // Save getir
            Credentials.Default.getirRestaurantID = getirRestaurantID.Text;
            Credentials.Default.getirToken = getirToken.Text;
            // Save
            Credentials.Default.Save();
            // And close
            Close();
            // TODO: refresh clients
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Do nothing
            Close();
        }
    }
}
