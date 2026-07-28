using ClosedXML.Excel;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playwright_learnings
{
    internal class Test10_Excel: CloudBrowserPageTest
    {

        [Test]
        public async Task Excel_upload()
        {
            await Page.GotoAsync("https://rahulshettyacademy.com/upload-download-test/index.html");

            var download = await Page.RunAndWaitForDownloadAsync(async () =>
            {
                await Page.GetByRole(AriaRole.Button, new() { Name = "Download" }).ClickAsync();

            });

            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"Downloads");
            string filePath = Path.Combine(downloadsFolder, download.SuggestedFilename);
            await download.SaveAsAsync(filePath);

            /*Process.Start(new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"Unblock-File -Path \"{filePath}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            })?.WaitForExit();*/


            //Updating and saving excel
            using var workbook = new XLWorkbook(filePath);
            var sheet = workbook.Worksheet("Sheet1");
            sheet.Cell("B2").Value = "Prateik";
            workbook.Save();

            //Uploading back to the website
            await Page.Locator("div input.upload").SetInputFilesAsync(filePath);

            await Page.PauseAsync();

        }

    }
}
