using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrvinSaltedAuto
{
    class Program
    {
        static void Main(string[] args)
        {
            var torBinPath = @"C:\Users\LINCOLN\Desktop\Tor Browser\Browser\firefox.exe";
            var brower = new TorBrower(torBinPath);
            brower.Start();
            IEnumerable<UserAccount> userAccounts;
            using(var sr = new StreamReader("..\\..\\UserAccounts.csv"))
            {
                var reader = new CsvReader(sr);
                userAccounts = reader.GetRecords<UserAccount>().ToList();
            }
            
            using (var sw = new StreamWriter("..\\..\\UserAccountStatus.csv", true))
            {
                var writer = new CsvWriter(sw);
                foreach (var useracct in userAccounts)
                {
                    var status = brower.Order(useracct, EnumIrvinProduct.PotatoSmall).ToList();

                    writer.WriteRecords(status);
                    writer.Flush();
                }
            }
            
        }
    }
}
