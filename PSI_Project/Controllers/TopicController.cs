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
    
    [HttpPost("{topicOldName}/modify")]
    public IActionResult ModifyTopic(string topicOldName,string topicName, string topicDescription, string subjectName)
    {
        Topic? oldTopic = _topicHandler.CheckItemInList(topicOldName);
        if (oldTopic != null)
        {
            _topicHandler.ModifyItem(oldTopic, new Topic(topicName, topicDescription, subjectName));
            return Ok(_topicHandler.ItemList);
        }
        return NotFound();
    }
    
    [HttpDelete("{topicName}/delete")] //should "delete/" be here?
    public void RemoveTopic(string topicName)
    {
        Topic? topicToRemove = _topicHandler.CheckItemInList(topicName);
        if (topicToRemove != null)
        {
            _topicHandler.RemoveItem(topicToRemove);
        }
    }
}