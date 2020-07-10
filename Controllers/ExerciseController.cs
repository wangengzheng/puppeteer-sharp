using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using puppeteer_sharp.Models;
using puppeteer_sharp.Models.Html;
using puppeteer_sharp.Services;
using PuppeteerSharp;

namespace puppeteer_sharp.Controllers
{

    //https://www.puppeteersharp.com/examples/index.html
    //https://www.cnblogs.com/Wayou/p/using_puppeteer_to_take_screenshot.html

    [Route("[controller]/[action]")]
    [ApiController]
    public class ExerciseController : Controller//: ControllerBase 
    {

        ExerciseService exerciseService;
        ViewRenderService viewRenderService;
        string contentPath = "";
        string exercisePath = "";
        private static bool exerciseExist =false;
        public ExerciseController(
            IHostingEnvironment hostingEnvironment
            , ExerciseService exerciseService
            , ViewRenderService viewRenderService
            )
        {
            contentPath = hostingEnvironment.ContentRootPath;
            exercisePath = Path.Combine(contentPath, "exercise");
            if (!exerciseExist || !Directory.Exists(exercisePath))
            {                
                Directory.CreateDirectory(exercisePath);
                exerciseExist = true;            
            }

            this.exerciseService = exerciseService;
            this.viewRenderService = viewRenderService;
        }

        //习题选择题 kh73k4w6jb_ZTs0hH6EkAM question Image


        //1 出现 缺失文件的情况 跟 chromnium 引用有关 ex: error while loading shared libraries: libX11.so.6
        //2 出现 打包镜像慢的情况 只能用vultr解决 本地实在太慢 出现下载慢DownloadAsync都是跟墙有关
        //3 出现 Running as root without --no-sandbox is not supported 需要改 args --no-sandbox --disable-setuid-sandbox

        // GET api/values
        [HttpGet("{exerciseId}/{reload?}")]
        public async Task<ActionResult> Question(string exerciseId, string reload)
        {
            if (string.IsNullOrEmpty(exerciseId)) {
                return File(new byte[] { }, "image/png");
            }

            var filePath = Path.Combine(exercisePath, nameof(Question).ToLower(), exerciseId, "0.png");

            var exercise = await exerciseService.GetById(exerciseId);

            if (exercise == null)
            {
                return NotFound();
            }


            filePath = CheckFilePath(filePath,reload);

            var html = @"<!DOCTYPE html>
<html>
<head>
<title>Page Title</title>
</head>
<body>
<h1> this is question! </h1>
</body>
</html>";

            if (System.IO.File.Exists(filePath))
            {
                return File(System.IO.File.ReadAllBytes(filePath), "image/png");
            }

            html = await viewRenderService.RenderToStringAsync("Exercise/QuestionView", exercise);

            return await HtmlAutoSize(html, filePath);

            return await Html(html, filePath, screenshotOptions: new ScreenshotOptions() {
                FullPage=true,
                Type= ScreenshotType.Png
            });
        }

        [HttpGet("{exerciseId}/{reload?}")]
        public async Task<ActionResult> Answer(string exerciseId, string reload)
        {
            if (string.IsNullOrEmpty(exerciseId))
            {
                return File(new byte[] { }, "image/png");
            }

            var filePath = Path.Combine(exercisePath, nameof(Answer).ToLower(), exerciseId, "0.png");

            filePath = CheckFilePath(filePath,reload);

            var exercise = await exerciseService.GetById(exerciseId);

            if (exercise == null || string.IsNullOrEmpty(exercise.Answer))
            {
                return NotFound();
            }

            var html = $@"<html><head>
    <meta name=""viewport"" content=""width=device-width"">
    <title>answer</title>
</head>
<body>
<div id=""content"" style=""display: inline-block"">
    <div>{exercise.Answer}</div>    
</div>
</body></html>";

            //var html = exercise.Answer;

            return await HtmlAutoSize(html, filePath);

            return await Html(html, filePath, screenshotOptions: new ScreenshotOptions()
            {
                //FullPage = true, //options.clip and options.fullPage are exclusive
                Clip = new PuppeteerSharp.Media.Clip() {
                    X = 0, Y = 0, Height = 30, Width = html.Length * 30
                },
                Type = ScreenshotType.Png
            });
        }

        [HttpGet("{exerciseId}/{reload?}")]
        public async Task<ActionResult> Analysis(string exerciseId, string reload)
        {
            if (string.IsNullOrEmpty(exerciseId))
            {
                return NotFound();
                return File(new byte[] { }, "image/png");
            }

            var exercise = await exerciseService.GetById(exerciseId);

            if (exercise == null ||  string.IsNullOrEmpty(exercise.Analysis))
            {
                return NotFound();
            }

            var filePath = Path.Combine(exercisePath, nameof(Analysis).ToLower(), exerciseId, "0.png");

            filePath = CheckFilePath(filePath, reload);            

            var html = $@"<html><head>
    <meta name=""viewport"" content=""width=device-width"">
    <title>analysis</title>
</head>
<body>
<div id=""content"" style=""display: inline-block"">
    <div>{exercise.Analysis}</div>    
</div>
</body></html>";

            //var html = exercise.Analysis;

            return await HtmlAutoSize(html, filePath);
        }

