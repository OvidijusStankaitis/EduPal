using Microsoft.AspNetCore.Mvc;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController : ControllerBase
{
    private readonly TopicHandler _topicHandler = new TopicHandler();

    [HttpGet]
    public IActionResult ListTopics()
    {
        return Ok(_topicHandler.ItemList);
    }
    
    [HttpPost("upload")]
    public IActionResult UploadTopic(string topicName, string topicDescription, string subjectName)
    {
        _topicHandler.CreateItem(new Topic(topicName, topicDescription, subjectName));
        return Ok(_topicHandler.ItemList);
    }
    
    [HttpPost("modify")]
    public IActionResult ModifyTopic(Topic oldTopic,string topicName, string topicDescription, string subjectName) 
    {
        _topicHandler.ModifyItem(oldTopic, new Topic(topicName, topicDescription, subjectName));
        return Ok(_topicHandler.ItemList);
    }
    
    [HttpDelete("delete/{topicName}")] //should "delete/" be here?
    public void RemoveTopic(string topicName)
    {
        Topic? topicToRemove = null;
        foreach (var topic in _topicHandler.ItemList)
        {
            if (topicToRemove.Name == topicName)
            {
                topicToRemove = topic;
                break;
            }
        }
        if (topicToRemove != null)
        {
            _topicHandler.RemoveItem(topicToRemove);
        }
    }
}