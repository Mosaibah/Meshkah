using Meshkah.Models;

namespace Meshkah.ViewModels
{
    public class PointsTransactionVM
    {
        public int? PointId { get; set; }

        public virtual Point? Point { get; set; }
        public List<long> Selected_Users { get; set; }


    }
}
