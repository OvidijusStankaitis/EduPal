using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSI_Project.Models;

public class Note : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new string Id { get; } = Guid.NewGuid().ToString(); // If you want to generate GUID in code, not by database

    public string Name { get; set; }
    public string Content { get; set; }
    
    public Note()
    {
        // The Id is initialized when the object is constructed
    }
    
    public Note(string noteName, string noteContent) : this() // Call the parameterless constructor
    {
        Name = noteName;
        Content = noteContent;
    }
}