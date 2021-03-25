using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using NModbus;
using NModbus.SerialPortStream;
using RestSharp;
using RJCP.IO.Ports;
namespace ZoomandTeamsintegeration
{
    class Program
    {
        static async Task Main(string[] args)
        {
           
            // CreateZoom();
            await Teams();
        }


        public static async Task Teams()
        {
            try
            {
                
                var publicClientApplication = PublicClientApplicationBuilder
                .Create("935744b0-3943-4521-8e57-54bcbf493de1")
                .WithTenantId("5be4ddfe-d46d-4e46-8c80-a26488d9a39c")
                .WithRedirectUri("http://localhost")
                .Build();
                
                InteractiveAuthenticationProvider authProvider = new InteractiveAuthenticationProvider(publicClientApplication, new[] { "Directory.ReadWrite.All", "User.Read", "Group.ReadWrite.All","Team.Create", "OnlineMeetings.ReadWrite" });
                GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                /*  var team = new Team
                  {
                      DisplayName = "My Sample Team",
                      Description = "My Sample Team’s Description",
                      AdditionalData = new Dictionary<string, object>()
                  {
                      {"template@odata.bind", "https://graph.microsoft.com/v1.0/teamsTemplates('standard')"},

                  }
                  };*/
                

                var subject = "Create a meeting with customId provided";

                var externalId = "7eb8263f-d0e0-4149-bb1c-1f0476083c56";

                var participants = new MeetingParticipants
                {
                    Attendees = new List<MeetingParticipantInfo>()
                        {
                            new MeetingParticipantInfo
                            {
                                Identity = new IdentitySet
                                {
                                    User = new Identity
                                    {
                                       
                                        Id = "e527ded8-6be2-4d76-aca6-579a66ce9ec9"
                                    }
                                },
                                Upn = "admin@shoaibshahid11111gmail.onmicrosoft.com"
                            }
                        }
                };

               var ue= await graphClient.Me.OnlineMeetings
                    .CreateOrGet(externalId, null,null, participants, null, subject)
                    .Request()
                    .PostAsync();
                
                // await graphClient.Teams.Request().AddAsync(team);
                // var ue = await graphClient.Me.Request().GetAsync();

                Console.WriteLine(ue.JoinWebUrl);

            }
            catch (Exception)
            {

                throw;
            }

            
        }


        public static void CreateZoom()
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var apiSecret = "U3DLqNBrcYPG3zUrqYyjRjFsaZSHroIXLtHO";
            byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "U3DLqNBrcYPG3zUrqYyjRjFsaZSHroIXLtHO",
                Expires = now.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJhdWQiOm51bGwsImlzcyI6IlpHcEpGZzdYUVU2R0VlcHJTbGVrYnciLCJleHAiOjE2MDE1Mzc2ODYsImlhdCI6MTYwMTQ1MTI5Mn0.mF0KbGOuoGm7uuwE93zcICbhu1kCem8U-ljsj70fhpA";
                //tokenHandler.WriteToken(token);
            var client = new RestClient("https://api.zoom.us/v2/users/me/meetings");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
            request.AddParameter("application/json", "{\r\n    \"topic\": \"test\",\r\n    \"type\": \"1\",\r\n    \"password\": \"12345\",\r\n    \"agenda\": \"testing\",\r\n    \"settings\": {\r\n        \"host_video\": \"true\",\r\n        \"participant_video\": \"true\",\r\n        \"cn_meeting\": \"false\",\r\n        \"in_meeting\": \"false\",\r\n        \"mute_upon_entry\": \"false\",\r\n        \"watermark\": \"false\",\r\n        \"use_pmi\": \"false\",\r\n        \"approval_type\": \"2\",\r\n        \"audio\": \"both\",\r\n        \"auto_recording\": \"none\"\r\n    }\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
       
    }
}
