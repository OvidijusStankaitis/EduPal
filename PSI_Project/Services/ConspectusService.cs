using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Services;

public class ConspectusService
{
    private ConspectusRepository _conspectusRepository;

    public ConspectusService(ConspectusRepository conspectusRepository)
    {
        _conspectusRepository = conspectusRepository;
    }

    public List<Conspectus> GetConspectuses(string topicId)
    {
        List<Conspectus> conspectusList = _conspectusRepository.GetConspectusListByTopicId(topicId).ToList();
        conspectusList.Sort();
        return conspectusList;
    }

}