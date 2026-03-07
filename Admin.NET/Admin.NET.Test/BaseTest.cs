// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace Admin.NET.Test;

/// <summary>
/// Test base class
/// </summary>
public class BaseTest : IDisposable
{
    private readonly string _baseUrl = "http://localhost:8888";
    protected readonly EdgeDriver Driver = new();

    protected BaseTest(string token = null)
    {
        var url = _baseUrl;
        if (!string.IsNullOrWhiteSpace(token)) url += $"/#/login?token={token}";
        Driver.Manage().Window.Maximize();
        Driver.Navigate().GoToUrl(url);

        // Implicit wait for 3 seconds (implicit wait is when the element is not presented)
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
    }

    /// <summary>
    /// Wait for the page to load
    /// </summary>
    protected async Task WaitExecutorCompleteAsync()
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        await Task.Delay(1000);
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    protected async Task Login(string account = "superadmin", string password = "123456")
    {
        await GoToUrlAsync("/#/login");
        var inputList = Driver.FindElements(By.CssSelector("#pane-account input"));

        // Enter username
        var accountInput = inputList.First();
        accountInput.Clear();
        accountInput.SendKeys(account);

        // Enter password
        var passwordInput = inputList.Skip(1).First();
        passwordInput.Clear();
        passwordInput.SendKeys(password);

        // enter confirmation code
        var captchaInput = inputList.Skip(2).First();
        captchaInput.Clear();
        captchaInput.SendKeys("0");

        // submit
        var button = Driver.FindElement(By.CssSelector("#pane-account button"));
        button.Click();
    }

    /// <summary>
    /// Open the specified page
    /// </summary>
    /// <param name="url"></param>
    protected async Task GoToUrlAsync(string url)
    {
        if (url.StartsWith("http")) await Driver.Navigate().GoToUrlAsync(url);
        else await Driver.Navigate().GoToUrlAsync(_baseUrl + "/" + url.TrimStart('/'));
        await WaitExecutorCompleteAsync();
    }

    /// <summary>
    /// Wait for the user to press Enter to continue
    /// </summary>
    /// <param name="text">prompt word</param>
    protected void WaitEnter(string text = "Wait for the user to press Enter to continue...")
    {
        Console.WriteLine(text);
        Console.ReadLine();
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}