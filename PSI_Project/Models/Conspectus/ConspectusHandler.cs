namespace PSI_Project;

public class ConspectusHandler
{
    public List<Conspectus> ConspectusList = new();
    private const string DatabaseFilePath = "DB/conspectus.txt";    // TODO: delete when database is added
    
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
                
                String[] conspectusData = line.Split(" ");
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

    public void UploadConspectus(Conspectus conspectus)
    {
        try
        {
            StreamWriter sw = File.AppendText(DatabaseFilePath);
            sw.WriteLine(conspectus.Id + " " + conspectus.Path);
            
            ConspectusList.Add(conspectus);
            
            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public List<Conspectus> RemoveConspectus(Conspectus conspectus)
    {
        // TODO: finish
        try
        {
            StreamReader sr = File.OpenText(DatabaseFilePath);
            
            while(!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                
                if (line == null)
                    continue;

                string conspectusId = line.Split(" ")[0];
                if (conspectusId == conspectus.Id)
                {
                    // delete line
                }

                Console.WriteLine(conspectusId);
            }
            
            sr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return ConspectusList;
    }

}