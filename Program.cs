using MyFirstProgram.UI;

//
// Requirements:
//
// 1. You need to create a Math game containing the 4 basic operations
// 2. The divisions should result on INTEGERS ONLY and dividends should go from 0 to 100. Example: Your app shouldn't present the division 7/2 to the user, since it doesn't result in an integer.
// 3. Users should be presented with a menu to choose an operation
// 4. You should record previous games in a List and there should be an option in the menu for the user to visualize a history of previous games.
// 5. You don't need to record results on a database. Once the program is closed the results will be deleted.
//

Console.Clear();

Console.WriteLine("-------------------------------------------------------------------");
Console.WriteLine("Welcome New User to the Math Game!");
Console.WriteLine("-------------------------------------------------------------------\n");   

//
// Simple Login/Authorization 
//

Console.Write("Please enter your name to begin: ");
string? userName = Console.ReadLine();

if (string.IsNullOrEmpty(userName))
{
    Console.WriteLine("You must enter a name to play!");
    Environment.Exit(-1);;
}

//
// Main Dialog loop
//

var UserInterface = new UI();
UserInterface.ShowMainDialog(userName);

//
// End of Programme
//

Environment.Exit(0);

