using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Project.services;
WebHost.CreateDefaultBuilder().
ConfigureServices(s=>
{
    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    s.AddSingleton<login>();
    s.AddSingleton<Dashboard>();
    s.AddSingleton<UpdateAllData>();
     s.AddSingleton<DeleteData>();
s.AddAuthorization();
s.AddControllers();
s.AddCors();
}).Configure(app=>
{
 app.UseCors(options =>
         options.WithOrigins("https://localhost:5002", "http://localhost:5001")
         .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseRouting();
app.UseStaticFiles();

app.UseEndpoints(e=>
{
           var login=  e.ServiceProvider.GetRequiredService<login>();
           var dash=  e.ServiceProvider.GetRequiredService<Dashboard>();
           var updateall=  e.ServiceProvider.GetRequiredService<UpdateAllData>();
           var del=  e.ServiceProvider.GetRequiredService<DeleteData>();

 e.MapPost("login",
         [AllowAnonymous] async (HttpContext http) =>
         {
             var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
             requestData rData = JsonSerializer.Deserialize<requestData>(body);
              if (rData.eventID == "1001") // update
                await http.Response.WriteAsJsonAsync(await login.Login(rData));//doubt
                  else if (rData.eventID == "1002") // update
                   await http.Response.WriteAsJsonAsync(await login.Register(rData));
                   else if (rData.eventID == "1003") // update
                   await http.Response.WriteAsJsonAsync(await login.InsertDocument(rData));

         });
          e.MapPost("dashboard",
         [AllowAnonymous] async (HttpContext http) =>
         {
             var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
             requestData rData = JsonSerializer.Deserialize<requestData>(body);
                 if (rData.eventID == "1001") // update
                   await http.Response.WriteAsJsonAsync(await dash.GetAll(rData));
                  else if (rData.eventID == "1002") // update
                   await http.Response.WriteAsJsonAsync(await dash.GetAllDoc(rData));

         });
       
         e.MapPost("updateData",
         [AllowAnonymous] async (HttpContext http) =>
         {
             var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
             requestData rData = JsonSerializer.Deserialize<requestData>(body);
              if (rData.eventID == "1001") // update
                         await http.Response.WriteAsJsonAsync(await updateall.UpdateAll(rData)); 
               else if (rData.eventID == "1002") // update
                         await http.Response.WriteAsJsonAsync(await updateall.UpdatePassword(rData)); 
                else if (rData.eventID == "1003") // update
                         await http.Response.WriteAsJsonAsync(await updateall.UpdateDetails(rData));        

         });
         e.MapPost("DeleteData",
         [AllowAnonymous] async (HttpContext http) =>
         {
             var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
             requestData rData = JsonSerializer.Deserialize<requestData>(body);
              if (rData.eventID == "1001") // update
                         await http.Response.WriteAsJsonAsync(await del.deleteDetails(rData));  
                else if (rData.eventID == "1002") // update
                         await http.Response.WriteAsJsonAsync(await del.deleteDocument(rData));  
         });
});
}).Build().Run();
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
public record requestData
{
    [Required]
    public string eventID { get; set; }
    [Required]
    public IDictionary<string, object> addInfo { get; set; }
}

public record responseData
{
    public responseData()
    {
        eventID = "";
        rStatus = 0;
        rData = new Dictionary<string, object>();
    }
    [Required]
    public int rStatus { get; set; } = 0;
    public string eventID { get; set; }
    public IDictionary<string, object> addInfo { get; set; }
    public IDictionary<string, object> rData { get; set; }
}
