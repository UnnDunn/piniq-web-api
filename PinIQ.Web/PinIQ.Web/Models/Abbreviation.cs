using Pinball.Entities;

namespace Pinball.Web.Models
{
    public class Abbreviation : AbbreviationEntity
    {
        public PinballTable Table { get; set; }
    }
}