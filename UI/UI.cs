using Spectre.Console;

using MyFirstProgram.Enums;
using MyFirstProgram.Models;
using MyFirstProgram.Respository;


namespace MyFirstProgram.UI;
public class UI
{

    internal void ShowMainDialog(string userName)
    {
        DifficultyLevel level = DifficultyLevel.Easy;
        GamesDatabase gamesDatabase = new();
        var random = new Random();

        int gameNumber = 0;
        int gameWins = 0;

        while (true)
        {
            Console.Clear();

            //
            // Header
            //

            var table = new Table();

            table.Title = new TableTitle($"[bold]MathGame - Round {(gameNumber+1)}[/]");
            
            table.AddColumn("").AddColumns("");
            
            table.AddRow($" Current Player: [yellow]{userName}[/]",$" Diffculty Level: [yellow]{level}[/]");

            if (gameNumber > 0)
            {
                table.AddRow($" Games Won: [yellow]{gameNumber}[/]",$" Games Lost: [red]{gameNumber-gameWins}[/]");
            }

            table.Width = 80;
            table.HideHeaders();
            table.Border(TableBorder.Rounded);

            AnsiConsole.Write(table);

            //
            // Menu
            //

            var enumToFriendlyString = Enum.GetValues<GameOptions>()
                .ToDictionary(e => e.ToFriendlyString(), e => e);

            var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                            .Title("[yellow]Select option:[/]")
                            .AddChoices(enumToFriendlyString.Keys)
            );

            var selectedOption = enumToFriendlyString[choice];

            //
            // Process non-games options
            //

            if (selectedOption == GameOptions.ExitProgram)
            {
                AnsiConsole.MarkupLine("[red]Goodbye![/]");
                return;
            }
            else if (selectedOption == GameOptions.ShowGameHistory)
            {

                ShowGameHistory(gamesDatabase);

                AnsiConsole.MarkupLine("[blue]Please Enter to continue...[/]");
                Console.ReadKey();
                continue;
            }
            else if (selectedOption == GameOptions.IncreaseDifficulty)
            {
                if (level == DifficultyLevel.Hard)
                {
                    AnsiConsole.MarkupLine("[red]Cannot increase difficulty level further![/]");
                    Console.ReadKey(true); // Wait for a key press
                }
                else
                {
                    level++;
                }
                continue;
            }
            else if (selectedOption == GameOptions.DecreaseDifficulty)
            {
                if (level == DifficultyLevel.Easy)
                {
                    AnsiConsole.MarkupLine("[red]Cannot decrease difficulty level further![/]");
                    Console.ReadKey(true); // Wait for a key press
                }
                else
                {
                    level--;
                }
                continue;
            }

            //
            // Process game Options
            //

            int? result = null;

            (int firstNumber, int secondNumber) = GetRandomNumbers(selectedOption, level, random);

            try
            {

                switch (selectedOption)
                {
                    case GameOptions.AdditionGame:
                        result = checked(firstNumber + secondNumber);
                        break;

                    case GameOptions.SubtractionGame:
                        result = checked(firstNumber - secondNumber);
                        break;

                    case GameOptions.MultiplicationGame:
                        result = checked(firstNumber * secondNumber);
                        break;

                    case GameOptions.DivisionGame:
                        result = checked(firstNumber / secondNumber);
                        break;

                    default:
                        AnsiConsole.MarkupLine($"[red]Internal Errror - Game option '{selectedOption.ToString()}' not implemented![/]");
                        Console.ReadKey(true); // Wait for a key press
                        break;
                }
            }
            catch (OverflowException)
            {
                AnsiConsole.MarkupLine("[red]Result exceeded the maximum or minumum value of an integer! Press any key to continue...[/]");
                Console.ReadKey(true); // Wait for a key press
                continue;
            }

            //
            // Display the game, and get the users result
            //

            string gamePlayed = $"{firstNumber} {selectedOption.ToDescriptiveOrFriendlyString()} {secondNumber}";

            AnsiConsole.MarkupLine($"What is: [yellow]{gamePlayed}[/] Equal to?");

            int response = AnsiConsole.Prompt(
                new TextPrompt<int>("[green]Your Answer:[/]")
                    .PromptStyle("green")
                    .Validate(number =>
                    {
                        if (number > int.MaxValue || number < int.MinValue)
                        {
                            return ValidationResult.Error("[red]Number must be within the range of an integer[/]");
                        }
                        return ValidationResult.Success();
                    })
            );

            if (result != null)
            {
                gameNumber++;
                gameWins += (result == response) ? 1 : 0;

                SaveAndDisplayResult(userName, gamesDatabase, selectedOption, gamePlayed + $" = {result} ",
                    (result == response ? GameResult.Correct : GameResult.Incorrect));

                AnsiConsole.Markup("[blue]Press any key to continue...[/]");
                Console.ReadKey(true); // Wait for a key press
            }

        }

    }
    private static void SaveAndDisplayResult(string userName, GamesDatabase gamesDatabase, GameOptions selectedOption, string gamePlayed, GameResult outcome)
    {
        Console.WriteLine();
        Console.WriteLine();
        AnsiConsole.MarkupLine("[blue]MathGame result:[/]");

        gamesDatabase.GamesHistory.Add(new GamesHistoryItem
        {
            Id = gamesDatabase.GamesHistory.Any() ? gamesDatabase.GamesHistory.Max(game => game.Id) + 1 : 1,
            PlayerName = userName,
            GameType = selectedOption,
            GameDate = DateTime.Now,
            GamePlay = gamePlayed,
            GameResult = outcome
        });

        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn("[yellow]Player Name[/]");
        table.AddColumn("[yellow]Game Type[/]");
        table.AddColumn("[yellow]Play[/]");
        table.AddColumn("[yellow]Result[/]");

        table.AddRow(
            $"[cyan]{userName}[/]",
            $"[green]{selectedOption}[/]",
            gamePlayed,
            (outcome == GameResult.Correct ? "[blue]" : "[red]") + $"{outcome.ToString()}[/]"
            );

        AnsiConsole.Write(table);
    }

    //
    // Display the game history
    //
    private void ShowGameHistory(GamesDatabase gamesDatabase)
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn("[yellow]ID[/]");
        table.AddColumn("[yellow]Player Name[/]");
        table.AddColumn("[yellow]Game Type[/]");
        table.AddColumn("[yellow]Date[/]");
        table.AddColumn("[yellow]Play[/]");
        table.AddColumn("[yellow]Result[/]");

        foreach (var game in gamesDatabase.GamesHistory)
        {
            table.AddRow(
                game.Id.ToString(),
                $"[cyan]{game.PlayerName}[/]",
                $"[green]{game.GameType}[/]",
                game.GameDate.ToString(),
                game.GamePlay,
                (game.GameResult == GameResult.Correct ? "[blue]" : "[red]") + $"{game.GameResult.ToString()}[/]"
                );
        }

        AnsiConsole.Write(table);
    }

    //
    // Randomly generate two numbers, and 
    // for division check the result is a whole number
    //

    private (int firstNumber, int secondNumber) GetRandomNumbers(GameOptions selectedOption, DifficultyLevel level, Random random)
    {
        AnsiConsole.MarkupLine($"  Game: [bold]{selectedOption.ToString()}[/]");

        // Easy: 1-10, Medium: 10-100, Hard: 100-1000

        int firstNumber = random.Next((int)Math.Pow(10, (int)level - 1), 1 + (int)Math.Pow(10, (int)level));
        int secondNumber = random.Next((int)Math.Pow(10, (int)level - 1), 1 + (int)Math.Pow(10, (int)level));

        if (selectedOption == GameOptions.DivisionGame)
        {
            // 1. Diviser cannot be > 100
            // 2. Division result must be whole number

            secondNumber = Math.Min(secondNumber, 100);

            if (firstNumber % secondNumber != 0)
            {
                // instead of looping, we can just multiply the divisor by a random number, to get a whole number divisible by it
                firstNumber = secondNumber * random.Next(1, 1 + (int)Math.Pow(10, (int)level));
            }
        }

        return (firstNumber, secondNumber);

    }
}


