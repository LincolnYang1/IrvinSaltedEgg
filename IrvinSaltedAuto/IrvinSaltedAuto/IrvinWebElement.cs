using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrvinSaltedAuto
{
    public class IrvinWebElement
    {
        public static By GetPopupCheckoutPanel() => By.Id("checkout-wrapper");
        public static By GetSmallPotatoInput() => By.Name("products[2][quantity]");
        public static By GetBigPotatoInput() => By.Name("products[0][quantity]");
        public static By GetSmallFishSkinInput() => By.Name("products[3][quantity]");
        public static By GetBigFishSkinInput() => By.Name("products[1][quantity]");

        public static By GetSubmitButton() => By.Id("checkout-btn");

        public static By GetEmailAddressInput() => By.Name("billingProfile[email]");
        public static By GetNextBtn() => By.Id("account-information-btn");
        
        public static By GetPasswordInput() => By.Name("password");
        public static By GetLoginBtn() => By.Id("login-information-btn");
        
        public static By GetDatePicker() => By.Id("delivery-datepicker");

        public static DateTime[] GetAvailPickDates(IWebDriver web)
        {
            var month = web.FindUntilElementReady(By.ClassName("datepicker-switch")).Text;
            var container = web.FindUntilElementReady(By.ClassName("datepicker-days"));
            var disabledDays = container.FindElements(By.ClassName("disabled"));
            return container
                .FindElements(By.ClassName("day"))
                .Where(el => !disabledDays.Any(disabledEl => el.Equals(disabledEl))) 
                .Select(d => DateTime.Parse(d.Text + " " + month)).ToArray();
        }

        public static void SelectAddress(IWebDriver web)
        {
            var drp = web.FindElement(By.Id("shipping-profile-select"));
            var drpOpts = drp.FindElements(By.TagName("option"));
            if(drpOpts == null || drpOpts.Count < 2)
            {
                throw new Exception("账号没有创建地址");
            }

            var selEle = new SelectElement(drp);
            selEle.SelectByIndex(1);
        }

        public static By GetNext2Btn() => By.Id("customer-information-btn");

        public static By GetCODRadio() => By.Name("payment_method");
        public static By GetNext3Btn() => By.Id("payment-information-btn");
        public static By GetSubmitBtn() => By.Name("process");
    }
}
