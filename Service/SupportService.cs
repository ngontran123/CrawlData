using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using System.Management;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Text.Unicode;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

using Telegram.Bot;

namespace TestPusher.Service;

public class SupportService
{

    public class InteractionData
{
    public class Interaction
    {
        public string Level { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; }
    }

    public class Interactions
    {
        public List<Interaction> Customer { get; set; } = new List<Interaction>();
        public List<Interaction> Professional { get; set; } = new List<Interaction>();
    }

    public class DrugFoodInteractions
    {
        public List<Interaction> Customer { get; set; } = new List<Interaction>();
        public List<Interaction> Professional { get; set; } = new List<Interaction>();
    }

    public Interactions InteractionsBetweenYourDrugs { get; set; } = new Interactions();
    public DrugFoodInteractions DrugAndFoodInteractions { get; set; } = new DrugFoodInteractions();
}


 List<string[]> list_comik_table=new List<string[]>();
 List<string[]> list_author_table = new List<string[]>();

 List<string[]> list_genre_table = new List<string[]>();

 List<string[]> list_comik_author_table = new List<string[]>();
 List<string[]> list_comik_genre_table = new List<string[]>();

 List<string[]> list_chapter_table = new List<string[]>();

 List<string[]> list_chapter_detail = new List<string[]>();

 int latest_comik_item=0;
 
 int latest_author_item=0;
 
 int latest_genre_item=0;

int latest_comik_author_item=0;

int latest_comik_genre_item = 0;

int latest_chapter_item=0;

int latest_chapter_detail_item = 0;

private readonly string bot_id="7554754756:AAFZg4ELlFhpvTfPEIOfZohaBKehRF1n_4k";

private readonly string chat_id="-1002330764858";



     public string AddSha256(string data)
 {
    using(SHA256 hash=SHA256.Create())
    {
        byte[] bytes=hash.ComputeHash(Encoding.UTF8.GetBytes(data));

        StringBuilder sha_hash=new StringBuilder();
        
        for(int i=0;i<bytes.Length;i++)
        {
            sha_hash.Append(bytes[i].ToString("x2"));
        }
        return sha_hash.ToString();
    }
 }

 public string GetCurrentFilePath(string file_name)
 {
    string full_file_path=Path.Combine(Directory.GetCurrentDirectory(),file_name);
    return full_file_path;
 }

 public void fillFullListData(string file_name,List<string[]>list_data)
 {
    try
    {
    string file_path=GetCurrentFilePath(file_name);
    if(!File.Exists(file_path))
    {
        return;
    }   
    using(StreamReader srd=new StreamReader(file_path,Encoding.UTF8))
    {
        while(!srd.EndOfStream)
        {
            string? line=srd.ReadLine();
            if(!string.IsNullOrEmpty(line))
            {
            list_data.Add(line.Split(','));
            }
        }
    }
    }
    catch(Exception er)
    {
        Console.WriteLine(er.Message);
    }
 }

 public string generateRandomPassword()
{
string guid=Guid.NewGuid().ToString();
return guid;
}

public string getCurrentOs()
{
var os_name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                      select x.GetPropertyValue("Caption")).FirstOrDefault();


return os_name != null ? os_name.ToString() : "Unknown";
}

public string standardString(string value)
{
    return value.Replace("\n","").Replace("\r","").Replace("\t","").TrimStart().TrimEnd();
}


public void writeCsvFile(string file_name,string data)
{
    try
    {
    string file_path=GetCurrentFilePath(file_name);
    using(var writer=new StreamWriter(file_path,true,Encoding.UTF8))
    {
        writer.WriteLine(data);
    }
    }
    catch(Exception ex)
    {

    }
}

public string handleEscapedField(string data)
{  if(!string.IsNullOrEmpty(data))
{
    if(data.Contains(",")||data.Contains(";")||data.Contains("\""))
    {
        data=data.Replace("\"","\"\"");
        return $"\"{data}\"";
    }
    else{
        return data;
    }
} 
    return "";
}

public int getLatestId(List<string[]>list_data)
{  int latest_row=0;
    try
    {

     if(list_data.Count>0)
     {
       string[] data=list_data[list_data.Count-1];
       latest_row=Convert.ToInt32(data[0]);  
     }
     else
     {
        latest_row=0;
     }
    }
    catch(Exception er)
    {
    Console.WriteLine(er.Message);
    }
    return latest_row;
}


