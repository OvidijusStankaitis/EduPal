namespace PSI_Project;

public class ConspectusHandler
{
    public List<Conspectus> ConspectusList = new();
    private const string DatabaseFilePath = "DB/conspectus.txt";    // TODO: delete when database is added
    private const string TempDatabaseFilePath = "DB/temp_conspectus.txt"; 
    
    public ConspectusHandler()
    {
        LoadConspectusList();
    }

    public void LoadConspectusList()
    {
        if (!File.Exists(DatabaseFilePath))
            return;
        
        try
        {
            StreamReader sr = File.OpenText(DatabaseFilePath);

            while(!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                
                if (line == null)
                    continue;
                
                String[] conspectusData = line.Split(";");
                Conspectus conspectus = new Conspectus(conspectusData[0], conspectusData[1]);
                ConspectusList.Add(conspectus);
            }

            sr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public Conspectus? GetConspectusById(string id)
    {
        foreach(Conspectus conspectus in ConspectusList)
        {
            if (conspectus.Id == id)
                return conspectus;
        }

        return null;
    }

    public void UploadConspectus(Conspectus conspectus)
    {
        try
        {
            StreamWriter sw = File.AppendText(DatabaseFilePath);
            sw.WriteLine(conspectus.Id + ";" + conspectus.Path + ";");
            
            ConspectusList.Add(conspectus);
            
            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public List<Conspectus> RemoveConspectus(string conspectusId) //why List<Conspectus> MAYBE VOID??? 
    {
        try
        {
            // renaming conspectus.txt to temp_conspectus.txt, opening stream reader
            FileInfo tempFile = new FileInfo(DatabaseFilePath);
            tempFile.MoveTo(TempDatabaseFilePath);
            StreamReader sr = File.OpenText(TempDatabaseFilePath);
            
            // creating stream reader to save altered database
            StreamWriter sw = File.CreateText(DatabaseFilePath);
            
            // deleting conspectus from databas
            string? line;
            while((line = sr.ReadLine()) != null)
            {
                string dbConspectusId = line.Split(";")[0];
                if (dbConspectusId == conspectusId)
                    continue;
                
                sw.WriteLine(line);
            }
            
            // closing filestreams
            sw.Close();
            sr.Close();
            
            // deleting temp file
            tempFile.Delete();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return ConspectusList;
    }

}