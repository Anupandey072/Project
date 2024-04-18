
using Microsoft.AspNetCore.Authorization;
using MySql.Data.MySqlClient;

namespace Project.services
{
    public class UpdateAllData
    {
        dbServices ds = new dbServices();

        [Authorize]
        public async Task<responseData> UpdateAll(requestData req)
        {
            responseData resData= new responseData();
            resData.rData["rCode"]=0;
            try
            {
                 string name = req.addInfo["Name"].ToString();
                 string email = req.addInfo["Email"].ToString();
                 string mobile = req.addInfo["Mobile"].ToString();
                //  string password = req.addInfo["Password"].ToString();//create new api
                MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@ID",req.addInfo["ID"]),
                new MySqlParameter("@Name", name),
                new MySqlParameter("@Email", email),
                new MySqlParameter("@Mobile", mobile),
                new MySqlParameter("@Profile_Image", req.addInfo["Profile_Image"].ToString()),
                new MySqlParameter("@Adhar", req.addInfo["Adhar"].ToString()),
                new MySqlParameter("@Pancard", req.addInfo["Pancard"].ToString()),
                new MySqlParameter("@Status", 1),
                }; 
                var sq = $"update User_Register set Name=@Name,Email=@Email,Mobile=@Mobile,Profile_Image=@Profile_Image,Status=@Status where ID=@ID;";
                var data = ds.ExecuteInsertAndGetLastId(sq, myParams);
                
                if (data!=null)
                {
                    var query = $"insert into pc_student.User_RegisterDoc(Document,U_ID,Status) values(@Adhar,@ID,@Status);";
                    var querys = ds.executeSQL(query, myParams);
                    var que = $"insert into pc_student.User_RegisterDoc(Document,U_ID,Status) values(@Pancard,@ID,@Status);";
                    var ques = ds.executeSQL(que, myParams);
                
                if (querys!=null && ques !=null)
                {
                    resData.eventID = req.eventID;
                    resData.rData["rMessage"] = "Updation Successfully";
                }
                else
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Document not Updated";

                }
                    
                }
                else
                {
                   
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "User Data not saved";
                }
           
            }
            catch (Exception ex)
            {
                resData.rData["rCode"]=1;
                resData.rData["rMessage"]=ex.Message;
            }
            return resData;
        }
        [Authorize]
        public async Task<responseData> UpdatePassword(requestData req)
        {
            responseData resData= new responseData();
            resData.rData["rCode"]=0;
            try
            {
                 string OldPassword = req.addInfo["OldPassword"].ToString();
                string mobile = req.addInfo["Mobile"].ToString();
                string NewPassword = req.addInfo["NewPassword"].ToString();
               
                MySqlParameter[] myParams = new MySqlParameter[] {

                new MySqlParameter("@ID",req.addInfo["ID"]),
                new MySqlParameter("@OldPassword",OldPassword),
                new MySqlParameter("@Mobile",mobile),
                new MySqlParameter("@NewPassword",NewPassword),
                }; 
                var exist=$"select * from User_Register where Mobile=@Mobile And Password=@OldPassword;";
                var exists = ds.executeSQL(exist, myParams);
                if(exists!=null && exists[0].Count()>0)
                {
                    var sq = $"update User_Register set Password=@NewPassword where ID=@ID;";
                     var data = ds.executeSQL(sq, myParams);
                
                if (data!=null)
                {
                    resData.eventID = req.eventID;
                    resData.rData["rMessage"] = "Password Updated Successfully";
                }
                else
                {
                  resData.rData["rCode"] = 1;
                  resData.rData["rMessage"] = "Invalid Credentials";  
                } 
                }
                else{
                    resData.rData["rMessage"] = "Mobile Number and Old Password is Invalid"; 
                    resData.rData["rCode"]=1;
                }
                
            }
            catch (Exception ex)
            {
                resData.rData["rCode"]=1;
                resData.rData["rMessage"]=ex.Message;
            }
            return resData;
        }
        [Authorize]
public async Task<responseData> UpdateDetails(requestData req)
        {
            responseData resData= new responseData();
            resData.rData["rCode"]=0;
            try
            {
                string name = req.addInfo["Name"].ToString();
                string email = req.addInfo["Email"].ToString();
                string mobile = req.addInfo["Mobile"].ToString();
                string image = req.addInfo["Profile_Image"].ToString();
                MySqlParameter[] myParams = new MySqlParameter[] {

                new MySqlParameter("@ID",req.addInfo["ID"]),
                new MySqlParameter("@Name",name),
                new MySqlParameter("@Email",email),
                new MySqlParameter("@Mobile",mobile),
                new MySqlParameter("@Profile_Image",image),
                new MySqlParameter("@Status",1),
                }; 
                var update1=$"update User_Register set Name=@Name,Email=@Email,Mobile=@Mobile,Profile_Image=@Profile_Image,Status=@Status where ID=@ID";
                var data1 = ds.executeSQL(update1, myParams);
                if(data1[0].Count()>0)
                {
                    
                    resData.eventID = req.eventID;
                    resData.rData["rMessage"] = "Failed";
                }
                else{
                    
                    resData.rData["rCode"]=0;
                    resData.rData["rMessage"] = "Details Updated Successfully";
                }
                
            }
            catch (Exception ex)
            {
                resData.rData["rCode"]=1;
                resData.rData["rMessage"]=ex.Message;
            }
            return resData;
        }
        
    }
}