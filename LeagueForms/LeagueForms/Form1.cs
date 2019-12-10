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

        public async void startButton_ClickAsync(object sender, EventArgs e)
        {
            var riotApi = RiotApi.NewInstance("RGAPI-92619da3-4664-432d-ad63-68ee984d8a43");
            var summonerNameQuery = usernameText.Text;
            var summonerData = await riotApi.SummonerV4.GetBySummonerNameAsync(MingweiSamuel.Camille.Enums.Region.NA, summonerNameQuery);
            if (null == summonerData)
            {
                textBox1.Text = ("No Summoner Found");
                return;
            }

            var matchlist = await riotApi.MatchV4.GetMatchlistAsync(MingweiSamuel.Camille.Enums.Region.NA, summonerData.AccountId, queue: new[] { 420 }, endIndex: 10);

            var matchDataTasks = matchlist.Matches.Select(
                   matchMetadata => riotApi.MatchV4.GetMatchAsync(MingweiSamuel.Camille.Enums.Region.NA, matchMetadata.GameId)
               ).ToArray();

            var matchDatas = await Task.WhenAll(matchDataTasks);
            float sum = 0;

            for (var i = 0; i < matchDatas.Count(); i++)
            {
                var matchData = matchDatas[i];

                var participantIdData = matchData.ParticipantIdentities.First(pi => summonerData.Id.Equals(pi.Player.SummonerId));

                var participant = matchData.Participants.First(p => p.ParticipantId == participantIdData.ParticipantId);

                var win = participant.Stats.Win;
                var champ = (Champion)participant.ChampionId;
                var k = participant.Stats.Kills;
                var d = participant.Stats.Deaths;
                var a = participant.Stats.Assists;
                var c = (participant.Stats.TotalMinionsKilled + participant.Stats.NeutralMinionsKilled);
                var g = matchData.GameDuration;
                TimeSpan t = TimeSpan.FromSeconds(matchData.GameDuration);
                string gametime = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
                var kda = (k + a) / (float)d;
                var cspm = (c / (float)g) * 60;


                textBox1.Text += String.Format("{0,3}) {1,-4} ({2}) Game Time: {3}\r\n K/D/A {4}/{5}/{6} ({7:0.00}) CS: {8} CSPM: {9:0.00}\r\n\r\n",
                    i + 1, win ? "Win" : "Loss", champ.Name(), gametime, k, d, a, kda, c, cspm);

                sum += cspm;
            textBox2.Text = (sum/10).ToString();
            }
        }
    }
}
