using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MyAnimeListFriendBot
{
    class Program
    {
        static void Main(string[] args)
        {
            int amountFriend;
            string userName, password, commentFriend;

            // Enter Username
            Console.WriteLine("Enter Username :");
            userName = Console.ReadLine();

            // Enter Password
            Console.WriteLine("Enter Password :");
            password = Console.ReadLine();

            Console.WriteLine("How many friends you want to add :");
            amountFriend = int.Parse(Console.ReadLine());

            Console.WriteLine("Send default comment on friend page? Y/N");
            commentFriend = Console.ReadLine().ToUpper();

            // Load The Driver
            IWebDriver driver = new ChromeDriver();

            // Load The IJavaScriptExecutor
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // Load The WebDriverWait
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 1, 0));

            // Enter Url
            driver.Manage().Window.Maximize();
            driver.Url = "https://myanimelist.net/login.php";

            // Enter Username/Password            
            driver.FindElement(By.Id("loginUserName")).SendKeys(userName);
            driver.FindElement(By.Id("login-password")).SendKeys(password + Keys.Enter);

            // Check For Login   
            Thread.Sleep(1000);

            int login = driver.FindElements(By.ClassName("badresult")).Count;

            Console.WriteLine(login);

            if (login == 0)
            {
                int amountFriendCounter = 0;

            // Navigate to Users                          
            Re: driver.Navigate().GoToUrl("https://myanimelist.net/users.php");

                Thread.Sleep(2000);

                IList<IWebElement> friends = driver.FindElements(By.XPath("//*[@class='borderClass']/div[1]/a"));

                for (int i = 0; i < friends.Count; i++)
                {
                    string urlName = friends[i].Text;

                    js.ExecuteScript("window.open();");

                    IList<string> tabs = new List<string>(driver.WindowHandles);

                    driver.SwitchTo().Window(tabs[1]);
                    driver.Navigate().GoToUrl("https://myanimelist.net/profile" + "/" + urlName.ToString());

                    Thread.Sleep(2000);

                    // Check User Is Exists
                    int is404 = driver.FindElements(By.ClassName("error404")).Count;

                    // Check Request Is Available
                    int isRequest = driver.FindElements(By.CssSelector("[class*='icon-user-function icon-request js-user-function disabled']")).Count;

                    // Check Already a Friend
                    int isFriend = driver.FindElements(By.CssSelector("[class*='icon-user-function icon-remove js-user-function']")).Count;

                    // Check Comments Are Available
                    int isComment = driver.FindElements(By.ClassName("textarea")).Count;

                    if (is404 > 0)
                    {
                        Console.WriteLine("There is no such a user name: " + urlName);
                    }
                    else
                    {
                        if (isRequest > 0)
                        {
                            Console.WriteLine(urlName + " Is Not Accepting Friend Request");
                        }
                        else
                        {
                            if (isFriend > 0)
                            {
                                Console.WriteLine(urlName + " is already a friend of yours");
                            }
                            else
                            {
                                if (isComment > 0 && commentFriend == "Y")
                                {
                                    // Add Some Comments
                                    IWebElement comments = wait.Until(d => d.FindElement(By.ClassName("textarea")));
                                    comments.SendKeys("Hi there, I like watching anime and talking about it so I'm looking for friends.");

                                    IWebElement submitButton = wait.Until(d => d.FindElement(By.XPath("//*[@id='lastcomment']/div/form/div/input")));
                                    submitButton.Click();

                                    Thread.Sleep(2000);
                                }
                                else
                                {
                                    Console.WriteLine("Comments for " + urlName + " is disabled / user didn't select it");
                                }

                                // Send Friend Request                            
                                IWebElement request = wait.Until(d => d.FindElement(By.XPath("//*[@id='request']")));
                                request.Click();

                                IWebElement request_Submit = wait.Until(d => d.FindElement(By.XPath("//*[@id='dialog']/tbody/tr/td/form/div[3]/input[1]")));
                                request_Submit.Click();

                                amountFriendCounter++;

                                Console.WriteLine(urlName + " Added");

                                if (amountFriendCounter == amountFriend)
                                {
                                    driver.Close();
                                    driver.SwitchTo().Window(tabs[0]);
                                    break;
                                }

                                Thread.Sleep(15000);
                            }
                        }
                    }

                    driver.Close();
                    driver.SwitchTo().Window(tabs[0]);
                }

                if (amountFriendCounter != amountFriend)
                {
                    Console.WriteLine("Not All " + amountFriend + " Friends Added Refreshing User Page");
                    goto Re;
                }

                js.ExecuteScript("alert('All Friends Added Successfully! Program Closing In 3 Seconds');");
                Thread.Sleep(3000);
                driver.Quit();
            }
            else
            {
                js.ExecuteScript("alert('Username/Password Is Wrong, Program Shuting Down In 2 Seconds');");
                Thread.Sleep(2000);
                driver.Quit();
            }
        }
    }
}
