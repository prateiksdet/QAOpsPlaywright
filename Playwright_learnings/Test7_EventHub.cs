using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Playwright_learnings
{
    [TestFixture]
    public class Test7_EventHub: CloudBrowserPageTest
    {
        public string FutureDateValue(int days)
        {
            return DateTime.Now.AddDays(days)
                               .ToString("yyyy-MM-ddTHH:mm");
        }
        /*
            - Above method fetches current date time value and adds whatever days added in it and returns added date.
         */

        [Test]
        public async Task EventHub_E2E()
        {
            var Context = await Browser.NewContextAsync();
            var Page = await Context.NewPageAsync();

            string BASE_URL = "https://eventhub.rahulshettyacademy.com";
            await Page.GotoAsync(BASE_URL);
            /*
                - Here, url has been stored into a variable first, so that it can be used at multiple locations. 
            */


            await Page.GetByPlaceholder("you@email.com").FillAsync("prateik.mallik@outlook.com");
            await Page.GetByLabel("Password").FillAsync("Dukekumar@96");
            await Page.Locator("#login-btn").ClickAsync();
            await Expect(Page.GetByText("Browse Events →")).ToBeVisibleAsync();
            Console.WriteLine("Login is successful");

            await Page.GetByRole(AriaRole.Button, new() { Name = "Admin" }).ClickAsync();
            await Page.Locator(".absolute.right-0.top-full").GetByText("Manage Events").ClickAsync();
            Console.WriteLine("Manage event page loaded");


            string uniqueEventTitle = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            // Since user wants to put title of event as current date time, DateTimeOffset is used for that.

            await Page.Locator("#event-title-input").FillAsync(uniqueEventTitle);
            await Page.Locator("#admin-event-form textarea").FillAsync("This is a description input box");
            await Page.GetByLabel("City").FillAsync("Darbhanga");
            await Page.GetByLabel("Venue").FillAsync("Sanrachna Engineers");
            await Page.GetByLabel("Event Date & Time").FillAsync(FutureDateValue(5));
            /*
                - Here FutureDateValue is called.
                - Date format in FutureDateValue() depends on the input format of Date & Time event field.
             */

            await Page.GetByLabel("Price ($)").FillAsync("100");
            await Page.GetByLabel("Total Seats").FillAsync("50");
            await Page.Locator("#add-event-btn").ClickAsync();
            await Expect(Page.GetByText("Event created!")).ToBeVisibleAsync();
            Console.WriteLine("Even created successfully");

            //User is moving to events screen.
            await Page.Locator("#nav-events").ClickAsync();
            await Page.Locator("#event-card").First.WaitForAsync();
            ILocator allEventCards = Page.Locator("#event-card"); //Fetching all event cards together

            

            
            await Expect(allEventCards.First).ToBeVisibleAsync();
            await allEventCards.Filter(new() { HasText = uniqueEventTitle }).IsVisibleAsync(); //Filtering event which has uniqueEventTitle as titile.

            string? seatsBeforeBooking = await allEventCards.Filter(new() { HasText = uniqueEventTitle }).Locator("div span.text-xs.font-semibold.text-emerald-600").TextContentAsync();
            int before = int.Parse(seatsBeforeBooking!.Split(' ')[0]);
            /*
                - seatsBeforeBooking returns - "50 seats available"
                - Split(' ')[0] is used.
                - Split by ' ' splits the sentence into 3 parts based on space i.e., [50,seats,available] --- Array
                - [o] means fetching 1st index item which is 50.
             */

            Console.WriteLine(before);
            await allEventCards.Filter(new() { HasText = uniqueEventTitle }).GetByTestId("book-now-btn").ClickAsync();
            /*
                - Instead of using for loop, QA is filtering the required card based on HasText.
                - Once filtered, card becomes unique and locator which were giving multiple results due to multiple cards available, can be used now.
             */

            Console.WriteLine("Navigated to confirm booking screen");

            await Expect(Page.Locator("#ticket-count")).ToHaveTextAsync("1");
            await Page.GetByLabel("Full Name").FillAsync("Prateik M");
            await Page.Locator("#customer-email").FillAsync("prateik.mallik@outout.com");
            await Page.GetByPlaceholder("+91 98765 43210").FillAsync("+919123443210");
            await Page.Locator(".confirm-booking-btn").ClickAsync();
            Console.WriteLine("Booking is successful");

            await Expect(Page.Locator("span.booking-ref")).ToBeVisibleAsync();
            string? bookingRefNum = await Page.Locator("span.booking-ref").TextContentAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "View My Bookings" }).ClickAsync();
            
            await Expect(Page).ToHaveURLAsync($"{BASE_URL}/bookings");
            /*
                -   It is a way to validate expected URL.
             */




            //Verify my bookings

            //ILocator myBookingCards = Page.Locator("#booking-card");
            //int myBookingCardsCount = await myBookingCards.CountAsync();

            //bool bookingFound = false;
            //await Expect(myBookingCards.First).ToBeVisibleAsync();
            //for(int i=0; i<myBookingCardsCount; i++)
            //{
            //    string? bookingRef = await myBookingCards.Nth(i).Locator(".booking-ref").TextContentAsync();       //Fetches bookingRef of all cards as loop goes on.
            //    string? eventTitle = await myBookingCards.Nth(i).Locator("h3.font-semibold").TextContentAsync();   //Fetches eventTitle of all cards as loop goes on.


            //    if (bookingRef?.Trim() == bookingRefNum && eventTitle?.Trim() == uniqueEventTitle)        //Matches fetched data with existing data.
            //    {
            //        bookingFound = true;  //If found true, loops break
            //        break;
            //    }
            //}
            //Assert.That(bookingFound, Is.True);  //Assert changes from false to true.


            //Playwright approach to verify my bookings:

            ILocator matchingBookingCard = Page.Locator("#booking-card").Filter(new() { HasText = bookingRefNum }).Filter(new() { HasText = uniqueEventTitle });
            await Expect(matchingBookingCard).ToBeVisibleAsync();


            //Verify seat reduction
            await Page.Locator("#nav-events").ClickAsync();

            await Expect(allEventCards.Filter(new() { HasText = uniqueEventTitle })).ToBeVisibleAsync();
            await Expect(allEventCards.Filter(new() { HasText = uniqueEventTitle }).Locator("span.text-emerald-600")).ToContainTextAsync((before - 1).ToString());
            /*
                - In above scenario, we are using both i.e., ToBeVisibleAsync() and ToContainTextAsync() because:
                        - Here, card visibility is happening first and then seat update happens.
                        - ToBeVisible() is for DOM visibility.
                        - ToContainTextAsync() is for Data refresh.
                - This is perfect scenario of real enterprise automation problem.
            */

            string? seatsAfterBooking = await allEventCards.Filter(new() { HasText = uniqueEventTitle }).Locator("div span.text-xs.font-semibold.text-emerald-600").TextContentAsync();
            int after = int.Parse(seatsAfterBooking!.Split(' ')[0]);
            Console.WriteLine(after);

            Assert.That(after, Is.EqualTo(before - 1));


        }
    }
}