        /// <summary>
        /// question html view
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        [HttpGet("{exerciseId}/{reload?}")]
        public async Task<ActionResult> QuestionView(string exerciseId,string reload)
        {
            if (string.IsNullOrEmpty(exerciseId)) {
                return NotFound();
            }

            var exercise = await exerciseService.GetById(exerciseId);

            if (exercise == null) {
                return NotFound();
            }

            return View(exercise);
        }
        
        /// <summary>
        /// question json view
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="reload"></param>
        /// <returns></returns>
        [HttpGet("{exerciseId}/{reload?}")]
        public async Task<ActionResult> QuestionJson(string exerciseId, string reload)
        {
            if (string.IsNullOrEmpty(exerciseId))
            {
                return NotFound();
            }

            var exercise = await exerciseService.GetById(exerciseId);

            return Json(exercise);
        }

        private async Task<ActionResult> HtmlAutoSize(string html, string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {

                return File(System.IO.File.ReadAllBytes(filePath), "image/png");
            }

            try
            {
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

                using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new string[] {
                        "--no-sandbox",
                        "--disable-setuid-sandbox"
                    },
                    //DefaultViewport = new ViewPortOptions() {
                    //    Height = 1000,
                    //    Width = 800
                    //}
                }))
                {
                    using (var page = await browser.NewPageAsync())
                    {   
                        

                        await page.SetContentAsync(html);

                        var jsCode = @"()=>{
                            const selectors =document.getElementById('content');
                            if(selectors==undefined){
                                return null;
                            }
                            return {
                                CWidth :selectors.clientWidth,
                                CHeight :selectors.clientHeight
                            };
                        }";

                        var contentResponse = await page.EvaluateFunctionAsync<ContentResponseModel>(jsCode);

                        Console.WriteLine(JsonConvert.SerializeObject(contentResponse));

                        if (contentResponse == null) {
                            await page.ScreenshotAsync(filePath);
                        }
                        else {

                            await page.SetViewportAsync(new ViewPortOptions()
                            {
                                Height=(int)contentResponse.CHeight,
                                Width=(int)contentResponse.CWidth,                           
                            });

                            await page.ScreenshotAsync(filePath, new ScreenshotOptions()
                            {
                                FullPage=true,
                                Type= ScreenshotType.Png,                                
                            });

                            /*
                            await page.ScreenshotAsync(filePath,new ScreenshotOptions() { 
                                Clip=new PuppeteerSharp.Media.Clip() { 
                                X=0,Y=0,Height=contentResponse.CHeight,Width=contentResponse.CWidth
                                },
                                Type = ScreenshotType.Png
                            } );
                            */
                        }

                        return File(System.IO.File.ReadAllBytes(filePath), "image/jpeg");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);

                return Content(ex.Message);
            }
        }

        private async Task<ActionResult> Html(string html, string filePath, ViewPortOptions viewPortOptions = null,ScreenshotOptions screenshotOptions=null)
        {            
            if (System.IO.File.Exists(filePath)) {

                return File(System.IO.File.ReadAllBytes(filePath), "image/png");
            }

            try
            {
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
                        if (viewPortOptions != null)
                        {
                            await page.SetViewportAsync(viewPortOptions);
                        }                        

                        await page.SetContentAsync(html);

                        ////await page.WaitForSelectorAsync("body");

                        //var height = await page.EvaluateFunctionAsync<int>("()=>document.body.clientHeight");
                        //var widht = await page.EvaluateFunctionAsync<int>("()=>document.body.clientWidth");

                        //Console.WriteLine($"height:{height}");
                        //Console.WriteLine($"widht:{widht}");


                        await page.ScreenshotAsync(filePath, screenshotOptions);

                        

                        return File(System.IO.File.ReadAllBytes(filePath), "image/jpeg");
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);

                return Content(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">当前目录如果不创建就创建</param>
        /// <param name="reload">不为空就删除文件</param>
        /// 


        
        private string CheckFilePath(string filePath,string reload)
        {
            var fileInfo = new FileInfo(filePath);

            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }

            if (!string.IsNullOrEmpty(reload))
            {
                System.IO.File.Delete(filePath);

                filePath += $"{reload}.png";               

                return filePath;
            }

            return filePath;
        }

        //[HttpGet]
        private async Task<object> viewHtml()
        {
            //var viewResult = await QuestionView("wWu9iod7ho6S5p84Z0kWy0");

            object data = await exerciseService.GetById("wWu9iod7ho6S5p84Z0kWy0");

            return await viewRenderService.RenderToStringAsync("Exercise/QuestionView", data);
        }


        private async Task<object> Linux_Word()
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
    }
}