public async Task<int> addComikTable(IPage page,IReadOnlyList<ILocator> line_content,int id,string chapters)
{           int res=0;
            var line_content_first=line_content[0];
            var line_content_second=line_content[2];
            var line_content_third = line_content[3];
            var line_content_four= line_content[4];
            var description_content=await page.Locator(".dbs-content").TextContentAsync();
            string description=$"\"{standardString(description_content)}\"";            
            var comik_name=await line_content_first.Locator(".manga-name").TextContentAsync();
            var status = await line_content_second.Locator(".result").TextContentAsync();
            var published=await line_content_third.Locator(".result").TextContentAsync();
            var views=await line_content_four.Locator(".result").TextContentAsync();
            string views_parse=standardString(views);
            string status_comik=standardString(status);
            string published_date=standardString(published);
            string comik_name_parse =standardString(comik_name);            
            var avatar = await page.QuerySelectorAsync($"img[alt=\"{comik_name_parse}\"]");
            var link_avatar=await avatar.GetAttributeAsync("src");

            string file_name="comik.csv";


            if(this.list_comik_table.Any(comik=>comik.Length>2&&comik[1]==comik_name_parse))
            {   res=-1;
                return res;
            }
            string data=$"{id},{this.handleEscapedField(comik_name_parse)},{this.handleEscapedField(published_date)},{this.handleEscapedField(chapters)},{this.handleEscapedField(status_comik)},{this.handleEscapedField(description)},{this.handleEscapedField(views_parse)},white,{this.handleEscapedField(link_avatar)}";
            this.list_comik_table.Add(data.Split(','));
            writeCsvFile(file_name,data);
            Console.WriteLine($"{comik_name_parse} & {link_avatar}");
            Console.WriteLine($"Chapter:{chapters}");
            Console.WriteLine($"Description:{description}");
            Console.WriteLine($"Status:{status_comik}");
            Console.WriteLine($"Published:{published_date}");
            return res;
}

public string standardAuthorName(string author)
{   string standard_author_name="";
    author=author.Trim();
    if(author.Contains(","))
    {
        string[] value_split=author.Split(',');
        for(int i=value_split.Length-1;i>=0;i--)
        {
            standard_author_name+=value_split[i]+" ";
        }
    }
    else 
    {
        standard_author_name=author;
    }
    return standard_author_name.TrimStart().TrimEnd();
}

public async Task addAuthorTable(IReadOnlyList<ILocator> line_contents)
{ 

 var line_content=line_contents[1];
 var authors_detail_section=line_content.Locator(".result").First;
 var authors_detail=await authors_detail_section.Locator("a").AllAsync();
 string author_name_parse="";
 string file_name="author.csv";
 if(authors_detail!=null)
 { 
 foreach(var author in authors_detail)
 {   
    var author_name=await author.TextContentAsync();
    author_name_parse = standardString(author_name);
    author_name_parse=standardAuthorName(author_name_parse);
    if(this.list_author_table.Any(author=>author.Length>1 && author[1]==author_name_parse))
    {  string[] author_exist= this.list_author_table.SingleOrDefault(author=>author[1]==author_name_parse);
        int author_id=Convert.ToInt32(author_exist[0]);
        this.addComikAuthorDetail(this.latest_comik_item,author_id);
        continue;
    }
    string data=$"{this.latest_author_item},{this.handleEscapedField(author_name_parse)},{""}";
    this.list_author_table.Add(data.Split(','));
    this.writeCsvFile(file_name,data);
    this.addComikAuthorDetail(latest_comik_item,latest_author_item);
    this.latest_author_item+=1;
    Console.WriteLine(author_name_parse);
 }
 }
} 

public async Task addChapterDetail(IPage page,int chapter_id,string filename)
{
    await page.WaitForSelectorAsync(".image-vertical");

    var scroll_script=@"
    const autoScroll = async () => {
         const scrollStep = 600; 
         const scrollDelay = 500; 

            return new Promise((resolve, reject) => {
                const scrollInterval = setInterval(() => {
                    window.scrollBy(0, scrollStep);
                    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
                        clearInterval(scrollInterval);
                        resolve();
                    }
                }, scrollDelay);
            });      
        }
        autoScroll();
    ";
   
  //await Task.Delay(5000);
   
   await page.EvaluateAsync(scroll_script);
   
   int count_nums=0;
   
   var images=await page.Locator(".image-vertical").AllAsync();

   var canvas= await page.Locator(".card-wrap").AllAsync();
   
  if(canvas!=null)
  {
    foreach(var cv in canvas)
    {
        count_nums+=1;
        var image_href=await cv.GetAttributeAsync("data-url");
        this.latest_chapter_detail_item+=1;
        string handle_escaped=this.handleEscapedField(image_href);
    if(string.IsNullOrEmpty(handle_escaped))
    {   
        await page.CloseAsync();
        return;
    }
    string data_detail=$"{latest_chapter_detail_item},{chapter_id},{count_nums},{handle_escaped}";
    this.writeCsvFile(filename,data_detail);
    Console.WriteLine(image_href);
    }
  }
