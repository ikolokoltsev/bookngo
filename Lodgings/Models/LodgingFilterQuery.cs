namespace server.Lodgings.Models;

// TODO: replace enum, because parsing enums is pain
public class LodgingFilterQuery
{
      public double? MinPrice { get; set; }
      public double? MaxPrice { get; set; }
      
      public double? MinRating { get; set; }
      
      public LodgingStatus? Status { get; set; }
      public string? SearchTerm { get; set; }
}




//GET /lodging/filter?MinPrice=100
// Status får defaultvärdet: Available (0)
// Filtrerar ALLTID på Status = Available

// GET /lodging/filter?MinPrice=100
// Status är null
// Filtrerar INTE på Status alls ✅






// ### Hämta alla lodgings
// GET http://localhost:5000/lodging

// ### Test 1: Filter på MinPrice
// GET http://localhost:5000/lodging/filter?MinPrice=100

// ### Test 2: Filter på MaxPrice
// GET http://localhost:5000/lodging/filter?MaxPrice=150

// ### Test 3: Filter på MinRating
// GET http://localhost:5000/lodging/filter?MinRating=4.5

// ### Test 4: Filter på Status - Available
// GET http://localhost:5000/lodging/filter?Status=Available

// ### Test 5: Filter på Status - Booked
// GET http://localhost:5000/lodging/filter?Status=Booked

// ### Test 6: Sök efter stad
// GET http://localhost:5000/lodging/filter?SearchTerm=miami

// ### Test 7: Sök efter adress
// GET http://localhost:5000/lodging/filter?SearchTerm=lake

// ### Test 8: Prisintervall
// GET http://localhost:5000/lodging/filter?MinPrice=100&MaxPrice=180

// ### Test 9: Kombinera pris och rating
// GET http://localhost:5000/lodging/filter?MinPrice=100&MinRating=4.5

// ### Test 10: Alla filter samtidigt
// GET http://localhost:5000/lodging/filter?MinPrice=100&MaxPrice=180&MinRating=4.5&Status=Available&SearchTerm=lake

// ### Test 11: Inga filter (ska ge alla)
// GET http://localhost:5000/lodging/filter

// ### Test 12: Omöjlig kombination (ska ge tom lista)
// GET http://localhost:5000/lodging/filter?MinPrice=1000&MaxPrice=50