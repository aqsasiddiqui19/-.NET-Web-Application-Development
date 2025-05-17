using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce_API_Project.Models
{
    public class Price
    {
        [Key]
        public Guid PriceId { get; set; } = Guid.NewGuid();  

        [Precision(18, 4)]  // Ensures DB stores 18-digit numbers with 4 decimal places
        public decimal BasePrice { get; set; } // Original Price

        [Precision(18, 4)]
        public decimal TaxPercentage { get; set; } // Tax Percentage 

        [Precision(18, 4)]  
        public decimal DiscountPercentage { get; set; } // Discount Percentage

        [Precision(18, 4)]
        public decimal DiscountedPrice { get; set; } // Price after Discount

        [Precision(18, 4)]  
        public decimal FinalAmount { get; set; } // Final Price after Tax and Discount
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Auto-set when created
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; } // NULL means it's the current price
        public Guid AdminId { get; set; }        // FK to Admin
    }
}






//Note:
//Difference
//    Newtonsoft.Json by default handles reference loops, 
//    but System.Text.Json throws exceptions if there's a loop unless configured

//JSON Serialization:
//JSON serialization is the process of converting an object
//(such as a class or data structure) into a JSON (JavaScript Object Notation) format,
//which is a lightweight data-interchange format that's easy to read and write for humans,
//and easy to parse and generate for machines.


//In simple words:
//Serialization = Converting an object to a JSON string
//Deserialization = Converting a JSON string back into an  object.

//EXAMPLE:]

//using Newtonsoft.Json;

//public class Person
//{
//    public string Name { get; set; }
//    public int Age { get; set; }
//}

//var person = new Person { Name = "T", Age = 25 };
//string json = JsonConvert.SerializeObject(person);

//Console.WriteLine(json);  // Output: {"Name":"T","Age":25}

//DESERIALZATION:

//string json = "{\"Name\":\"T\",\"Age\":25}";
//Person person = JsonConvert.DeserializeObject<Person>(json);

//Console.WriteLine(person.Name);  // Output: T
