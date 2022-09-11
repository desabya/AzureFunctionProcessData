using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FnProcessFiles
{
    public class ProcessFiles
    {
        private readonly IConfiguration _configuration;

        public ProcessFiles(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [FunctionName("FunctionProcessFiles")]
        public async Task Run([ServiceBusTrigger("manpowersourcequeue", Connection = "AzureServiceBus")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            var result = JsonConvert.DeserializeObject<EmployeeRecord>(myQueueItem);
            
            //Create object 
            EmployeeDetails details = new EmployeeDetails();
            details.firstName = result.firstName;
            details.lastName = result.lastName;
            details.phoneNumber = result.phoneNumber;

            EmployeeProfesionalDetails employeeProfesionalDetails = new EmployeeProfesionalDetails();
            employeeProfesionalDetails.preferredFullName = result.preferredFullName;    
            employeeProfesionalDetails.jobTitleName= result.jobTitleName;
            employeeProfesionalDetails.emailAddress= result.emailAddress;
            employeeProfesionalDetails.employeeCode = result.employeeCode;

            var client = new ServiceBusClient(_configuration["AzureServiceBus"].ToString());
            var sender = client.CreateSender("manpowerdestqueue");
            //Create JSON
            var result1=JsonConvert.SerializeObject(details);
            var result2 = JsonConvert.SerializeObject(employeeProfesionalDetails);
            var msg1 = new ServiceBusMessage(result1);
            var msg2 = new ServiceBusMessage(result2);
            await sender.SendMessageAsync(msg1);
            await sender.SendMessageAsync(msg2);


        }
    }
}
