
using Microsoft.AspNetCore.Authorization;
using MySql.Data.MySqlClient;

namespace Project.services
{
    public class DeleteData
    {
        dbServices ds = new dbServices();
        [Authorize]
        public async Task<responseData> deleteDetails(requestData req)
        {
            responseData resData= new responseData();
            resData.rData["rCode"]=0;
            try
            {
                MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@ID",req.addInfo["ID"]),
                new MySqlParameter("@Status", 0),
                }; 
                var sq = $"update User_Register set Status=@Status where ID=@ID;";
                var data = ds.executeSQL(sq, myParams);
                
                if (data!=null)
                {
                    
                    var query = $"update User_RegisterDoc set Status=@Status where U_ID=@ID;";
                    var querys = ds.executeSQL(query, myParams);
                    if(querys!=null)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rMessage"] = "Deleted Successfully";
                    }
                    else 
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rMessage"] = "Failed Document Deletion"; 
                    }
                    resData.rData["rMessage"] = "Deleted Successfully";
                    
                }
                else
                {
                   
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Failed";
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
         public async Task<responseData> deleteDocument(requestData req)
        {
            responseData resData= new responseData();
            resData.rData["rCode"]=0;
            try
            {
                MySqlParameter[] myParams = new MySqlParameter[] {
                    new MySqlParameter("@ID",req.addInfo["ID"]),
                new MySqlParameter("@Status", 0),
                }; 
                var sq = $"update User_RegisterDoc set Status=@Status where D_ID=@ID;";
                var data = ds.executeSQL(sq, myParams);
                
                if (data!=null)
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Document Deleted Successfully";
                     
                }
                else
                {
                   
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Failed";
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