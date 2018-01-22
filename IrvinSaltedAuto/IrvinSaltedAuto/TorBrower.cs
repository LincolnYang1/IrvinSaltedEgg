using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IrvinSaltedAuto
{
    
    public class TorBrower
    {
        private static object lockObj = new Object();
        private static Process TorProcess { get; set; }
        private string TorBinPath { get; set; }
        private IWebDriver Driver { get; set; }
        private WebDriverWait Wait { get; set; }

        public TorBrower(
            string torBinPath
            )
        {
            TorBinPath = torBinPath;
        }

        private void CreateProcess()
        {
            lock (lockObj)
            {
                if (TorProcess == null)
                {
                    TorProcess = new Process();
                    TorProcess.StartInfo.FileName = TorBinPath;
                    TorProcess.StartInfo.Arguments = "-n";
                    TorProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    TorProcess.Start();
                }
            }
        }
        public void Start()
        {
            CreateProcess();

            FirefoxProfile torProfile = new FirefoxProfile(@"C:\Users\LINCOLN\Desktop\Tor Browser\Browser\TorBrowser\Data\Browser\profile.default");
            FirefoxBinary binary = new FirefoxBinary(@"C:\Users\LINCOLN\Desktop\Tor Browser\Browser\firefox.exe");
            torProfile.SetPreference("webdriver.load.strategy", "unstable");
            //binary.StartProfile(torProfile);

            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("webdriver.load.strategy", "unstable");
            profile.SetPreference("network.proxy.type", 1);
            profile.SetPreference("network.proxy.socks", "127.0.0.1");
            profile.SetPreference("network.proxy.socks_port", 9150);

            profile.SetPreference("browser.privatebrowsing.autostart", true);
            profile.SetPreference("browser.cache.disk.enable", false);
            profile.SetPreference("browser.cache.memory.enable", false);
            profile.SetPreference("browser.cache.offline.enable", false);
            profile.SetPreference("network.http.use-cache", false);
            FirefoxOptions options = new FirefoxOptions();
            options.Profile = profile;
            options.BrowserExecutableLocation = @"C:\Users\LINCOLN\Desktop\Tor Browser\Browser\firefox.exe";
            Driver = new FirefoxDriver(options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(600);
        }

        public IEnumerable<UserAccountStatus> Order(UserAccount acct, EnumIrvinProduct prod)
        {
            Driver
                .Tee(web => web.Navigate().GoToUrl("https://irvinsaltedegg.com/"))
                .Tee(web =>
                {
                    for(int i = 0; i< 10; i++)
                    {
                        var readyFlag = web.FindUntilElementReady(IrvinWebElement.GetSmallPotatoInput());
                        if (readyFlag != null) break;
                    }
                })
                .Tee(web => web.FindUntilElementReady(IrvinWebElement.GetSmallPotatoInput()).ClearSendKeys("5"))
                .Tee(web => web.FindUntilElementReady(IrvinWebElement.GetSubmitButton()).Click()).Tee(w => Thread.Sleep(1000))
                .Tee(web => web.FindUntilElementReady(IrvinWebElement.GetPopupCheckoutPanel()))

                .Tee(web => web.FindUntilElementReady(IrvinWebElement.GetEmailAddressInput()).SendKeys(acct.Email))
                .Tee(web => web.FindUntilElementReady(IrvinWebElement.GetNextBtn()).Click()).Tee(w => Thread.Sleep(1000))
                .Tee(web => web.FindUntilElementReady(IrvinWebElement.GetPasswordInput()).SendKeys(acct.Password))
                .Tee(web => web.FindUntilElementReady(IrvinWebElement.GetLoginBtn()).Click()).Tee(w => Thread.Sleep(1000));

            Func<IWebElement> getDatePickerElement = ()=> Driver.FindUntilElementReady(IrvinWebElement.GetDatePicker());
            var availDates = getDatePickerElement().ClickWait(el =>
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("$('#delivery-datepicker').focus();$('#delivery-datepicker').focus();");
                return IrvinWebElement.GetAvailPickDates(Driver);
            });

            IrvinWebElement.SelectAddress(Driver);
            Thread.Sleep(5000);
            var selectedDate = DateTime.MinValue;

            foreach (var date in availDates)
            {
                getDatePickerElement().ClickWait(el => {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("$('#delivery-datepicker').focus();$('#delivery-datepicker').focus();");
                    var month = Driver.FindUntilElementReady(By.ClassName("datepicker-switch"));
                    return el.ClearSendKeys(date.ToString("yyyy-MM-dd"));
                });

                Driver.FindUntilElementReady(IrvinWebElement.GetNext2Btn()).Click();
                var errElement = Driver.GetElement(By.ClassName("has-error"));
                if (errElement == null)
                {
                    selectedDate = date;
                    break;
                }
                else
                {
                    yield return new UserAccountStatus {
                        UserAccount = acct,
                        BookDate = date,
                        HasError = true,
                        Status = "one per day limit",
                        OperationTimeStamp = DateTime.Now };
                }
            }
            
            if(selectedDate != DateTime.MinValue)
            {
                Thread.Sleep(5000);
                Driver.FindUntilElementReady(IrvinWebElement.GetCODRadio()).ClickWait(el => {
                    Thread.Sleep(5000);
                    return Driver.FindUntilElementReady(IrvinWebElement.GetNext3Btn());
                }).Click();

                Driver.FindUntilElementReady(IrvinWebElement.GetSubmitBtn()).Click();
                var errElement = Driver.GetElement(By.ClassName("has-error"));
                if (errElement == null)
                {
                    if(Driver.FindElement(By.ClassName("page-title")).Text == "THANK YOU FOR ORDERING!")
                    {
                        yield return new UserAccountStatus { UserAccount = acct, BookDate = selectedDate, HasError = false, OperationTimeStamp = DateTime.Now };
                    }
                }
            }
            
        }
    }
}
