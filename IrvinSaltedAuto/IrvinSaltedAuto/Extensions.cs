using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace IrvinSaltedAuto
{
    public static class Extensions
    {
        public static IWebElement GetElement(this IWebDriver web, By by)
        {
            try
            {
                return web.FindElement(by);
            }
            catch
            {
                return null;
            }
        }

        public static IWebElement ClearSendKeys(this IWebElement web, string val)
        {
            web.Clear();
            web.SendKeys(val);
            return web;
        }
        public static T ClickWait<T>(this IWebElement web, Func<IWebElement, T> act)
        {
            web.Click();
            return act(web);
        }
        public static void FindUntilElementInvisible(this IWebDriver web, By by) =>
            new WebDriverWait(web, TimeSpan.FromSeconds(30)).Until(ExpectedConditions.InvisibilityOfElementLocated(by));

        public static IWebElement FindUntilElementReady(this IWebDriver web, By by)
        {
            var wait = new WebDriverWait(web, TimeSpan.FromSeconds(120));

            return
                wait
                .Until(ExpectedConditions.ElementToBeClickable(web.FindElement(by)))
                .Tee(el => new Actions(web).MoveToElement(el))
                .Tee(el => Thread.Sleep(1000));
        }

        public static T Tee<T>(this T t, Action<T> act)
        {
            try
            {
                act(t);
                return t;
            }
            catch (Exception ex)
            {
                return t;
            }
        }
        public static R Pipe<T, R>(this T t, Func<T, R> func) => func(t);

    }
}
