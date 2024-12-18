using Microsoft.AspNetCore.Mvc;
using PusherServer;
using PusherClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestPusher.Service;
using System.Text.Json;
namespace TestPusher.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PushsherController : ControllerBase
{  
    private readonly PusherServer.Pusher pusher;
    private PusherClient.Pusher pusher_client;

 private SupportService _service = new SupportService();
  

   public class BindingData
   {
     public string Event { get; set; }
     public string Data { get; set; }
     public string Channel { get; set; }
   }
    
    public PushsherController()
    {
        var options = new PusherServer.PusherOptions
        {
            Cluster="ap1",
            Encrypted = true
        };
        pusher = new PusherServer.Pusher
        (
     "1845318",
      "4907c41a6a23e1b469bd",
      "431a84002e9ad8782d66",
      options
        );
      Task.Run(async()=>
      {  var options_client=new PusherClient.PusherOptions
        {
            Cluster="ap1",
            Encrypted=true
        };
        pusher_client=new PusherClient.Pusher
        (
            "4907c41a6a23e1b469bd",options_client
        );

        PusherClient.Channel channel=await pusher_client.SubscribeAsync("gsm-channel");
        
        
        channel.Bind("push-sim-test",(data)=>{
            string json = JsonConvert.SerializeObject(data); 
            var data_obj = JsonConvert.DeserializeObject<BindingData>(json);
            Console.WriteLine(data_obj.Data);
        });
      });
    }
   [HttpPost("trigger")]
   public async Task<IActionResult> TriggerEvent([FromBody] PusherEventData data)
   { 
    Console.WriteLine("trigger here");
    var res_trigger=await pusher.TriggerAsync(
        data.ChannelName,
        data.EventName,
       new {message=data.Data}
    );
     
    if(res_trigger.StatusCode==System.Net.HttpStatusCode.OK)
    {
       return Ok(new {success=true,data=res_trigger});
    } 
    return StatusCode((int)res_trigger.StatusCode,res_trigger.Body);
    
   }

   [HttpPost("scraping")]

   public async Task<IActionResult> WebScraping()
   {
string page_name="https://mangakakalot.to/new?sort=default&page=9";
     await this._service.webScrapingTesting(page_name);
     return Ok();
   }
   
 [HttpGet("chat_id")]

 public async Task<IActionResult> RetrieveChatId()
 {
  await this._service.getGroupChatId();
  return Ok();
 }


   [HttpPost("auth")]
   public IActionResult Auth([FromForm]AuthRequest request)
   {
  Console.WriteLine("here:"+request.ChannelName);
   var auth= pusher.Authenticate(request.ChannelName,request.SocketId);
   return Ok(auth);      
   }

 [HttpPost("crawldata")]

 public async Task<IActionResult> CrawlTopUpData()
 {      string url="https://cachua.shop";
        await this._service.topupDataScrapingTest(url);
        return Ok();
 }



[HttpGet("crawldata_service")]
public async Task<IActionResult> CrawlDataService([FromQuery]string url)
{ string json_data="";
try{
url=Uri.UnescapeDataString(url);
  var val=await this._service.topupDrugData(url);
   json_data=JsonConvert.SerializeObject(val,Formatting.Indented);
//  await this._service.sendMessageToTele(json_data);
}
catch(Exception ex)
{
  Console.WriteLine(ex.Message);
  return StatusCode(500);
}
  return Ok(json_data);
}

 public class PusherEventData
 {
    public string ChannelName{get;set;}
    public string EventName{get;set;}
    public string Data{get;set;}
 }
 public class AuthRequest
 {
 public string ChannelName{get;set;}

 public string SocketId{get;set;}
 }
}

