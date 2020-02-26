using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

namespace Assets.Scripts.Screeps3D
{
    public class TwitchClient : BaseSingleton<TwitchClient>
    {
        private const string PP_TWITCH_CHANNEL = "twitch:channel";
        private const string PP_TWITCH_TOKEN = "twitch:bot:token";
        private const string PP_TWITCH_USERNAME = "twitch:bot:username";

        public Client client;
        private string channel_name = "thmsndk"; // TODO: configurable

        public event EventHandler<string> OnGoToRoom;

        private void Start()
        {
            // Script should be running always
            Application.runInBackground = true;

            // TODO: UI for tokens / settings and such
            var channel = PlayerPrefs.GetString(PP_TWITCH_CHANNEL, string.Empty);
            var accessToken = PlayerPrefs.GetString(PP_TWITCH_TOKEN, string.Empty);
            var botTwitchUsername = PlayerPrefs.GetString(PP_TWITCH_USERNAME, string.Empty); // "Screeps3D"

            if (string.IsNullOrEmpty(accessToken))
            {
                // TODO: UI
                PlayerInput.Get("channel:bot_access_token:bot_username", (string input) =>
                {
                    var values = input.Split(':');
                    channel = values[0];
                    PlayerPrefs.SetString(PP_TWITCH_CHANNEL, channel);

                    accessToken = values[1];
                    PlayerPrefs.SetString(PP_TWITCH_TOKEN, accessToken);

                    botTwitchUsername = values[2];
                    PlayerPrefs.SetString(PP_TWITCH_USERNAME, botTwitchUsername);

                    InitializeAndConnect(channel, accessToken, botTwitchUsername);
                });
            }
            else
            {
                InitializeAndConnect(channel, accessToken, botTwitchUsername);
            }

        }

        public void SendMessage(string message)
        {
            client.SendMessage(client.JoinedChannels[0], message);
        }

        private void InitializeAndConnect(string channel, string accessToken, string username)
        {
            this.channel_name = channel;
            var credentials = new ConnectionCredentials(username, accessToken);
            client = new Client();
            client.Initialize(credentials, channel_name);

            client.OnChatCommandReceived += Client_OnChatCommandReceived;

            client.OnMessageReceived += Client_OnMessageReceived;

            client.Connect();
        }

        /*
         * You should also be aware of the restrictions of a bot. To prevent from spam bots and bot misuse, 
         * Twitch have placed restrictions on how many messages can be sent every 30 seconds, how many whisper can be sent, and how many new users can be whispered by a bot. 
         * You can put a request in to Twitch to have your bot upgraded to a “known bot” status that will improve these limits somewhat. 
         * One thing that can sometimes help viewer receive whispers from the bot is if the viewer follows the bot and/or whispers the bot first. 

         */

        private void Client_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            Debug.Log($"{e.Command.CommandText}: {e.Command.ArgumentsAsString}");
            // TODO: !help with a list of commands
            // !info that spits out a link to the event page.
            // allow moderators to !setinfo and persist link, could be link to info page :thinking: // can probably jsut hardcode a list of users for start.
            switch (e.Command.CommandText)
            {
                case "help":
                    client.SendMessage(client.JoinedChannels[0], "TODO: add a fancy help/command system :P !goto ");
                    break;
                case "goto":
                    // Raise goto event, and let RoomChooser listen for it.
                    // TODO: validate we are actually connected to the server :smirk:
                    var arguments = e.Command.ArgumentsAsList;
                    if (arguments.Count > 0)
                    {
                        var userName = e.Command.ChatMessage.Username;
                        var roomName = arguments[0];
                        NotifyText.Message($"{userName} told me to go to {roomName}", UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
                        OnGoToRoom?.Invoke(this, roomName);
                    }
                    break;
                case "server":
                    // tell what server we are on
                    break;
                case "tick":
                    // tell what tick we are on? :thinking:
                    break;
                case "connect":
                    // press the connect button in case the stream has disconnected.
                    break;
                case "warpath":
                    // message the chat with pvp battles / conflicts should this perhaps be something running on a timer?
                    //Apparently loan is utilized https://www.leagueofautomatednations.com/vk/battles.json
                    
                    break;
                case "pvp":
                    // message the chat with pvp battles from the experimental endpoint.

                    break;
                case "rcl":
                    // List combined RCL rankings.
                    break;
                case "nextbattle":
                    // goes to the next "interesting" battle, e.g. classifications from warpath.

                    break;
                default:
                    break;
                    // send a random message at a timer mentioning the channel and the warpath channels
            }
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Debug.Log($"{e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                client.SendMessage(client.JoinedChannels[0], "This is a test message from the bot / 3D client");
            }
        }
        // TODO: a GUI to configure your twitch settings

    }
}
