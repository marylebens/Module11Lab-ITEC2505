using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace RubberDuckStore.Pages
{
    // This is a PageModel for a Razor page that handles 
    // displaying Rubber duck products for the duck store.
    public class RubberDucksModel : PageModel
    {
        //Property that will store the duck ID of the selected duck from the form
        [BindProperty]
        public int SelectedDuckId { get; set; }

        //List that will hold all of the ducks for the dropdown list on the form
        public List<SelectListItem> DuckList { get; set; }

        //Property that will store the currently selected duck object
        public Duck SelectedDuck { get; set; }

        // Handles HTTP GET requests to the page. 
        // It initializes the DuckList with all ducks from the database.
        public void OnGet()
        {
            LoadDuckList();
        }

        // Handles HTTP Post requests from the form (when the user selects a duck)
        //Load the duck list & retrieve the selected duck's details
        public IActionResult OnPost()
        {   
            // Call the load duck list method
            LoadDuckList(); // Ensure the duck list is populated for redisplay

            // Error handling - make sure there is a valid DuckID
            if (SelectedDuckId != 0)
            {
                SelectedDuck = GetDuckById(SelectedDuckId);
            } //end if

            // Return the page so it can be displayed in the browser
            return Page();

        } //end on Post

        // Helper method to load the ducks from the SQLite database
        // for displaying in the drop down list
        private void LoadDuckList(){
            //Create a new list
            DuckList = new List<SelectListItem>();

            // Create a connection to the SQLite database
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db")){
                //Open the connection
                connection.Open();

                //Create a SQL command and set up our SQL query to select all ducks
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Ducks";

                //Create a reader to read the results of the query
                using (var reader = command.ExecuteReader())
                {
                   //While loop to iterate through all of the records in the result
                   //set from the database
                   while (reader.Read())
                   {
                        //Create a new list item for the current duck from the resultset
                        // and add that duck to the dropdown list
                        DuckList.Add(new SelectListItem{
                            Value = reader.GetInt32(0).ToString(), //Duck ID as the value
                            Text = reader.GetString(1) //Duck Name as the text
                        });
                   } //end while
                } //end using reader
            } //end using
        } //end method

        // Helper method retrieves a duck by its ID from the database
        // Returns all details of the duck - returns a Duck object
        private Duck GetDuckById(int id){
            //Create a database connection 
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                //Open a connection
                connection.Open();

                //Create a command and execute the SQL query to retrieve
                //the record for the selected duck ID
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Ducks WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id); //Using a parameterized query to prevent SQL injection

                //Create a reader to reader the resultset from the query
                using (var reader = command.ExecuteReader()){
                    // If the reader has another record, then read it (there should only be one 
                    //since the primary key is unique)
                    if(reader.Read())
                    {
                        return new Duck {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageFileName = reader.GetString(4)
                        };
                    } //end if
                } //end using
            } //end using 
            return null; // Return null if no duck is found with the given ID
        } //end method
    } //end model class

    // Simple class representing a Rubber Duck product for the store
    public class Duck {
        public int Id { get; set; }
        public string Name {get; set; }
        public string Description {get; set; }
        public decimal Price {get; set; }
        public string ImageFileName {get; set; }
    } //end class 
}
