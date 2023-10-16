namespace PSI_Project.Models;

public struct PersonName  // 1: custom struct usage
{
    private string _name;   // 2: property usage in struct
    private string _surname;

    public string Name
    {
        get => _name;
        set
        {
            if (!_name.IsValidPersonName())
                throw new Exception("invalid first name");
            
            _name = value;
        }
    }

    public string Surname
    {
        get => _surname;
        set
        {
            if (_surname.IsValidPersonName())
                throw new Exception("invalid last name");
            
            _surname = value;
        }
    }

    public string FullName => Name + " " + Surname;
}