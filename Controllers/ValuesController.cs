using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace puppeteer_sharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        string contentPath = "";
        IHostingEnvironment hostingEnvironment;
        public ValuesController(IHostingEnvironment hostingEnvironment)
        {
            contentPath = hostingEnvironment.ContentRootPath;
            Console.WriteLine(contentPath);
            this.hostingEnvironment = hostingEnvironment;
        }

        //习题选择题 kh73k4w6jb_ZTs0hH6EkAM question Image


        //1 出现 缺失文件的情况 跟 chromnium 引用有关 ex: error while loading shared libraries: libX11.so.6
        //2 出现 打包镜像慢的情况 只能用vultr解决 本地实在太慢 出现下载慢DownloadAsync都是跟墙有关
        //3 出现 Running as root without --no-sandbox is not supported 需要改 args --no-sandbox --disable-setuid-sandbox

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Content($"{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)? OSPlatform.Windows : OSPlatform.Linux)}-{BrowserFetcher.DefaultRevision}");
        }


        private async Task<object> Image()
        {

            //http://vote.jxt189.com:9779/group3/M00/A6/C8/pIYBAFmmsWWAYP8jAAAbR6yv08M147.png?auth_key=1598514434-195797-0-015a96b49fa2694fd6500aea1a543e2d&sc=clatea

            var version = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 706915 : BrowserFetcher.DefaultRevision;



            Console.WriteLine(Platform.Linux.ToString());

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new string[] {
                        "--no-sandbox",
                        "--disable-setuid-sandbox"
                    }
            }))
            {
                using (var page = await browser.NewPageAsync())
                {



                    var html = @"<!DOCTYPE html>
<html>
<head>
<title>Page Title</title>
</head>
<body>

<p><span style=""font-family:times new roman,times,serif""><span style=""font-size:28px""><strong>当别人向你问好时，你会正确应答吗？请选择正确的答案：</strong></span></span></p>

<p><span style=""font-family:times new roman,times,serif""><span style=""font-size:28px""><strong>Good night.</strong></span></span></p>
<p>&nbsp;</p>

A.<p><strong><span style=""font-size:28px""><span style=""font-family:times new roman,times,serif"">Good evening!</span></span></strong></p>
A.<p><strong><span style=""font-size:28px""><span style=""font-family:times new roman,times,serif"" > Good night.</span></span></strong></p>
A.<p><strong><span style=""font-size:28px"" ><span style=""font-family:times new roman,times,serif""> Good afternoon!</span></span></strong></p>

</body>
</html>";

                    //html = "A";


                    //await page.GoToAsync("http://www.baidu.com");

                    //Console.WriteLine(await page.GetContentAsync());









                    await page.SetContentAsync(html);

                    //await browser.CloseAsync();


                    //page.wi

                    var fileDir = Path.Combine(contentPath, "Images");
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }


                    //await page.GoToAsync("http://www.baidu.com");
                    //await page.GoToAsync("http://avgd.edstao.com/goods/detail?productid=9777BD88-7A80-4FB1-9577-685FD95E400D");

                    var guid = Guid.NewGuid().ToString();
                    Console.WriteLine(guid);

                    var filePath = Path.Combine(fileDir, guid + ".png");

                    //ViewPortOptions

                    //await page.SetViewportAsync(new ViewPortOptions()
                    //{
                    //    //IsMobile = true,
                    //    //Height = 1200,
                    //    //Width = 1100
                    //});

                    await page.ScreenshotAsync(filePath, new ScreenshotOptions()
                    {
                        FullPage = true,
                        //Clip =new PuppeteerSharp.Media.Clip() { 
                        //  X=0,Y=0,Width=300,Height=300
                        //},
                        Quality = 80,
                        Type = ScreenshotType.Jpeg,
                    });


                    return File(System.IO.File.ReadAllBytes(filePath), "image/jpeg");
                }
            }
        }

        private async Task<object> Html()
        {

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            var page = await browser.NewPageAsync();

            var html = @"<!DOCTYPE html>
<html>
<head>
<title>Page Title</title>
</head>
<body>

<h1>This is a Heading</h1>
<p>This is a paragraph.</p>

</body>
</html>";

            //html = "A";


            //await page.GoToAsync("http://www.baidu.com");

            //Console.WriteLine(await page.GetContentAsync());


            await page.SetContentAsync(html);
            //await page.GoToAsync("http://www.baidu.com");

            var filePath = $@"e:\1temp\Images\google_{Guid.NewGuid().ToString()}.png";



            //await page.SetViewportAsync(new ViewPortOptions() { 
            //    IsMobile=true,
            //    Height=200,Width=100
            //});

            await page.ScreenshotAsync(filePath, new ScreenshotOptions()
            {
                //FullPage=true,
                Clip = new PuppeteerSharp.Media.Clip()
                {
                    X = 0,
                    Y = 0,
                    Width = 300,
                    Height = 300
                },
                Quality = 80,
                Type = ScreenshotType.Jpeg,
            });


            return File(System.IO.File.ReadAllBytes(filePath), "image/jpeg");
        }


        /// <summary>
        /// exercise answerhtml2image
        /// </summary>
        /// <returns></returns>
        private async Task<object> Answer()
        {

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            var page = await browser.NewPageAsync();

            var html = @"<!DOCTYPE html>
< html >
< head >
< title > Page Title </ title >
   </ head >
   < body >
   

   < h1 > This is a Heading </ h1 >
   < p > This is a paragraph.</ p >
      

      </ body >
      </ html > ";

            html = "A";

            await page.SetContentAsync(html);
            //await page.GoToAsync("http://www.baidu.com");

            var filePath = $@"e:\1temp\Images\google_{Guid.NewGuid().ToString()}.png";



            //await page.SetViewportAsync(new ViewPortOptions() { 
            //    IsMobile=true,
            //    Height=200,Width=100
            //});

            await page.ScreenshotAsync(filePath, new ScreenshotOptions()
            {
                //FullPage=true,
                Clip = new PuppeteerSharp.Media.Clip()
                {
                    X = 0,
                    Y = 0,
                    Width = 30,
                    Height = 30
                },
                Quality = 80,
                Type = ScreenshotType.Jpeg,
            });


            return File(System.IO.File.ReadAllBytes(filePath), "image/jpeg");

        }


        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
