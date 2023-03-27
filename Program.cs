using NLog;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");


string scrubbedFile = FileScrubber.ScrubMovies("movies.csv");
logger.Info(scrubbedFile);

MovieFile movieFile = new MovieFile(scrubbedFile);

string choice = "";
do
{
  // display choices to user
  Console.WriteLine("1) Add Movie");
  Console.WriteLine("2) Display All Movies");
  Console.WriteLine("3) Find Movie");
  Console.WriteLine("Enter to quit");
  // input selection
  choice = Console.ReadLine();
  
  if (choice == "1")
  {
    // Add movie
    Movie movie = new Movie();
    // ask user to input movie title
    Console.WriteLine("Enter movie title");
    // input title
    movie.title = Console.ReadLine();
    // verify title is unique
    if (movieFile.isUniqueTitle(movie.title)){

        // ask user to input movie director
        Console.WriteLine("Enter movie director");
        // input title
        movie.director = Console.ReadLine();

        // ask user to input movie running time
        Console.WriteLine("Enter movie running time (hh:mm:ss)");
        movie.runningTime = TimeSpan.Parse(Console.ReadLine());


      // input genres
      string input;
      do
      {
        // ask user to enter genre
        Console.WriteLine("Enter genre (or done to quit)");
        // input genre
        input = Console.ReadLine();
        // if user enters "done"
        // or does not enter a genre do not add it to list
        if (input != "done" && input.Length > 0)
        {
          movie.genres.Add(input);
        }
      } while (input != "done");
      // specify if no genres are entered
      if (movie.genres.Count == 0)
      {
        movie.genres.Add("(no genres listed)");
      }
            // add movie
      movieFile.AddMovie(movie);
    }
  } else if (choice == "2")
  {
    // Display All Movies
    foreach(Movie m in movieFile.Movies)
    {
      Console.WriteLine(m.Display());
    }
  } else if (choice == "3")
  {
    // find movie
    
    // ask user to input movie title
    Console.WriteLine("Enter movie title");
    // input title
    var movieTitleToFind = Console.ReadLine();

    // filter for possible matches
    var movieMatches = movieFile.Movies.Where(m => m.title.Contains(movieTitleToFind));
    
    // count and display how many possible matches were found
    int num = movieFile.Movies.Where(m => m.title.Contains(movieTitleToFind)).Count();
    Console.WriteLine($"There are {num} possible matches: ");   

    //display the titles which may match the user input
    foreach(Movie m in movieMatches)
    {
        Console.WriteLine($"  {m.title}");
    }
  }
  logger.Info("User choice: {Choice}", choice);
} while (choice == "1" || choice == "2" || choice == "3");


logger.Info("Program ended");
