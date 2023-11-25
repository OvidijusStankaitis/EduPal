using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Services
{
    public class ConspectusService
    {
        private ConspectusRepository _conspectusRepository;

        public ConspectusService(ConspectusRepository conspectusRepository)
        {
            _conspectusRepository = conspectusRepository;
        }

        public async Task<List<Conspectus>> GetConspectusesAsync(string topicId)
        {
            IEnumerable<Conspectus> conspectusList = await _conspectusRepository.GetConspectusListByTopicIdAsync(topicId);
            List<Conspectus> sortedConspectusList = conspectusList.ToList();
            sortedConspectusList.Sort();
            return sortedConspectusList;
        }
    }
}