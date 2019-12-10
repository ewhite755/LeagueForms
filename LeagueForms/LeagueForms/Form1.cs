using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel;

namespace LeagueForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void apiKey_TextChanged(object sender, EventArgs e)
        {
            startButton.Enabled = true;
        }

        private async void startButton_ClickAsync(object sender, EventArgs e)
        {
            var riotApi = RiotApi.NewInstance(apiKey.Text);
            var summonerNameQuery = usernameText.Text;
            var summonerData = await riotApi.SummonerV4.GetBySummonerNameAsync(MingweiSamuel.Camille.Enums.Region.NA, summonerNameQuery);
            if (null == summonerData)
            {
                listBox1.Items.Add("No Summoner Found");
                return;
            }


        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