else if(images!=null){
   foreach(var image in images)
   { 
    count_nums+=1;
    var image_href=await image.GetAttributeAsync("src");
    this.latest_chapter_detail_item+=1;
    string handle_escaped=this.handleEscapedField(image_href);
    if(string.IsNullOrEmpty(handle_escaped))
    {   
        await page.CloseAsync();
        return;
    }
    string data_detail=$"{latest_chapter_detail_item},{chapter_id},{count_nums},{handle_escaped}";
    this.writeCsvFile(filename,data_detail);
    Console.WriteLine(image_href);
   }
  
}
  await Task.Delay(1000);
  await page.GoBackAsync();
// if(images!=null)
// {
//     Console.WriteLine("yesh");
// }
// else{
//     Console.WriteLine("no");
// }
}

public async Task addGenreTable(IReadOnlyList<ILocator> line_contents)
{
try{
  var line_content=line_contents[5];
  var genre_detail_section=line_content.Locator(".result").First;
  var genre_detail=await line_content.Locator("a").AllAsync();
  string genre_parse="";
  string file_name="genres.csv";
  foreach(var genre in genre_detail)
  {
    var genre_name=await genre.TextContentAsync();
    
    genre_parse=standardString(genre_name);

    if(this.list_genre_table.Any(genre=>genre.Length>1 && genre[1]==genre_parse))
    {  Console.WriteLine("genre existed here:"+genre_parse);
      string[] current_genres=this.list_genre_table.SingleOrDefault(genre_item=>genre_item[1]==genre_parse);
      int genre_id=Convert.ToInt32(current_genres[0]);
      this.addComikGenreDetail(latest_comik_item,genre_id);
    }
    else
    { 
    Console.WriteLine("genres out here:"+genre_parse);
    string data=$"{this.latest_genre_item},{this.handleEscapedField(genre_parse)}";
    list_genre_table.Add(data.Split(','));
    this.writeCsvFile(file_name,data);
    this.addComikGenreDetail(latest_comik_item,latest_genre_item);
    this.latest_genre_item+=1;
    Console.WriteLine(genre_parse);
    }
  }
}
catch(Exception er)
{
    Console.WriteLine("Add genre exceptions:"+er.Message);
}
}

public async Task addComikAuthorDetail(int comik_id,int author_id)
{ string file_name="comik_author.csv";
if(this.list_comik_author_table.Any(item=>item.Length>2 && item[1]==comik_id.ToString()&&item[2]==author_id.ToString()))
{
    return;
}
  latest_comik_author_item+=1;
  string data=$"{latest_comik_author_item},{comik_id},{author_id}";
  this.writeCsvFile(file_name,data);
}

public async Task addComikGenreDetail(int comik_id,int genre_id)
{
    string file_name="comik_genre.csv";
    if(this.list_comik_genre_table.Any(item=>item.Length>2 && item[1]==comik_id.ToString() && item[2]==genre_id.ToString()))
    {
        return;
    }
    this.latest_comik_genre_item+=1;
    string data=$"{this.latest_comik_genre_item},{comik_id},{genre_id}";
    this.writeCsvFile(file_name,data);
}

