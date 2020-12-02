using System;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ifTurnOnPC
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = Environment.GetCommandLineArgs().First();
            string fileName = Path.GetFileName(filePath);

            Console.Title = fileName;

            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            key.SetValue(fileName, filePath);

            Timer timer = new Timer((o) =>
            {
                if (!IsConnectedToInternet())
                {
                    return;
                }

                try
                {
                    TwilioClient.Init("sid", "token");

                    MessageResource.Create(
                        to: new PhoneNumber(""),
                        from: new PhoneNumber(""),
                        body: "Turn on your PC.");
                }
                catch { }

                GC.Collect();
            }, null, 0, 2000);

            Console.ReadLine();
        }

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }
    }
}
