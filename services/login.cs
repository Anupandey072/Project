using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace Project.services
{
    public class login
    {
        dbServices ds = new dbServices(); 
        public async Task<responseData> Login(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                string input = req.addInfo["UserId"].ToString();
                bool isEmail = IsValidEmail(input);
                bool isMobile = IsValidMobile(input);
                string columnName;
                if (isEmail)
                {
                    columnName = "Email";
                }
                else if (isMobile)
                {
                    columnName = "Mobile";
                }
                else
                {
                    columnName = "";
                }

                MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@UserId", input),
                new MySqlParameter("@Password", req.addInfo["Password"].ToString())

                };
                var sq = $"SELECT * FROM User_Register WHERE {columnName}=@UserId AND Password = @Password AND Status>0;";
                var data = ds.ExecuteSQLName(sq, myParams);

                if (data == null)
                {

                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Invalid Credentials";
                }
                else
                {
                    var claims = new[]
       {
            new Claim(ClaimTypes.NameIdentifier, data[0][0]["Email"].ToString()), // Assuming ID is the user identifier
            new Claim(ClaimTypes.NameIdentifier, data[0][0]["Mobile"].ToString()),
            // Add other claims as needed, like Name, Email, etc.
        };

                    // Key for signing the token, you should keep this secure and secret
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Yh2k7QSu4l8CZg5p6X3Pna9L0Miy4D3Bvt0JVr87UcOj69Kqw5R2Nmf4FWs03Hdx"));

                    // Create the token
                    var token = new JwtSecurityToken(
                        issuer: "Source_Authentication_Service",
                        audience: "Source_Micorservices",
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(600), // Token expiry time
                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                    // Generate the token
                    string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                    // Return the generated token
                   


                    // resData.eventID = req.eventID;
                     resData.rData["rMessage"] = "Login Successfully";
                    resData.rData["ID"] = data[0][0]["ID"];
                    resData.rData["Name"] = data[0][0]["Name"];
                    resData.rData["Mobile"] = data[0][0]["Mobile"];
                    resData.rData["Email"] = data[0][0]["Email"];
                    resData.rData["Profile_Image"] = data[0][0]["Profile_Image"];
                    resData.rData["jwtToken"]=jwtToken;
                }
             
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex.Message;
            }
            return resData;

        }
        public async Task<responseData> Register(requestData req)
            {
            responseData resData= new responseData();
            resData.rData["rCode"]=0;
            try
            {
                string email=req.addInfo["Email"].ToString();
                string mobile=req.addInfo["Mobile"].ToString();
                bool validemail=IsValidEmail(email);
                bool validmobile=IsValidEmail(mobile);
                string emailid;
                string mobileno;
                if(validemail && validmobile)
                {
                    emailid="Email";
                    mobileno="Mobile";
                    MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@Name", req.addInfo["Name"].ToString()),
                new MySqlParameter("@Email",validemail),
                new MySqlParameter("@Mobile",validmobile),
                new MySqlParameter("@Password", req.addInfo["Password"].ToString()),
                new MySqlParameter("@Status",1),
                new MySqlParameter("@Profile_Image", req.addInfo["Profile_Image"].ToString()),
                new MySqlParameter("@Adharcard", req.addInfo["Adharcard"].ToString()),
                new MySqlParameter("@Pancard", req.addInfo["Pancard"].ToString()),
                };
            var exist=$"select * from pc_student.User_Register where Mobile=@Mobile OR Email=@Email;";
             var exists = ds.executeSQL(exist, myParams);
            if(exists!=null && exists[0].Count()>0)
            {
                resData.rData["rMessage"] = "Already Exists";
            }
            else {
                    var sq = @"insert into pc_student.User_Register(Name,Email,Mobile,Password,Profile_Image,Status) values(@Name,@emailid,@mobileno,@Password,@Profile_Image,@Status);";
                var data = ds.ExecuteInsertAndGetLastId(sq, myParams);
                
                if (data!=null)
                {
                    var query = $"insert into pc_student.User_RegisterDoc(Document,U_ID,Status) values(@Adharcard,{data},@Status);";
                    var querys = ds.executeSQL(query, myParams);
                    var que = $"insert into pc_student.User_RegisterDoc(Document,U_ID,Status) values(@Pancard,{data},@Status);";
                    var ques = ds.executeSQL(que, myParams);
                
                if (querys==null && ques ==null)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Document not saved";
                }
                else
                {

                    resData.eventID = req.eventID;
                    resData.rData["rMessage"] = "Registration Successfully";

                }
                    
                }
                else
                {
                   
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "User Data not saved";
                } 
            }
                    
                }
                else{
                   resData.rData["rMessage"] = "Please Enter Valid Email and Mobile Number"; 
                }
                
               
            }
                 catch (Exception ex)
                 {
                resData.rData["rCode"]=1;
                resData.rData["rMessage"]=ex.Message;
            }
           return resData;
        }
   
    public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
        public static bool IsValidMobile(string mobile)
        {
            string pattern = @"^[0-9]{7,15}$";
            return Regex.IsMatch(mobile, pattern);
        }


        //  public async Task<responseData> InsertDocument(requestData req)
        // {
        //     responseData resData= new responseData();
        //     resData.rData["rCode"]=0;
        //     try
        //     {
        //         string img=req.addInfo["Doc_Img"].ToString();
        //         MySqlParameter[] myParams = new MySqlParameter[] {
        //         new MySqlParameter("@U_ID", req.addInfo["U_ID"]),
        //         new MySqlParameter("@Status",1),
        //         new MySqlParameter("@Doc_Img",img),
        //         };
        //         var query = $"insert into pc_student.User_RegisterDoc(Document,U_ID,Status) values(@Doc_Img,@U_ID,@Status);";
        //             var querys = ds.executeSQL(query, myParams);
        //             if (querys==null)
        //         {
        //             resData.rData["rCode"] = 1;
        //             resData.rData["rMessage"] = "Document not Saved";
        //         }
        //         else
        //         {

        //             resData.eventID = req.eventID;
        //             resData.rData["rMessage"] = "Document Saved Successfully";

        //         }
        //     }
        //          catch (Exception ex)
        //          {
        //         resData.rData["rCode"]=1;
        //         resData.rData["rMessage"]=ex.Message;
        //     }
        //    return resData;
        // }


        public async Task<responseData> InsertDocument(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;

            try
            {
                var doc = req.addInfo["Doc_Img"].ToString();
                var Document = JArray.Parse(doc);
                foreach (var imageName in Document)
                {
                    MySqlParameter[] myParams = new MySqlParameter[] {
                        new MySqlParameter("@U_ID",req.addInfo["U_ID"]),
                        new MySqlParameter("@Status", 1),
                        new MySqlParameter("@Doc_Img", imageName["Doc_Img"]) // Save the image name
                    };

                    var query = $"INSERT INTO pc_student.User_RegisterDoc(Document, U_ID, Status) VALUES(@Doc_Img, @U_ID, @Status);";
                    var queryResult = ds.executeSQL(query, myParams);

                    if (queryResult == null)
                    {
                        resData.rData["rCode"] = 1;
                        resData.rData["rMessage"] = "Document not Saved";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rMessage"] = "Document Saved Successfully";
                    }
                }


            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex.Message;
            }
            return resData;
        }


    }
}