public int calculator(string val)
{
    string[] nums=val.Split(' ');
    string operators=nums[1];
    int first_nums=Convert.ToInt32(nums[0]);
    int second_nums = Convert.ToInt32(nums[2]);
    int res=-1;
    switch(operators)
    {
        case "+" :
        {   res=first_nums+second_nums;
            break;
        }
        case "-":
        {
            res=first_nums-second_nums;
            break;
        }
        case "*":{
            res=first_nums*second_nums;
            break;            
        }
    }
    return res;
}

 public async Task<InteractionData> topupDrugData(string url)
 {        
    var interactionData = new InteractionData();

 try{
   var playwright = await Playwright.CreateAsync();
        
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        
        var page = await browser.NewPageAsync();

        await page.GotoAsync(url);


        // Get Customer-level interactions (for 'Interactions Between Your Drugs')

        var customerLevel =  page.Locator(".interactions-reference-wrapper").First;
    
       var reference_value = await customerLevel.Locator(".interactions-reference").AllAsync();
          
          foreach(var item in reference_value)
          {

            var reference_wrapper= item.Locator(".interactions-reference-header");
            
            var levelElement =  reference_wrapper.Locator(".ddc-status-label");

            var levelName = await levelElement.TextContentAsync();

            Console.WriteLine("Level element:"+levelName);

            var tagElement=  reference_wrapper.Locator("h3");
    
            var tagContent =  await tagElement.EvaluateAsync<string>(@"(h3) => {
        // Get all child nodes of the <h3> and filter out <svg>
        return Array.from(h3.childNodes)
            .filter(node => node.nodeType === Node.TEXT_NODE) // Keep only text nodes
            .map(node => node.textContent.trim()) // Extract the text content
            .join(' '); // Join into a single string
    }");
           tagContent=tagContent.Trim().Replace(" ","<=>");

        var contentElement = await item.Locator("p").AllAsync();
        
        string text_content="";

        foreach(var content in contentElement)
        {
            var content_text=await content.TextContentAsync();
            if(content_text.Contains("Applies to"))
            {
                continue;
            }
            text_content+=content_text;
        }

        Console.WriteLine("Text content:"+text_content);

        Console.WriteLine("Tag content:"+tagContent);


           if(tagContent.Contains("food"))
           {
             interactionData.DrugAndFoodInteractions.Customer.Add(new InteractionData.Interaction
            {
                Level = levelName,  
                
                Content = text_content,

                Tag=tagContent
            });
           }

        else{
            interactionData.InteractionsBetweenYourDrugs.Customer.Add(new InteractionData.Interaction
            {
                Level = levelName,  
                
                Content = text_content,

                Tag=tagContent
            });
        }
        }



        await page.Locator("a.nav-item", new PageLocatorOptions { HasTextString = "Professional" }).ClickAsync();

        // await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine("did load to this page");
 var professionalLevel =  page.Locator(".interactions-reference-wrapper").First;
    
       var reference_profess_value = await professionalLevel.Locator(".interactions-reference").AllAsync();
          
          foreach(var item in reference_profess_value)
          {

            var reference_wrapper= item.Locator(".interactions-reference-header");
            
            var levelElement =  reference_wrapper.Locator(".ddc-status-label");


            var levelName = await levelElement.TextContentAsync();

            Console.WriteLine("Level element:"+levelName);


            var tagElement=  reference_wrapper.Locator("h3");

           
               
            
            var tagContent =  await tagElement.EvaluateAsync<string>(@"(h3) => {
        // Get all child nodes of the <h3> and filter out <svg>
        return Array.from(h3.childNodes)
            .filter(node => node.nodeType === Node.TEXT_NODE) // Keep only text nodes
            .map(node => node.textContent.trim()) // Extract the text content
            .join(' '); // Join into a single string
    }");
           tagContent=tagContent.Trim().Replace(" ","<=>");

        var contentElement = await item.Locator("p").AllAsync();
        
        string text_content="";

        foreach(var content in contentElement)
        {
            var content_text=await content.TextContentAsync();
            if(content_text.Contains("Applies to"))
            {
                continue;
            }
            text_content+=content_text;
        }

        Console.WriteLine("Text content:"+text_content);

        Console.WriteLine("Tag content:"+tagContent);


           if(tagContent.Contains("food"))
           {
             interactionData.DrugAndFoodInteractions.Professional.Add(new InteractionData.Interaction
            {
                Level = levelName,  
                
                Content = text_content,

                Tag=tagContent
            });
           }

        else{
            interactionData.InteractionsBetweenYourDrugs.Professional.Add(new InteractionData.Interaction
            {
                Level = levelName,  
                
                Content = text_content,

                Tag=tagContent
            });
        }
        }

    //     var professionalLevels = await professionalLevel.Locator(".interactions-reference-wrapper").AllAsync();

    //     foreach (var level in professionalLevels)
    //     {   
    //         var reference = await level.Locator(".interactions-reference").AllAsync();
          
    //       foreach(var item in reference)
    //       {

    //         var reference_wrapper= item.Locator(".interactions-reference-header");
            
    //         var levelElement =  reference_wrapper.Locator(".ddc-status-label");


    //         var levelName = await levelElement.TextContentAsync();

    //         Console.WriteLine("Level element:"+levelName);


    //         var tagElement=  reference_wrapper.Locator("h3");

           
               
            
    //         var tagContent =  await tagElement.EvaluateAsync<string>(@"(h3) => {
    //     // Get all child nodes of the <h3> and filter out <svg>
    //     return Array.from(h3.childNodes)
    //         .filter(node => node.nodeType === Node.TEXT_NODE) // Keep only text nodes
    //         .map(node => node.textContent.trim()) // Extract the text content
    //         .join(' '); // Join into a single string
    // }");
    //        tagContent=tagContent.Trim().Replace(" ","<=>");

    //     var contentElement = await item.Locator("p").AllAsync();
        
    //     string text_content="";

    //     foreach(var content in contentElement)
    //     {
    //         var content_text=await content.TextContentAsync();
    //         if(content_text.Contains("Applies to"))
    //         {
    //             continue;
    //         }
    //         text_content+=content_text;
    //     }

    //     Console.WriteLine("Text content:"+text_content);

    //     Console.WriteLine("Tag content:"+tagContent);


    //        if(tagContent.Contains("food"))
    //        {
    //          interactionData.DrugAndFoodInteractions.Professional.Add(new InteractionData.Interaction
    //         {
    //             Level = levelName,  
                
    //             Content = text_content,

    //             Tag=tagContent
    //         });
    //        }

    //     else{
    //         interactionData.InteractionsBetweenYourDrugs.Pro.Add(new InteractionData.Interaction
    //         {
    //             Level = levelName,  
                
    //             Content = text_content,

    //             Tag=tagContent
    //         });
    //     }
    //     }
    //     }

        // var professionalLevels = await page.QuerySelectorAllAsync(".interactions-table .professional-interaction");
        // foreach (var level in professionalLevels)
        // {
        //     var levelName = await level.QuerySelectorAsync(".interaction-level");
        //     var content = await level.QuerySelectorAsync(".interaction-description");

        //     interactionData.InteractionsBetweenYourDrugs.Professional.Add(new InteractionData.Interaction
        //     {
        //         Level = await levelName.TextContentAsync(),
        //         Content = await content.TextContentAsync()
        //     });
        // }

        // // Get Customer-level drug and food interactions
        // var drugFoodCustomerLevels = await page.QuerySelectorAllAsync(".drug-food-interactions .customer-interaction");
        // foreach (var level in drugFoodCustomerLevels)
        // {
        //     var levelName = await level.QuerySelectorAsync(".interaction-level");
        //     var content = await level.QuerySelectorAsync(".interaction-description");

        //     interactionData.DrugAndFoodInteractions.Customer.Add(new InteractionData.Interaction
        //     {
        //         Level = await levelName.TextContentAsync(),
        //         Content = await content.TextContentAsync()
        //     });
        // }

        // // Get Professional-level drug and food interactions
        // var drugFoodProfessionalLevels = await page.QuerySelectorAllAsync(".drug-food-interactions .professional-interaction");
        // foreach (var level in drugFoodProfessionalLevels)
        // {
        //     var levelName = await level.QuerySelectorAsync(".interaction-level");
        //     var content = await level.QuerySelectorAsync(".interaction-description");

        //     Console.WriteLine("Levename is:"+levelName);

        //     interactionData.DrugAndFoodInteractions.Professional.Add(new InteractionData.Interaction
        //     {
        //         Level = await levelName.TextContentAsync(),
        //         Content = await content.TextContentAsync()
        //     });
        // }

        // Close the browser
        await page.CloseAsync();

        await browser.CloseAsync();
 }
 catch(Exception er)
 {
    Console.WriteLine(er.Message);
 } 
        
        return interactionData;  
 }
 




