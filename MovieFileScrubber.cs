using NLog;
public static class FileScrubber
{
    private static NLog.Logger logger = LogManager.LoadConfiguration(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
    public static string ScrubMovies(string readFile)
    {
        try
        {
            // determine name of writeFile
            string ext = readFile.Split('.').Last();
            string writeFile = readFile.Replace(ext, $"scrubbed.{ext}");
            // if writeFile exists, the file has already been scrubbed
            if (File.Exists(writeFile))
            {
                // file has already been scrubbed
                logger.Info("File already scrubbed");
            }
            else
            {
                // file has not been scrubbed
                logger.Info("File scrub started");
                // open write file
                StreamWriter sw = new StreamWriter(writeFile);
                // open read file
                StreamReader sr = new StreamReader(readFile);
                // remove first line - column headers
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    // create instance of Movie class
                    Movie movie = new Movie();
                    string line = sr.ReadLine();
                    // look for quote(") in string
                    // this indicates a comma(,) or quote(") in movie title
                    int idx = line.IndexOf('"');
                    string genres = "";
                    if (idx == -1)
                    {
                        // no quote = no comma or quote in movie title
                        // movie details are separated with comma(,)
                        string[] movieDetails = line.Split(',');
                        movie.mediaId = UInt64.Parse(movieDetails[0]);
                        movie.title = movieDetails[1];
                        genres = movieDetails[2];
                        movie.director = movieDetails.Length > 3 ? movieDetails[3] : "unassigned";
                        movie.runningTime = movieDetails.Length > 4 ? TimeSpan.Parse(movieDetails[4]) : new TimeSpan(0);
                    }
                    else
                    {
                        // quote = comma or quotes in movie title
                        // extract the movieId
                        movie.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                        // remove movieId and first comma from string
                        line = line.Substring(idx);
                        // find the last quote
                        idx = line.LastIndexOf('"');
                        // extract title
                        movie.title = line.Substring(0, idx + 1);
                        // remove title and next comma from the string
                        line = line.Substring(idx + 2);
                        // split the remaining string based on commas
                        string[] details = line.Split(',');
                        // the first item in the array should be genres 
                        genres = details[0];
                        // if there is another item in the array it should be director
                        movie.director = details.Length > 1 ? details[1] : "unassigned";
                        // if there is another item in the array it should be run time
                        movie.runningTime = details.Length > 2 ? TimeSpan.Parse(details[2]) : new TimeSpan(0);
                    }
                    sw.WriteLine($"{movie.mediaId},{movie.title},{genres},{movie.director},{movie.runningTime}");
                }
                sw.Close();
                sr.Close();
                logger.Info("File scrub ended");
            }
            return writeFile;
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }
        return "";
    }
}