public async Task topupDataScrapingTest(string url)
{
 
  DateTime start_time=DateTime.MinValue;
  
  while(true)
  { 
try{
   if(DateTime.Now.Subtract(start_time).TotalMinutes>=60)
   {  start_time=DateTime.Now;
    await this.sendMessageToTele("Bắt đầu cào nha các hảo hán.");
    using(var playwright= await Playwright.CreateAsync())
    {
    var browser=await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions{Headless=false});
    var page =await browser.NewPageAsync();
    // await page.AddInitScriptAsync(@"
    //  document.addEventListener('contextmenu', event => event.stopPropagation(), true);
    //   document.body.oncontextmenu = null;
    //     ");  
page.SetDefaultTimeout(60000);

    await page.GotoAsync(url);
    await page.FillAsync("input[name='name']","0917111666a");
    await Task.Delay(1000);
    await page.FillAsync("input[name='password']","BXksRBL7Vds4@2024");
    await Task.Delay(1000);
    string capcha_value=await page.InnerTextAsync("div.text-center.font-weight-bold b");
    string capcha_res=calculator(capcha_value).ToString();
    await page.FillAsync("input[name='mathcaptcha']",capcha_res);
    var button=page.Locator(".btn-style-1").First;
    await button.ClickAsync();    
    await Task.Delay(3000);
        //    await page.AddInitScriptAsync(@"
        //     document.addEventListener('contextmenu', event => event.stopPropagation(), true);
        //     document.body.oncontextmenu = null;
        // ");  

    // await page.EvaluateAsync(@"document.querySelector('div.modal#modal_start').style.display = 'none';");

    //     var displayStyle = await page.EvaluateAsync<string>(@"document.querySelector('div.modal#modal_start').style.display;");
    //     Console.WriteLine($"Modal display style: {displayStyle}");
     await page.WaitForSelectorAsync("div.modal.in");

     await page.ClickAsync("div.modal-footer button.btn.btn-danger[data-dismiss='modal']");


     await page.WaitForTimeoutAsync(1000); 
//     var wait_popup= await page.WaitForSelectorAsync("div.modal-footer");
//     await Task.Delay(1000);
//   if(wait_popup!=null)
//   {
//    var popup= page.Locator("div.modal-footer").First;
//    if(popup!=null)
//    {await page.EvaluateAsync(@"() => {
//     const button = document.querySelector('button.btn.btn-danger[data-dismiss='modal']');
//     button.style.setProperty('content', 'none', 'important');
// }");
//     Console.WriteLine(popup);
//    var butt_count=await popup.Locator("button.btn.btn-danger[data-dismiss='modal']").TextContentAsync();
//    var butt= popup.Locator("button.btn.btn-danger[data-dismiss='modal']");
//    await butt.ClickAsync();
//    Console.WriteLine(butt_count);
//    }
//   }
    
    var new_page =  page.Locator("select#sel_type_card");

    var all_options= new_page.Locator("option");

    var all_option_count=await new_page.Locator("option").CountAsync();

 Dictionary<string,List<string>> data_res=new Dictionary<string, List<string>>();

    if(new_page!=null)
    { 
     Console.WriteLine("here");

      for(int j=1;j<all_option_count;j++)
      { 
    var select_page = page.Locator("select#sel_type_card");
    if(select_page!=null)
    {
        await select_page.ClickAsync();
    }
    var card=all_options.Nth(j);
      
    var text = await card.TextContentAsync();

    Console.WriteLine("text here:"+text);
      
    var value=await card.GetAttributeAsync("value");

    string clean_card="";

    string data_status=await card.GetAttributeAsync("data-status");
      
    string class_card=await card.GetAttributeAsync("class");
      
      clean_card=$"{text}";

    await select_page.SelectOptionAsync(new SelectOptionValue { Value = value });

    await page.WaitForTimeoutAsync(1000);
    
    var select_amount_card=page.Locator("select#sel_amount_card");

    var all_sell_options=select_amount_card.Locator("option");

    var all_option_sell_amount=await select_amount_card.Locator("option").CountAsync();
    Console.WriteLine(all_option_sell_amount);
    string current_title="";
    var title_data=await page.WaitForSelectorAsync("p.desc_note_card");
     current_title=await title_data.TextContentAsync();

    List<string> value_card=new List<string>();

    for(int i=1;i<all_option_sell_amount;i++)
    {   //this place is used to get sub value for card amount
        var re_select_amount=page.Locator("select#sel_amount_card");
        await re_select_amount.ClickAsync();
        var nth_option=all_sell_options.Nth(i);
        var sell_value=await nth_option.GetAttributeAsync("value");
        await re_select_amount.SelectOptionAsync(new SelectOptionValue { Value = sell_value });
        var data_price = await nth_option.GetAttributeAsync("data-price");
        var data_discount = await nth_option.GetAttributeAsync("data-discount");
        var data_context=await nth_option.TextContentAsync();
        Console.WriteLine("Data context:"+data_context);
        string clean_data=$"title_data:{current_title},data_price:{data_price},data_discount:{data_discount}";
       value_card.Add(clean_data);
    }

    data_res.Add(clean_card,value_card);

//   foreach(KeyValuePair<string,List<string>> kp in data_res)
//   {
//     Console.WriteLine(kp.Key+":");
//     foreach(var data in kp.Value)
//     {
//         Console.WriteLine(data);
//     }
//   }
    //     string class_name=await card.GetAttributeAsync("class");
    //    Console.WriteLine(card);
    //     Console.WriteLine(await card.InnerTextAsync());
      }
    }
    
    var type_phones=page.Locator("select#type_phone");

    var type_phone_options=type_phones.Locator("option");

    var count_type_phone_option=await type_phone_options.CountAsync();

  List<string> type_phones_list=new List<string>();

    for(int i=1;i<count_type_phone_option;i++)
    {
      var select_type_phones=page.Locator("select#type_phone");
      if(select_type_phones!=null)
      {
        await select_type_phones.ClickAsync();
      }
      var current_option=type_phone_options.Nth(i);
      string tag_class=await current_option.GetAttributeAsync("class");
      string data_status=await current_option.GetAttributeAsync("data-value");
      string value = await current_option.GetAttributeAsync("value");
      string content = await current_option.TextContentAsync();
      string title="";
     
     await select_type_phones.SelectOptionAsync(new SelectOptionValue { Value = value });

      var title_tag= page.Locator("p.desc_note");
      
      title=await title_tag.TextContentAsync();
     

      int retry=0;

      while(string.IsNullOrEmpty(title) && retry<5)
      { 
        retry+=1;
        
        title_tag=page.Locator("p.desc_note");

        title=await title_tag.TextContentAsync();
        
        await Task.Delay(1000);
      }

     string clean_data=$"value:{value},content:{content},title:{title}";
    

     type_phones_list.Add(clean_data);
    }

    Console.WriteLine("type phone count:"+type_phones_list.Count);
    
    
    using(HttpClient client=new HttpClient())
    {  
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

        Console.WriteLine("Post Url here");

        DataObject data=new DataObject{FirstBlock=data_res,SecondBlock=type_phones_list};
    
        TopupModel topup_model=new TopupModel{data=data,uri="https://cachua.shop"};
        string post_url="https://xulythecao.com/api/crawl-discount-hour";
        string sub_post_url="https://doithecaoonline.com/api/crawl-discount-hour";
        StringContent data_content = new StringContent(JsonConvert.SerializeObject(topup_model),UTF8Encoding.UTF8,"application/json");
        Console.WriteLine(JsonConvert.SerializeObject(topup_model));
        var request = await client.PostAsync(post_url,data_content);
        request.EnsureSuccessStatusCode();
       
        Console.WriteLine("post done"); 
        var response = request.Content.ReadAsStringAsync();

        var request_sub=await client.PostAsync(sub_post_url,data_content);
        
        request_sub.EnsureSuccessStatusCode();

         var status_code=request.StatusCode;
        Console.WriteLine(status_code);
      if(request_sub!=null && request!=null)
      {
        if(!request.IsSuccessStatusCode || !request_sub.IsSuccessStatusCode)
        {  if(!request.IsSuccessStatusCode && request_sub.IsSuccessStatusCode)
        {
            await this.sendMessageToTele("Gửi Failed bên xulythecao cmnr rồi các hảo hán ơi.");
        }
        else if(request.IsSuccessStatusCode && !request_sub.IsSuccessStatusCode )
        {
        await this.sendMessageToTele("Gửi Failed bên doitheocao cmnr rồi các hảo hán ơi.");
        }
        else{
              await this.sendMessageToTele("Gửi Failed bên ở 2 bên hết cmnr rồi các hảo hán ơi.");
        }
        }
        else
        {
         await this.sendMessageToTele("Gửi thành công hết rùi các hảo hán ơi!!!!!.");
        }
      }
      else
      {
          await this.sendMessageToTele("Object null rùi các hảo hán ơi!!!");
      }
    }

    await page.CloseAsync();
    }
  }
  }
 catch(Exception er)
    {
    await this.sendMessageToTele($"Gửi Failed với lỗi message là:"+er.Message);

    Console.WriteLine(er.Message);
    }

 await Task.Delay(100);
  }
    
   
    
}

  
public async Task getGroupChatId()
{
    using(HttpClient client=new HttpClient())
    {
        string url="https://api.telegram.org/bot7554754756:AAFZg4ELlFhpvTfPEIOfZohaBKehRF1n_4k/getUpdates";
        var request = await client.GetAsync(url);
        var response = await request.Content.ReadAsStringAsync();
        Console.WriteLine(response);
    }
}

public async Task sendMessageToTele(string message)
{
    try
    {
    var bot_client = new TelegramBotClient(bot_id);
    await bot_client.SendTextMessageAsync(this.chat_id,message);
    }
    catch(Exception er)
    {
        Console.WriteLine("tele:"+er.Message);
    }
}


public async Task webScrapingTesting(string page_name)
{    
    string comik_table="comik.csv";

    string author_table = "author.csv";

    string genre_table = "genres.csv";

    string comik_author_table = "comik_author.csv";

    string comik_genre_table = "comik_genre.csv";

    string chapter_table = "chapter.csv";

    string chapter_detail_table = "chapter_detail.csv";
           
    fillFullListData(comik_table,list_comik_table);

    fillFullListData(author_table,list_author_table);

    fillFullListData(genre_table,list_genre_table);

    fillFullListData(comik_author_table,list_comik_author_table);  

    fillFullListData(comik_genre_table,list_comik_genre_table);

    fillFullListData(chapter_table,list_chapter_table);

    fillFullListData(chapter_detail_table,list_chapter_detail);  
    
    using(var playwright=await Playwright.CreateAsync())
    {
           
        var browser=await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions{Headless=false});

        var page = await browser.NewPageAsync();

             await page.AddInitScriptAsync(@"
            document.addEventListener('contextmenu', event => event.stopPropagation(), true);
            document.body.oncontextmenu = null;
        ");  

        page.SetDefaultTimeout(60000);
                     
        await page.GotoAsync(page_name);
       // await page.GotoAsync("https://mangakakalot.to/read/the-first-times-lady-68337/en/chapter-1");

        //await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        //await addChapterDetail(page);                        
        var items = await page.Locator(".item").AllAsync();
        
        latest_comik_item=getLatestId(list_comik_table)+1;

        latest_author_item=getLatestId(list_author_table)+1;

        latest_genre_item=getLatestId(list_genre_table)+1;

        latest_comik_author_item=getLatestId(list_comik_author_table);

        latest_comik_genre_item=getLatestId(list_comik_genre_table);

        latest_chapter_item=getLatestId(list_chapter_table);

        latest_chapter_detail_item =getLatestId(list_chapter_detail);

        var page_list_section=page.Locator(".kpage-list").First;

        var page_next=page_list_section.Locator("a[title='Next']");

        Console.WriteLine("latest id:"+latest_comik_item);

        Console.WriteLine("latest chapter id:"+latest_chapter_item);
      while(page_next!=null)
      {
        foreach(var item in items)
        {   //await addComikTable(page,item);
         var link =  item.Locator(".manga-poster").First;

         int count_element=0;
         int count_chapter=0;
         
         var count_chap_content= await item.Locator(".chapter-name").CountAsync();
         Console.WriteLine("Count chap content:"+count_chap_content);
         if(count_chap_content>1)
         {  
            continue;
         }
         var chapter_content=await item.Locator(".chapter-name").TextContentAsync();
         
         string chapt= standardString(chapter_content);
        
         await link.ClickAsync();
         
         var line_content=await page.Locator(".line-content").AllAsync();

         while(line_content==null || line_content.Count < 5)
         {
            await page.ReloadAsync();
            line_content=await page.Locator(".line-content").AllAsync();
            await Task.Delay(100);
         }

        int val= await addComikTable(page,line_content,latest_comik_item,chapt.Trim());
         
        if(val==-1)
        {   
            await page.GoBackAsync();
            await Task.Delay(100);
            continue;
        }
        await addAuthorTable(line_content);

        await addGenreTable(line_content);

        await Task.Delay(2000);
         
        var dropdown = page.Locator("#chap-lang").First;
        
        await dropdown.ClickAsync();

        ILocator element=null;
        var element_count=await page.Locator("a[data-code='en'][data-type='chap']").CountAsync();
        if(element_count==0)
        {
        element = page.Locator("a[data-code='ja'][data-type='chap']").First;
        }
        else{
         element = page.Locator("a[data-code='en'][data-type='chap']").First;
        }

         if(element==null)
        {
        await page.GoBackAsync();
        continue;
        }

    try{   
    //    while(element==null && count_element<3)
    //    {count_element+=1;
    //     element=page.Locator("a[data-code='ja'][data-type='chap']").First;
    //    } 
    //    if(element==null)
    //    {
    //     await page.GoBackAsync();
    //     continue;
    //    }
        await element.ClickAsync();
    }
    catch(TimeoutException er)
    {  Console.WriteLine("time out exception here:"+er.Message);
    }

        await Task.Delay(3000);

        var count_chapters=await page.Locator(".clb-ul#list-chapter-en").CountAsync();

        ILocator chapters=null;

        if(count_chapters>0)
        {
        chapters=page.Locator(".clb-ul#list-chapter-en").First;
        }
       
       else
       {
        chapters=page.Locator(".clb-ul#list-chapter-ja").First;
       }

        if(chapters==null)
        {
        await page.GoBackAsync();
        continue;
        }
       

        var chaptr=await chapters.Locator(".item").AllAsync();

        foreach(var chapter in chaptr)
        {  
            var chapter_no=await chapter.Locator(".chapter-name").TextContentAsync();
            var chapter_link=chapter.Locator("a").First;
            string chapter_no_parse=standardString(chapter_no);
            Console.WriteLine("CHAPTER:"+chapter_no_parse);            
            DateTime date_time_now=DateTime.Now;

            string date_added = date_time_now.ToString("dd/MM/yyyy HH:mm:ss");
           Console.WriteLine("latest chapter item:"+latest_chapter_item);
            if(this.list_chapter_table.Any(ch=>ch.Length>2 && ch[1]==chapter_no_parse && ch[2]==latest_comik_item.ToString()))
            {  Console.WriteLine(chapter_no_parse);
                continue;
            }

            latest_chapter_item+=1;

            Console.WriteLine("CHpter:"+latest_chapter_item);            
            
            string chapter_data=$"{latest_chapter_item},{chapter_no_parse},{latest_comik_item},{this.handleEscapedField(date_added)}";
            
            this.writeCsvFile(chapter_table,chapter_data);
          
            await chapter_link.ClickAsync();

               await page.AddInitScriptAsync(@"
            document.addEventListener('contextmenu', event => event.stopPropagation(), true);
            document.body.oncontextmenu = null;");
            
            await addChapterDetail(page,latest_chapter_item,chapter_detail_table);

            await dropdown.ClickAsync();

            await Task.Delay(100);

            await element.ClickAsync();
            
            await Task.Delay(100);            
        }
         
          latest_comik_item+=1;
       // await addGenreTable(page,line_content);
         await page.GoBackAsync();
        }
        await page_next.ClickAsync();

        page_list_section=page.Locator(".kpage-list").First;

        page_next=page_list_section.Locator("a[title='Next']");
      }
        // await page.GetByRole(AriaRole.Link,new(){Name="Browse"}).ClickAsync();
        // await page.ScreenshotAsync(new PageScreenshotOptions{Path="screenshot.png"});    
        await browser.CloseAsync();        
    }
}

}